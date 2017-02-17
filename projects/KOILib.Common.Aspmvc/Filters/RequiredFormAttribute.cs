using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using KOILib.Common.Core.Extensions;

namespace KOILib.Common.Aspmvc.Filters
{
    public class RequiredFormAttribute : FilterAttribute, IAuthorizationFilter
    {
        public IEnumerable<string> Names { get; private set; }

        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            var req = filterContext.HttpContext.Request;
            var valid = Names.All(name => !string.IsNullOrEmpty(req.Form[name]));

            if (!valid)
                throw new Exception("invalid in requirements."); //TODO
        }

        public RequiredFormAttribute(string commaseparatedName)
        {
            this.Names = commaseparatedName.SplitByCommaOrFeed();
        }
    }
}
