using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace KOILib.Common.Aspmvc.Helpers
{
    public static class JsonHelperExtension
    {
        public static IHtmlString Serialize(this JsonHelper self, object value)
        {
            return MvcHtmlString.Create(JsonConvert.SerializeObject(value));
        }

    }
}
