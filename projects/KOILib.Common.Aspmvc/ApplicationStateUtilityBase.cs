using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using KOILib.Common.Extensions;

namespace KOILib.Common.Aspmvc
{
    public abstract class ApplicationStateUtilityBase
    {
        private HttpApplicationStateBase _httpApplication;

        #region 同期ロック
        /// <summary>
        /// Dispose時にUnlock()するオブジェクトを生成します。using()で利用ください。
        /// </summary>
        public IDisposable Lock()
        {
            _httpApplication.Lock();
            return Disposable.Create(() => _httpApplication.UnLock());
        }
        #endregion

        #region 有効期限管理
        private static readonly string _keyOfExpireTime = "##EXPIRE_TIME##";
        protected TimeSpan Lifetime { get; set; }
        private ConcurrentDictionary<string, DateTime> _expireTime
        {
            get
            {
                using (this.Lock())
                {
                    if (_httpApplication[_keyOfExpireTime] != null)
                        return (ConcurrentDictionary<string, DateTime>)_httpApplication[_keyOfExpireTime];

                    _httpApplication[_keyOfExpireTime] = new ConcurrentDictionary<string, DateTime>();
                    return _expireTime;
                }
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

        protected TValue Get<TValue>(string key)
        {
            //有効期限切れオブジェクトの整理
            SweepExpired();

            if (_httpApplication[key] == null)
                return default(TValue);

            var value = _httpApplication[key];

            //参照での有効期限延長なし
            //ResetExpire(key);

            return (TValue)value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="noExpire">trueのとき無期限、falseのとき有効期限ありで、値がセットされた場合のみ有効期限をリセットします</param>
        public void Set<TValue>(string key, TValue value, bool noExpire)
        {
            using (this.Lock())
            {
                _httpApplication.Remove(key);
                _httpApplication[key] = value;

                if (!noExpire)
                    SetExpire(key);
            }
        }

        public void Remove(string key)
        {
            using (this.Lock())
            {
                _httpApplication.Remove(key);

                var t = DateTime.Now;
                _expireTime.TryRemove(key, out t);
            }
        }

        public bool Exists(string key)
        {
            if (IsExpired(key))
                return false;
            return (_httpApplication[key] != null);
        }

        public ApplicationStateUtilityBase(HttpContext context) 
            : this(new HttpApplicationStateWrapper(context.ApplicationInstance.Application))
        {
        }
        public ApplicationStateUtilityBase(HttpApplicationState state) 
            : this(new HttpApplicationStateWrapper(state))
        {
        }
        public ApplicationStateUtilityBase(HttpApplicationStateBase state)
        {
            _httpApplication = state;
            Lifetime = new TimeSpan(0, 20, 0);
        }
    }
}
