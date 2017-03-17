using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using KOILib.Common.Log4;

namespace KOILib.Common.Aspmvc.Filters
{
    public class GeneralHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("fillterContext");

            var ex = filterContext.Exception;
            filterContext.Controller.TempData["EXCEPTION"] = ex;
            
            //logging
            if (this is ILog4Logging)
            {
                var logging = (ILog4Logging)this;
                if (logging.Logger != null)
                    logging.Logger.Write(logging.LogLevel, ex);
            }

            //HttpAntiForgeryException の場合はHTTP401を返す
            if (ex is HttpAntiForgeryException)
            {
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                //Application_Errorは呼ばれない
                HandleAjaxRequestException(filterContext);
            }
            else
            {
                //カスタムエラーが有効でなければbase.OnException()でもExceptionHandledがtrueにならないので
                //Application_Errorも呼ばれる
                base.OnException(filterContext);
            }
        }

        private void HandleAjaxRequestException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            filterContext.Result = new JsonResult()
            {
                Data = new
                {
                    caption = filterContext.Exception.Message,
                    message = filterContext.Exception.ToString(),
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}
