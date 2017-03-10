using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common
{
    /// <summary>
    /// IDisposable 実装
    /// </summary>
    public class Disposable : IDisposable
    {
        public static IDisposable Create(Action dispose)
        {
            var disposable = new Disposable()
            {
                _dispose = dispose
            };
            return disposable;
        }

        private Action _dispose;

        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_dispose != null)
                        _dispose();
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }

        protected Disposable()
        {
        }
    }
}
