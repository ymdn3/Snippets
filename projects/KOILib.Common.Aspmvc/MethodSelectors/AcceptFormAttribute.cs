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
    /// Formに特定の値のパラメータが存在するかどうかで妥当性を判断するメソッドセレクター
    /// </summary>
    public class AcceptFormAttribute : ActionMethodSelectorAttribute
    {
        /// <summary>
        /// チェック対象のFormパラメータ名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Formパラメータの値。
        /// 指定しないときNameが存在すれば妥当となる。
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
            var req = controllerContext.HttpContext.Request;

            var formValue = req.Form[Name];
            var hasName = formValue != null;

            if (!hasName)
                return false;

            if (Values == null)
                return hasName;

            if (IgnoreCase)
                return formValue.EqualsAny(Values, StringComparison.OrdinalIgnoreCase);
            else
                return formValue.EqualsAny(Values, StringComparison.Ordinal);
        }

        public AcceptFormAttribute(string name, string value, params string[] ormore)
        {
            this.Name = name;
            this.Values = ormore.Prepend(value);
        }
        public AcceptFormAttribute(string name, string value)
        {
            this.Name = name;
            this.Values = new string[] { value };
        }
        public AcceptFormAttribute(string name)
        {
            this.Name = name;
            this.Values = null;
        }
    }
}
