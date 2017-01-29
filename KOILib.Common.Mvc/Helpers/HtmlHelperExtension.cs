using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KOILib.Common.Core.Extensions;

namespace KOILib.Common.Mvc.Helpers
{
    public static class HtmlHelperExtension
    {
        private static Dictionary<string, string> _inlineRenderCache = new Dictionary<string, string>();
        private static SHA1CryptoServiceProvider _sha1 = new SHA1CryptoServiceProvider();

        public static IHtmlString InlineRender<TModel>(this HtmlHelper<TModel> self, string[] virtualPathes)
        {
            var sb = new StringBuilder();
            using (var w = new StringWriter(sb))
            {
                virtualPathes.ParallelDo((virtualPath) => self.AppendTo(w, virtualPath));
            }
            return MvcHtmlString.Create(sb.ToString());
        }
        public static IHtmlString InlineRender<TModel>(this HtmlHelper<TModel> self, string virtualPath)
        {
            var sb = new StringBuilder();
            using (var w = new StringWriter(sb))
            {
                self.AppendTo(w, virtualPath);
            }
            return MvcHtmlString.Create(sb.ToString());
        }
        private static void AppendTo(this HtmlHelper self, StringWriter wr, string virtualPath)
        {
            var physicalPath = self.ViewContext.HttpContext.Server.MapPath(virtualPath);
            if (File.Exists(physicalPath))
            {
                using (var r = File.OpenText(physicalPath))
                {
                    var hash = BitConverter.ToString(_sha1.ComputeHash(r.BaseStream));
                    if (!_inlineRenderCache.ContainsKey(hash))
                    {
                        r.BaseStream.Position = 0;
                        _inlineRenderCache.Add(hash, r.ReadToEnd());
                    }
                    wr.Write(_inlineRenderCache[hash]);
                }
            }
        }
        
    }
}
