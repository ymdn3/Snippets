using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KOILib.Common.Aspmvc
{
    public static class Extension
    {
        public static IHtmlString ToHtmlString(this TagBuilder self, TagRenderMode tagRenderMode)
        {
            return MvcHtmlString.Create(self.ToString(tagRenderMode));
        }
        public static IHtmlString ToHtmlString(this TagBuilder self)
        {
            return MvcHtmlString.Create(self.ToString());
        }

        /// <summary>
        /// 現在の HttpContext から呼ぶことのできる HttpWordkerRequest を元にコピーを生成し、HttpContext.Current の値に設定します。
        /// Task実行のケースでは、HttpContext.Current が null となるため、当メソッドでコピーを再設定します。
        /// ※あくまでコピーであることに注意。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="forced">trueのとき、HttpContext.Currentがnullかどうかにかかわらずセットします。</param>
        public static void TakeoverCurrent(this HttpContextBase self, bool forced = false)
        {
            if (forced || HttpContext.Current == null)
            {
                var provider = (IServiceProvider)self;
                var wr = (HttpWorkerRequest)provider.GetService(typeof(HttpWorkerRequest));
                HttpContext.Current = new HttpContext(wr);
            }
        }
    }
}
