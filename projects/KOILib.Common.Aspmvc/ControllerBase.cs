using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        //JsonResultがJSON.NETを使うようオーバーライド
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
            ////標準
            //return base.Json(data, contentType, contentEncoding, behavior);
        }

        protected HttpStatusCodeResult HttpStatusBadRequest(string description = null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, description);
        }
        protected HttpStatusCodeResult HttpStatusForbidden(string description = null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden, description);
        }
        protected HttpStatusCodeResult HttpStatusNoContent(string description = null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.NoContent, description);
        }

    }
}
