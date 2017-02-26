using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KOILib.Common.Core;
using KOILib.Common.Core.Extensions;

namespace KOILib.Common.Aspmvc.Helpers
{
    public static class HtmlHelperExtension
    {
        public static IHtmlString Image<TModel>(this HtmlHelper<TModel> self, byte[] byteArray, dynamic attrib, string contentType = null)
        {
            var base64 = Convert.ToBase64String(byteArray);
            var imgsrc = String.Format("data:{0};base64,{1}", contentType ?? "application/image", base64);

            var tb = new TagBuilder("img");
            if (attrib != null) tb.MergeAttributes(Utils.DictionaryFrom(attrib));
            tb.Attributes.Add("src", imgsrc);
            return MvcHtmlString.Create(tb.ToString(TagRenderMode.SelfClosing));
        }
        public static IHtmlString Image<TModel>(this HtmlHelper<TModel> self, byte[] byteArray, string contentType = null)
        {
            return Image(self, byteArray, null, contentType);
        }

        #region ファイルから直接インラインに流し込むヘルパーメソッド
        private static readonly Dictionary<string, string> _inlineRenderCache = new Dictionary<string, string>();
        private static readonly SHA512CryptoServiceProvider _hasher = new SHA512CryptoServiceProvider();

        public static IHtmlString InlineRender<TModel>(this HtmlHelper<TModel> self, IEnumerable<string> virtualPathes)
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
                    var hash = BitConverter.ToString(_hasher.ComputeHash(r.BaseStream));
                    if (!_inlineRenderCache.ContainsKey(hash))
                    {
                        r.BaseStream.Position = 0;
                        _inlineRenderCache.Add(hash, r.ReadToEnd());
                    }
                    wr.Write(_inlineRenderCache[hash]);
                }
            }
        }
        #endregion
    }
}
