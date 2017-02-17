using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace KOILib.Common.Aspmvc
{
    public abstract class SessionUtilityBase
    {
        private HttpSessionStateBase _httpSession;

        protected T Get<T>(string key, bool atOnce)
        {
            if (_httpSession[key] == null)
                return default(T);

            var value = _httpSession[key];

            if (atOnce)
                _httpSession.Remove(key);

            return (T)value;
        }

        public void Abandon()
        {
            _httpSession.Clear();
            _httpSession.Abandon();
        }

        public void Remove(string key)
        {
            _httpSession.Remove(key);
        }

        public void Set<T>(string key, T value)
        {
            _httpSession.Remove(key);
            _httpSession[key] = value;
        }

        public bool Exists(string key)
        {
            return (_httpSession[key] != null);
        }

        public T GetOnce<T>(string key)
        {
            return Get<T>(key, true);
        }

        public SessionUtilityBase(HttpContext context)
        {
            _httpSession = new HttpSessionStateWrapper(context.Session);
        }
        public SessionUtilityBase(HttpSessionState session)
        {
            _httpSession = new HttpSessionStateWrapper(session);
        }
        public SessionUtilityBase(HttpSessionStateBase session)
        {
            _httpSession = session;
        }
    }
}
