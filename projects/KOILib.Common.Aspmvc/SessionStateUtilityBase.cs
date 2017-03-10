using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

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

            return (T)value;
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
        }

        public void Set<T>(string key, T value)
        {
            _HttpSession.Remove(key);
            _HttpSession[key] = value;
        }

        public bool Exists(string key)
        {
            return (_HttpSession[key] != null);
        }

        public T GetOnce<T>(string key)
        {
            return Get<T>(key, true);
        }

        public SessionStateUtilityBase(HttpContext context)
        {
            _HttpSession = new HttpSessionStateWrapper(context.Session);
        }
        public SessionStateUtilityBase(HttpSessionState session)
        {
            _HttpSession = new HttpSessionStateWrapper(session);
        }
        public SessionStateUtilityBase(HttpSessionStateBase session)
        {
            _HttpSession = session;
        }
    }
}
