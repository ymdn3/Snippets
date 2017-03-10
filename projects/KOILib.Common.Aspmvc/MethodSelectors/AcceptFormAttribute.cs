using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

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
        public string Value { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            var req = controllerContext.HttpContext.Request;

            return Value != null
                ? string.Equals(Value, req.Form[Name], StringComparison.OrdinalIgnoreCase)
                : req.Form[Name] != null;
        }

        public AcceptFormAttribute(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
        public AcceptFormAttribute(string name) : this(name, null)
        {
        }
    }
}
