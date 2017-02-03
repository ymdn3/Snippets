using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KOILib.Common.Aspmvc.Helpers
{
    public static class BootstrapHelperExtension
    {
        /// <summary>
        /// Bootstrapのグリフアイコンを表示するspan要素を生成しますが、HTMLエンコードを行いません。
        /// </summary>
        /// <param name="classname"></param>
        /// <param name="aria_hidden"></param>
        /// <returns></returns>
        public static string GlyphString(this BootstrapHelper self, string classname, bool aria_hidden)
        {
            const string g = "glyphicon";
            var tag = new TagBuilder("span");
            tag.AddCssClass(g);
            tag.AddCssClass(classname.StartsWith(g) ? classname
                                                    : g + "-" + classname);
            if (aria_hidden)
                tag.MergeAttribute("aria-hidden", "true");

            return tag.ToString();
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
            return MvcHtmlString.Create(self.GlyphString(classname, aria_hidden));
        }
        /// <summary>
        /// aria-hidden属性にtrue値を指定した、Bootstrapのグリフアイコンを表示するspan要素を生成します。
        /// </summary>
        /// <param name="classname">グリフアイコン名。"glyphicon-"は省略できます。</param>
        /// <returns></returns>
        public static IHtmlString Glyph(this BootstrapHelper self, string classname)
        {
            return MvcHtmlString.Create(self.GlyphString(classname, true));
        }

    }
}
