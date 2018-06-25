using System;
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
        private static readonly string _keyOfExpireTime = "##EXPIRE_TIME##";
        protected TimeSpan Lifetime { get; set; }
        private ConcurrentDictionary<string, DateTime> _expireTime
        {
            get
            {
                if (_HttpSession[_keyOfExpireTime] != null)
                    return (ConcurrentDictionary<string, DateTime>)_HttpSession[_keyOfExpireTime];

                _HttpSession[_keyOfExpireTime] = new ConcurrentDictionary<string, DateTime>();
                return _expireTime;
            }
        }
        /// <summary>
        /// 期限切れのキー値をすべて削除します
        /// </summary>
        protected void SweepExpired()
        {
            var utcReferenceTime = DateTime.UtcNow;
            var expires = _expireTime
                .Where(x => x.Value < utcReferenceTime)
                .ToArray();
            expires
                .Each(x => Remove(x.Key));
        }
        /// <summary>
        /// 指定のキーが期限切れかどうかを判断します
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected bool IsExpired(string key)
        {
            var utcReferenceTime = DateTime.UtcNow;
            var utcExpireTime = DateTime.UtcNow;
            if (_expireTime.TryGetValue(key, out utcExpireTime))
                if (utcExpireTime < utcReferenceTime)
                    return true;
            return false;
        }
        /// <summary>
        /// 指定のキー値に所定の有効期限を設定します
        /// </summary>
        /// <param name="key"></param>
        protected void SetExpire(string key)
        {
            var expire = DateTime.UtcNow.Add(Lifetime);
            _expireTime.AddOrUpdate(key, expire, (k, v) => expire);
        }
        /// <summary>
        /// 指定のキー値が有効期限管理されている場合に、有効期限をリセットします
        /// </summary>
        /// <param name="key"></param>
        protected void ResetExpire(string key)
        {
            var current = DateTime.MinValue;
            if (_expireTime.TryGetValue(key, out current))
                SetExpire(key);
        }
        /// <summary>
        /// 指定のキーを期限切れにします
        /// </summary>
        /// <param name="key"></param>
        protected void Expire(string key)
        {
            var current = DateTime.MinValue;
            if (_expireTime.TryGetValue(key, out current))
                _expireTime.TryUpdate(key, DateTime.MinValue, current);
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
            _expireTime.TryRemove(key, out t);
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
