using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common
{
    /// <summary>
    /// イベントハンドラ登録・解除
    /// </summary>
    public class EventSubscriber : Disposable
    {
        public static IDisposable Create(object target, string eventname, Delegate handler)
        {
            var eventinfo = target.GetType().GetEvent(eventname);
            if (eventinfo != null)
            {
                eventinfo.AddEventHandler(target, handler);
                return Create(() => { eventinfo.RemoveEventHandler(target, handler); });
            }
            else
            {
                return Create(null);
            }
        }
    }
}
