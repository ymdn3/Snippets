using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Aspmvc.Models
{
    public interface IHtmlElement
    {
        string Id { get; set; }
        string Title { get; set; }
        string Class { get; set; }
        dynamic HtmlAttributes { get; set; }
    }
}
