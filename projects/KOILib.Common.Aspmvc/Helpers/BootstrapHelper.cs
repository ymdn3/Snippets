using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace KOILib.Common.Aspmvc.Helpers
{
    public class BootstrapHelper<TModel> : BootstrapHelper
    {
        private DynamicViewDataDictionary _dynamicViewDataDictionary;

        public BootstrapHelper(ViewContext viewContext, IViewDataContainer viewDataContainer)
            : this(viewContext, viewDataContainer, RouteTable.Routes)
        {
        }

        public BootstrapHelper(ViewContext viewContext, IViewDataContainer viewDataContainer, RouteCollection routeCollection)
            : base(viewContext, viewDataContainer, routeCollection)
        {
            ViewData = new ViewDataDictionary<TModel>(viewDataContainer.ViewData);
        }

        public new dynamic ViewBag
        {
            get
            {
                if (_dynamicViewDataDictionary == null)
                {
                    _dynamicViewDataDictionary = new DynamicViewDataDictionary(() => ViewData);
                }

                return _dynamicViewDataDictionary;
            }
        }

        public new ViewDataDictionary<TModel> ViewData { get; private set; }

    }

    public class BootstrapHelper
    {
        private DynamicViewDataDictionary _dynamicViewDataDictionary;

        public BootstrapHelper(ViewContext viewContext, IViewDataContainer viewDataContainer)
            : this(viewContext, viewDataContainer, RouteTable.Routes)
        {
        }

        public BootstrapHelper(ViewContext viewContext, IViewDataContainer viewDataContainer, RouteCollection routeCollection)
        {
            if (viewContext == null)
            {
                throw new ArgumentNullException("viewContext");
            }
            if (viewDataContainer == null)
            {
                throw new ArgumentNullException("viewDataContainer");
            }
            if (routeCollection == null)
            {
                throw new ArgumentNullException("routeCollection");
            }

            ViewContext = viewContext;
            ViewData = new ViewDataDictionary(viewDataContainer.ViewData);
            RouteCollection = routeCollection;
        }

        public RouteCollection RouteCollection { get; private set; }

        public dynamic ViewBag
        {
            get
            {
                if (_dynamicViewDataDictionary == null)
                {
                    _dynamicViewDataDictionary = new DynamicViewDataDictionary(() => ViewData);
                }
                return _dynamicViewDataDictionary;
            }
        }

        public ViewContext ViewContext { get; private set; }

        public ViewDataDictionary ViewData { get; private set; }

    }
}
