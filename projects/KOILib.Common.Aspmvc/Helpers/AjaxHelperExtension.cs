using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using KOILib.Common.Extensions;

namespace KOILib.Common.Aspmvc.Helpers
{
    public static class AjaxHelperExtension
    {
        public static MvcForm BeginForm<TModel>(this AjaxHelper<TModel> self, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return self.BeginForm(null, null, ajaxOptions, Utils.ToFlattenDictionary(htmlAttributes, "-"));
        }
    }
}
