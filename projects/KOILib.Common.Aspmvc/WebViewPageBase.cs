using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using KOILib.Common.Aspmvc.Helpers;

namespace KOILib.Common.Aspmvc
{
    public abstract class WebViewPageBase<TModel> : WebViewPage<TModel>
    {
        /// <summary>
        /// Bootstrap ヘルパー
        /// </summary>
        public BootstrapHelper<TModel> Bootstrap { get; private set; }

        /// <summary>
        /// Newtonsoft.Json ヘルパー
        /// </summary>
        public JsonHelper<TModel> Json { get; private set; }

        public override void InitHelpers()
        {
            base.InitHelpers();
            Bootstrap = new BootstrapHelper<TModel>(ViewContext, this);
            Json = new JsonHelper<TModel>(ViewContext, this);
        }
    }
}
