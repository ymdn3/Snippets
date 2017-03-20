using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using KOILib.Common.Extensions;

namespace KOILib.Common.Aspmvc.MethodSelectors
{
    /// <summary>
    /// ルートパラメータに特定の値が存在するかどうかで妥当性を判断するメソッドセレクター
    /// </summary>
    public class AcceptRouteAttribute : ActionMethodSelectorAttribute
    {
        /// <summary>
        /// チェック対象のルートパラメータ名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 妥当と判断するルートパラメータの値。
        /// </summary>
        public IEnumerable<string> Values { get; private set; } 

        /// <summary>
        /// 英字の大小を区別しないかどうか
        /// </summary>
        public bool IgnoreCase { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            var routeData = controllerContext.RouteData;

            if (routeData.Values[Name] == null)
                return false;

            var requestValue = routeData.GetRequiredString(Name);
            if (IgnoreCase)
                return requestValue.EqualsAny(Values, StringComparison.OrdinalIgnoreCase);
            else
                return requestValue.EqualsAny(Values, StringComparison.Ordinal);
        }

        /// <summary>
        /// 指定したいずれかの値との一致セレクターを生成します
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="ormore"></param>
        public AcceptRouteAttribute(string name, string value, params string[] ormore)
        {
            this.Name = name;
            this.Values = ormore.Prepend(value);
        }

        /// <summary>
        /// 指定した値との一致セレクターを生成します
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public AcceptRouteAttribute(string name, string value)
        {
            this.Name = name;
            this.Values = new string[] { value };
        }
    }
}
