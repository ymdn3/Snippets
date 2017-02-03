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
        public BootstrapHelper<TModel> Bootstrap { get; set; }

        public override void InitHelpers()
        {
            base.InitHelpers();
            Bootstrap = new BootstrapHelper<TModel>(ViewContext, this);
        }

    }
}
