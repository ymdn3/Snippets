using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KOILib.Common.Aspmvc.Controllers
{
    public class AcceptParameterAttribute : ActionMethodSelectorAttribute
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            var req = controllerContext.RequestContext.HttpContext.Request;
            return string.IsNullOrEmpty(this.Value)
                ? !string.IsNullOrEmpty(req.Form[this.Name])
                : string.Equals(req.Form[this.Name], this.Value, StringComparison.InvariantCultureIgnoreCase);
        }

    }
}
