using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using KOILib.Common;
using KOILib.Common.Extensions;

namespace KOILib.Common.Aspmvc.Helpers
{
    public static class HtmlHelperExtension
    {
        #region 部分ビューからレイアウトにレンダリングを行うためのヘルパーメソッド
        //http://stackoverflow.com/questions/5433531/using-sections-in-editor-display-templates/5433722#5433722
        public static MvcHtmlString Script(this HtmlHelper htmlHelper, Func<object, HelperResult> template)
        {
            htmlHelper.ViewContext.HttpContext.Items["_script_" + Guid.NewGuid()] = template;
            return MvcHtmlString.Empty;
        }

        public static IHtmlString RenderScripts(this HtmlHelper htmlHelper)
        {
            foreach (object key in htmlHelper.ViewContext.HttpContext.Items.Keys)
            {
                if (key.ToString().StartsWith("_script_"))
                {
                    var template = htmlHelper.ViewContext.HttpContext.Items[key] as Func<object, HelperResult>;
                    if (template != null)
                    {
                        htmlHelper.ViewContext.Writer.Write(template(null));
                    }
                }
            }
            return MvcHtmlString.Empty;
        }
        #endregion
        
        #region dataスキームをソースとしたimgタグ生成
        public static IHtmlString Image<TModel>(this HtmlHelper<TModel> self, byte[] byteArray, dynamic htmlAttributes, string contentType = null)
        {
            var base64 = Convert.ToBase64String(byteArray);
            var imgsrc = String.Format("data:{0};base64,{1}", contentType ?? "application/image", base64);

            var tb = new TagBuilder("img");
            tb.Attributes.Add("src", imgsrc);
            if (htmlAttributes != null)
                tb.MergeAttributes(Utils.ToFlattenDictionary(htmlAttributes, "-"), true);
            return MvcHtmlString.Create(tb.ToString(TagRenderMode.SelfClosing));
        }
        public static IHtmlString Image<TModel>(this HtmlHelper<TModel> self, byte[] byteArray, string contentType = null)
        {
            return Image(self, byteArray, null, contentType);
        }
        #endregion

        #region ファイルから直接インラインに流し込むヘルパーメソッド
        private static readonly Dictionary<string, string> _inlineRenderCache = new Dictionary<string, string>();
        private static readonly SHA512CryptoServiceProvider _hasher = new SHA512CryptoServiceProvider();

        public static IHtmlString InlineRender<TModel>(this HtmlHelper<TModel> self, IEnumerable<string> virtualPathes)
        {
            var sb = new StringBuilder();
            using (var w = new StringWriter(sb))
            {
                virtualPathes.Each((virtualPath) => self.AppendTo(w, virtualPath));
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
