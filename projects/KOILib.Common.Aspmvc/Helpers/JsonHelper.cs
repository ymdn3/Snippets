using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace KOILib.Common.Aspmvc.Helpers
{
    public class JsonHelper<TModel> : JsonHelper
    {
        public new ViewDataDictionary<TModel> ViewData { get; private set; }

        private DynamicViewDataDictionary _dynamicViewDataDictionary;
        public new dynamic ViewBag
        {
            get
            {
                if (_dynamicViewDataDictionary == null)
                    _dynamicViewDataDictionary = new DynamicViewDataDictionary(() => ViewData);
                return _dynamicViewDataDictionary;
            }
        }

        public JsonHelper(ViewContext viewContext, IViewDataContainer viewDataContainer)
            : this(viewContext, viewDataContainer, RouteTable.Routes)
        {
        }

        public JsonHelper(ViewContext viewContext, IViewDataContainer viewDataContainer, RouteCollection routeCollection)
            : base(viewContext, viewDataContainer, routeCollection)
        {
            ViewData = new ViewDataDictionary<TModel>(viewDataContainer.ViewData);
        }
    }

    public class JsonHelper : HelperBase
    {
        private DynamicViewDataDictionary _dynamicViewDataDictionary;
        public new dynamic ViewBag
        {
            get
            {
                if (_dynamicViewDataDictionary == null)
                    _dynamicViewDataDictionary = new DynamicViewDataDictionary(() => ViewData);
                return _dynamicViewDataDictionary;
            }
        }

        public JsonHelper(ViewContext viewContext, IViewDataContainer viewDataContainer)
            : this(viewContext, viewDataContainer, RouteTable.Routes)
        {
        }

        public JsonHelper(ViewContext viewContext, IViewDataContainer viewDataContainer, RouteCollection routeCollection)
            : base(viewContext, viewDataContainer, routeCollection)
        {
        }
    }
}
