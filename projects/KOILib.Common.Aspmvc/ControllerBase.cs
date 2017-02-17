using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace KOILib.Common.Aspmvc
{
    public abstract class ControllerBase : Controller
    {

        protected virtual void OnControllerBeginning(AuthenticationContext context)
        {
        }

        protected virtual void OnControllerFinished(ResultExecutedContext context)
        {
        }

        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            OnControllerBeginning(filterContext);

            base.OnAuthentication(filterContext);
        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);

            OnControllerFinished(filterContext);
        }

    }
}
