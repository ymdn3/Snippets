using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;

namespace KOILib.Common.Aspmvc.Helpers
{
    public static class AjaxHelperExtension
    {
        public static MvcForm BeginForm<TModel>(this AjaxHelper<TModel> self, AjaxOptions ajaxOptions, string id)
        {
            return self.BeginForm(null, null, ajaxOptions, new { id = id });
        }
    }
}
