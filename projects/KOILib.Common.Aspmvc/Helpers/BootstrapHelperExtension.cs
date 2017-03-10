using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KOILib.Common.Extensions;

namespace KOILib.Common.Aspmvc.Helpers
{
    public static class BootstrapHelperExtension
    {
        /// <summary>
        /// Bootstrapのグリフアイコンを表示するspan要素タグを生成します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="classname"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static TagBuilder GlyphTag(this BootstrapHelper self, string classname, dynamic htmlAttributes)
        {
            const string g = "glyphicon";

            var tb = new TagBuilder("span");
            tb.AddCssClass(g);
            tb.AddCssClass(classname.StartsWith(g) ? classname
                                                    : g + "-" + classname);
            if (htmlAttributes != null)
                tb.MergeAttributes(((object)htmlAttributes).ToFlattenDictionary("-"), true);
            return tb;
        }
        /// <summary>
        /// Bootstrapのグリフアイコンを表示するspan要素タグを生成します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="classname"></param>
        /// <returns></returns>
        public static TagBuilder GlyphTag(this BootstrapHelper self, string classname)
        {
            return GlyphTag(self, classname, null);
        }
        /// <summary>
        /// Bootstrapのグリフアイコンを表示するspan要素を生成します。
        /// </summary>
        /// <param name="classname">グリフアイコン名。"glyphicon-"は省略できます。</param>
        /// <param name="aria_hidden">aria-hidden属性の有無。
        /// たとえば重複する内容が記述された場合やデザイン目的で配置されたHTML要素に対してはtrueを指定します</param>
        /// <param name="htmlAttributes">HTTP要素の属性</param>
        /// <returns></returns>
        public static IHtmlString Glyph(this BootstrapHelper self, string classname, bool aria_hidden, dynamic htmlAttributes)
        {
            TagBuilder tb = GlyphTag(self, classname, htmlAttributes);

            if (aria_hidden)
                tb.MergeAttribute("aria-hidden", "true");

            return MvcHtmlString.Create(tb.ToString());
        }
        /// <summary>
        /// Bootstrapのグリフアイコンを表示するspan要素を生成します。
        /// </summary>
        /// <param name="classname">グリフアイコン名。"glyphicon-"は省略できます。</param>
        /// <param name="htmlAttributes">HTTP要素の属性</param>
        /// <returns></returns>
        public static IHtmlString Glyph(this BootstrapHelper self, string classname, dynamic htmlAttributes)
        {
            return Glyph(self, classname, true, htmlAttributes);
        }
        /// <summary>
        /// Bootstrapのグリフアイコンを表示するspan要素を生成します。
        /// </summary>
        /// <param name="classname">グリフアイコン名。"glyphicon-"は省略できます。</param>
        /// <param name="aria_hidden">aria-hidden属性の有無。
        /// たとえば重複する内容が記述された場合やデザイン目的で配置されたHTML要素に対してはtrueを指定します</param>
        /// <returns></returns>
        public static IHtmlString Glyph(this BootstrapHelper self, string classname, bool aria_hidden)
        {
            return Glyph(self, classname, aria_hidden, null);
        }
        /// <summary>
        /// aria-hidden属性にtrue値を指定した、Bootstrapのグリフアイコンを表示するspan要素を生成します。
        /// </summary>
        /// <param name="classname">グリフアイコン名。"glyphicon-"は省略できます。</param>
        /// <returns></returns>
        public static IHtmlString Glyph(this BootstrapHelper self, string classname)
        {
            return Glyph(self, classname, true, null);
        }

    }
}
