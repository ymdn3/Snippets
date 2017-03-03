using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Aspmvc.Models
{
    public interface IHtmlInputElement
    {
        string Accept { get; set; }
        string Alt { get; set; }
        string AutoComplete { get; set; } //HTML5
        bool AutoFocus { get; set; } //HTML5
        bool Checked { get; set; }
        bool Disabled { get; set; }
        string Form { get; set; } //HTML5
        string Height { get; set; } //HTML5
        string List { get; set; } //HTML5
        string Max { get; set; } //HTML5
        int? MaxLength { get; set; }
        string Min { get; set; } //HTML5
        bool Multiple { get; set; } //HTML5
        string Name { get; set; }
        string Pattern { get; set; } //HTML5
        string Placeholder { get; set; } //HTML5
        bool Readonly { get; set; }
        bool Required { get; set; } //HTML5
        int? Size { get; set; }
        string Src { get; set; }
        string Step { get; set; } //HTML5
        string Type { get; set; }
        string Width { get; set; } //HTML5

        string AutoFocusAttr { get; } //HTML5
        string CheckedAttr { get; }
        string DisabledAttr { get; }
        string MultipleAttr { get; } //HTML5
        string ReadonlyAttr { get; }
        string RequiredAttr { get; } //HTML5
    }
}
