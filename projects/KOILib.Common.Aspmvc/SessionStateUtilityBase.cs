using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using KOILib.Common.Extensions;

namespace KOILib.Common.Aspmvc
{
    public abstract class SessionStateUtilityBase
    {
        protected HttpSessionStateBase _HttpSession;

        protected T Get<T>(string key, bool atOnce)
        {
            if (_HttpSession[key] == null)
                return default(T);

            var value = _HttpSession[key];

            if (atOnce)
                _HttpSession.Remove(key);

            _ExpireManager.ResetExpire(key);
            return (T)value;
        }

        public void Abandon()
        {
            _HttpSession.Abandon();
        }

        public void Clear()
        {
            _HttpSession.Clear();
            _ExpireManager.Clear();
        }

        public void Set<T>(string key, T value, bool noExpire)
        {
            _HttpSession.Remove(key);
            _HttpSession[key] = value;

            if (!noExpire)
                _ExpireManager.SetExpire(key);
        }
        public void Set<T>(string key, T value)
        {
            Set(key, value, noExpire: false);
        }

        public bool Exists(string key)
        {
            return (_HttpSession[key] != null);
        }

        public T GetOnce<T>(string key)
        {
            return Get<T>(key, true);
        }

        public SessionStateUtilityBase(HttpContext context) : this(new HttpSessionStateWrapper(context.Session))
        {
        }
        public SessionStateUtilityBase(HttpSessionState session) : this(new HttpSessionStateWrapper(session))
        {
        }
        public SessionStateUtilityBase(HttpSessionStateBase session)
        {
            _HttpSession = session;
            _ExpireManager = new ExpireManager(_HttpSession, 60 * 1000);
        }

        internal ExpireManager _ExpireManager;
        internal class ExpireManager : WatcherBase
        {
            private HttpSessionStateBase _session;
            private ConcurrentDictionary<string, DateTime> _expireTime;

            protected override void TimerElapsedBody()
            {
                Sweep(DateTime.UtcNow);
            }
            internal void Sweep(DateTime utcReferenceTime)
            {
                _expireTime
                    .Where(x => x.Value < utcReferenceTime)
                    .ParallelDo(x => Expire(x.Key));
            }
            internal void Expire(string key)
            {
                var t = DateTime.UtcNow;
                if (_expireTime.TryRemove(key, out t))
                    _session.Remove(key);
            }
            internal void SetExpire(string key)
            {
                var extend = _session.Timeout;// Timeout設定分の延長
                _expireTime.AddOrUpdate(key, DateTime.UtcNow.AddMinutes(extend), (_, t) => t.AddMinutes(extend));
            }
            internal void ResetExpire(string key)
            {
                if (_expireTime.ContainsKey(key))
                    SetExpire(key);
            }
            internal void Clear()
            {
                _expireTime.Clear();
            }
            public ExpireManager(HttpSessionStateBase session, int interval)
            {
                _session = session;
                _expireTime = new ConcurrentDictionary<string, DateTime>();
                this.Start(interval);
            }
        }
    }
}
