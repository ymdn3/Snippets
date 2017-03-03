using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace KOILib.Common.Aspmvc
{
    public abstract class ApplicationStateUtilityBase
    {
        private HttpApplicationStateBase _httpApplication;

        protected T Get<T>(string key)
        {
            if (_httpApplication[key] == null)
                return default(T);

            var value = _httpApplication[key];
            return (T)value;
        }

        public void Clear()
        {
            _httpApplication.Lock();
            _httpApplication.Clear();
            _httpApplication.UnLock();
        }

        public void Remove(string key)
        {
            _httpApplication.Lock();
            _httpApplication.Remove(key);
            _httpApplication.UnLock();
        }
        protected void Set<T>(string key, T value)
        {
            _httpApplication.Lock();
            _httpApplication.Remove(key);
            _httpApplication[key] = value;
            _httpApplication.UnLock();
        }

        public bool Exists(string key)
        {
            return (_httpApplication[key] != null);
        }

        public ApplicationStateUtilityBase(HttpContext context)
        {
            _httpApplication = new HttpApplicationStateWrapper(context.ApplicationInstance.Application);
        }
        public ApplicationStateUtilityBase(HttpApplicationState state)
        {
            _httpApplication = new HttpApplicationStateWrapper(state);
        }
        public ApplicationStateUtilityBase(HttpApplicationStateBase state)
        {
            _httpApplication = state;
        }
    }
}
