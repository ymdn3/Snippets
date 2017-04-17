﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using KOILib.Common.Extensions;
using Newtonsoft.Json;

namespace KOILib.Common.Aspmvc
{
    public abstract class SessionStateUtilityBase
    {
        protected HttpSessionStateBase _HttpSession;

        #region 有効期限管理
        protected static readonly string _KeyOfExpireTime = "##EXPIRE_TIME##";
        protected TimeSpan Lifetime { get; set; }
        protected ConcurrentDictionary<string, DateTime> _ExpireTime
        {
            get
            {
                if (_HttpSession[_KeyOfExpireTime] != null)
                    return (ConcurrentDictionary<string, DateTime>)_HttpSession[_KeyOfExpireTime];

                _HttpSession[_KeyOfExpireTime] = new ConcurrentDictionary<string, DateTime>();
                return _ExpireTime;
            }
        }
        /// <summary>
        /// 期限切れのキー値をすべて削除します
        /// </summary>
        protected void SweepExpired()
        {
            lock (_ExpireTime)
            {
                var utcReferenceTime = DateTime.UtcNow;
                _ExpireTime
                    .Where(x => x.Value < utcReferenceTime)
                    .Each(x => Remove(x.Key));
            }
        }
        /// <summary>
        /// 指定のキーが期限切れかどうかを判断します
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected bool IsExpired(string key)
        {
            lock (_ExpireTime)
            {
                var utcReferenceTime = DateTime.UtcNow;
                if (_ExpireTime.ContainsKey(key))
                    if (_ExpireTime[key] < utcReferenceTime)
                        return true;
                return false;
            }
        }
        /// <summary>
        /// 指定のキー値に所定の有効期限を設定します
        /// </summary>
        /// <param name="key"></param>
        protected void SetExpire(string key)
        {
            lock (_ExpireTime)
            {
                var expire = DateTime.UtcNow.Add(Lifetime);
                _ExpireTime.AddOrUpdate(key, expire, (k, v) => expire);
            }
        }
        /// <summary>
        /// 指定のキー値が有効期限管理されている場合に、有効期限をリセットします
        /// </summary>
        /// <param name="key"></param>
        protected void ResetExpire(string key)
        {
            lock (_ExpireTime)
            {
                if (_ExpireTime.ContainsKey(key))
                {
                    var expire = DateTime.UtcNow.Add(Lifetime);
                    _ExpireTime.TryUpdate(key, expire, _ExpireTime[key]);
                }
            }
        }
        #endregion

        #region JSON変換
        /// <summary>
        /// JSON形式に変換してセットします
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">格納キー</param>
        /// <param name="value"></param>
        protected void SetJsonified<T>(string key, T value)
        {
            var json = JsonConvert.SerializeObject(value);
            Set(key, json);
        }

        /// <summary>
        /// JSON形式でセットされた値を取得します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">格納キー</param>
        /// <returns></returns>
        protected T GetOnceJsonified<T>(string key)
        {
            var json = Get<string>(key, atOnce: true);
            if (json != null)
                return JsonConvert.DeserializeObject<T>(json);
            else
                return default(T);
        }
        #endregion

        protected TValue Get<TValue>(string key, bool atOnce)
        {
            //有効期限切れオブジェクトの整理
            SweepExpired();

            if (_HttpSession[key] == null)
                return default(TValue);

            var value = _HttpSession[key];

            if (atOnce)
                this.Remove(key);

            //参照で有効期限を延長
            ResetExpire(key);

            return (TValue)value;
        }

        protected void Set<TValue>(string key, TValue value, bool noExpire)
        {
            _HttpSession.Remove(key);
            _HttpSession[key] = value;

            if (!noExpire)
                SetExpire(key);
        }

        public void Set<TValue>(string key, TValue value)
        {
            Set(key, value, noExpire: false);
        }

        public void Abandon()
        {
            _HttpSession.Abandon();
        }

        public void Clear()
        {
            _HttpSession.Clear();
        }

        public void Remove(string key)
        {
            _HttpSession.Remove(key);

            var t = DateTime.Now;
            _ExpireTime.TryRemove(key, out t);
        }

        public bool Exists(string key)
        {
            if (IsExpired(key))
                return false;
            return (_HttpSession[key] != null);
        }

        public TValue GetOnce<TValue>(string key)
        {
            return Get<TValue>(key, true);
        }


        public SessionStateUtilityBase(HttpContext context) : this(new HttpSessionStateWrapper(context.Session))
        {
        }
        public SessionStateUtilityBase(HttpSessionState state) : this(new HttpSessionStateWrapper(state))
        {
        }
        public SessionStateUtilityBase(HttpSessionStateBase state)
        {
            _HttpSession = state;
            Lifetime = new TimeSpan(0, state.Timeout, 0);
        }
    }
}
