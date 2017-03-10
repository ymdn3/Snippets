using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KOILib.Common.Extensions;

namespace KOILib.Common.Aspmvc.Models
{
    public class HtmlHiddenInput : IHtmlElement, IHtmlInputElement
    {
        public string AutoFocusAttr { get { return AutoFocus ? "autofocus" : null; } } //HTML5
        string IHtmlInputElement.CheckedAttr { get; }
        public string DisabledAttr { get { return Disabled ? "disabled" : null; } }
        string IHtmlInputElement.MultipleAttr { get; } //HTML5
        string IHtmlInputElement.ReadonlyAttr { get; }
        string IHtmlInputElement.RequiredAttr { get; } //HTML5

        string IHtmlInputElement.Accept { get; set; }
        string IHtmlInputElement.Alt { get; set; }
        string IHtmlInputElement.AutoComplete { get; set; } //HTML5
        public bool AutoFocus { get; set; } //HTML5
        bool IHtmlInputElement.Checked { get; set; }
        public bool Disabled { get; set; }
        public string Form { get; set; } //HTML5
        string IHtmlInputElement.Height { get; set; } //HTML5
        string IHtmlInputElement.List { get; set; } //HTML5
        string IHtmlInputElement.Max { get; set; } //HTML5
        int? IHtmlInputElement.MaxLength { get; set; }
        string IHtmlInputElement.Min { get; set; } //HTML5
        bool IHtmlInputElement.Multiple { get; set; } //HTML5
        public string Name { get; set; }
        string IHtmlInputElement.Pattern { get; set; } //HTML5
        string IHtmlInputElement.Placeholder { get; set; } //HTML5
        bool IHtmlInputElement.Readonly { get; set; }
        bool IHtmlInputElement.Required { get; set; } //HTML5
        int? IHtmlInputElement.Size { get; set; }
        string IHtmlInputElement.Src { get; set; }
        string IHtmlInputElement.Step { get; set; } //HTML5
        public string Type { get; set; }
        string IHtmlInputElement.Width { get; set; } //HTML5

        public string Id { get; set; }
        public string Title { get; set; }
        public string Class { get; set; }
        public dynamic HtmlAttributes { get; set; }

        public HtmlHiddenInput()
        {
            Type = "hidden";
        }
    }
    public class HtmlTextInput : IHtmlElement, IHtmlInputElement
    {
        public string AutoFocusAttr { get { return AutoFocus ? "autofocus" : null; } } //HTML5
        string IHtmlInputElement.CheckedAttr { get; }
        public string DisabledAttr { get { return Disabled ? "disabled" : null; } }
        string IHtmlInputElement.MultipleAttr { get; } //HTML5
        public string ReadonlyAttr { get { return Readonly ? "readonly" : null; } }
        public string RequiredAttr { get { return Required ? "required" : null; } } //HTML5

        string IHtmlInputElement.Accept { get; set; }
        string IHtmlInputElement.Alt { get; set; }
        public string AutoComplete { get; set; } //HTML5
        public bool AutoFocus { get; set; } //HTML5
        bool IHtmlInputElement.Checked { get; set; }
        public bool Disabled { get; set; }
        public string Form { get; set; } //HTML5
        string IHtmlInputElement.Height { get; set; } //HTML5
        public string List { get; set; } //HTML5
        string IHtmlInputElement.Max { get; set; } //HTML5
        public int? MaxLength { get; set; }
        string IHtmlInputElement.Min { get; set; } //HTML5
        bool IHtmlInputElement.Multiple { get; set; } //HTML5
        public string Name { get; set; }
        public string Pattern { get; set; } //HTML5
        public string Placeholder { get; set; } //HTML5
        public bool Readonly { get; set; }
        public bool Required { get; set; } //HTML5
        public int? Size { get; set; }
        string IHtmlInputElement.Src { get; set; }
        string IHtmlInputElement.Step { get; set; } //HTML5
        public string Type { get; set; }
        string IHtmlInputElement.Width { get; set; } //HTML5

        public string Id { get; set; }
        public string Title { get; set; }
        public string Class { get; set; }
        public dynamic HtmlAttributes { get; set; }

        public HtmlTextInput()
        {
            Type = "text";
        }
        public HtmlTextInput(string type)
        {
            if (type.EqualsAny(new[] { "search", "url", "tel" }, StringComparison.OrdinalIgnoreCase))
                Type = type;
        }
    }
    public class HtmlEmailInput : IHtmlElement, IHtmlInputElement
    {
        public string AutoFocusAttr { get { return AutoFocus ? "autofocus" : null; } } //HTML5
        string IHtmlInputElement.CheckedAttr { get; }
        public string DisabledAttr { get { return Disabled ? "disabled" : null; } }
        public string MultipleAttr { get { return Multiple ? "multiple" : null; } } //HTML5
        public string ReadonlyAttr { get { return Readonly ? "readonly" : null; } }
        public string RequiredAttr { get { return Required ? "required" : null; } } //HTML5

        string IHtmlInputElement.Accept { get; set; }
        string IHtmlInputElement.Alt { get; set; }
        public string AutoComplete { get; set; } //HTML5
        public bool AutoFocus { get; set; } //HTML5
        bool IHtmlInputElement.Checked { get; set; }
        public bool Disabled { get; set; }
        public string Form { get; set; } //HTML5
        string IHtmlInputElement.Height { get; set; } //HTML5
        public string List { get; set; } //HTML5
        string IHtmlInputElement.Max { get; set; } //HTML5
        public int? MaxLength { get; set; }
        string IHtmlInputElement.Min { get; set; } //HTML5
        public bool Multiple { get; set; } //HTML5
        public string Name { get; set; }
        public string Pattern { get; set; } //HTML5
        public string Placeholder { get; set; } //HTML5
        public bool Readonly { get; set; }
        public bool Required { get; set; } //HTML5
        public int? Size { get; set; }
        string IHtmlInputElement.Src { get; set; }
        string IHtmlInputElement.Step { get; set; } //HTML5
        public string Type { get; set; }
        string IHtmlInputElement.Width { get; set; } //HTML5

        public string Id { get; set; }
        public string Title { get; set; }
        public string Class { get; set; }
        public dynamic HtmlAttributes { get; set; }

        public HtmlEmailInput()
        {
            Type = "email";
        }
    }
    public class HtmlPasswordInput : IHtmlElement, IHtmlInputElement
    {
        public string AutoFocusAttr { get { return AutoFocus ? "autofocus" : null; } } //HTML5
        string IHtmlInputElement.CheckedAttr { get; }
        public string DisabledAttr { get { return Disabled ? "disabled" : null; } }
        string IHtmlInputElement.MultipleAttr { get; } //HTML5
        public string ReadonlyAttr { get { return Readonly ? "readonly" : null; } }
        public string RequiredAttr { get { return Required ? "required" : null; } } //HTML5

        string IHtmlInputElement.Accept { get; set; }
        string IHtmlInputElement.Alt { get; set; }
        public string AutoComplete { get; set; } //HTML5
        public bool AutoFocus { get; set; } //HTML5
        bool IHtmlInputElement.Checked { get; set; }
        public bool Disabled { get; set; }
        public string Form { get; set; } //HTML5
        string IHtmlInputElement.Height { get; set; } //HTML5
        string IHtmlInputElement.List { get; set; } //HTML5
        string IHtmlInputElement.Max { get; set; } //HTML5
        public int? MaxLength { get; set; }
        string IHtmlInputElement.Min { get; set; } //HTML5
        bool IHtmlInputElement.Multiple { get; set; } //HTML5
        public string Name { get; set; }
        public string Pattern { get; set; } //HTML5
        public string Placeholder { get; set; } //HTML5
        public bool Readonly { get; set; }
        public bool Required { get; set; } //HTML5
        public int? Size { get; set; }
        string IHtmlInputElement.Src { get; set; }
        string IHtmlInputElement.Step { get; set; } //HTML5
        public string Type { get; set; }
        string IHtmlInputElement.Width { get; set; } //HTML5

        public string Id { get; set; }
        public string Title { get; set; }
        public string Class { get; set; }
        public dynamic HtmlAttributes { get; set; }

        public HtmlPasswordInput()
        {
            Type = "password";
        }
    }
    public class HtmlNumberInput : IHtmlElement, IHtmlInputElement
    {
        public string AutoFocusAttr { get { return AutoFocus ? "autofocus" : null; } } //HTML5
        string IHtmlInputElement.CheckedAttr { get; }
        public string DisabledAttr { get { return Disabled ? "disabled" : null; } }
        string IHtmlInputElement.MultipleAttr { get; } //HTML5
        public string ReadonlyAttr { get { return Readonly ? "readonly" : null; } }
        public string RequiredAttr { get { return Required ? "required" : null; } } //HTML5

        string IHtmlInputElement.Accept { get; set; }
        string IHtmlInputElement.Alt { get; set; }
        public string AutoComplete { get; set; } //HTML5
        public bool AutoFocus { get; set; } //HTML5
        bool IHtmlInputElement.Checked { get; set; }
        public bool Disabled { get; set; }
        public string Form { get; set; } //HTML5
        string IHtmlInputElement.Height { get; set; } //HTML5
        public string List { get; set; } //HTML5
        public string Max { get; set; } //HTML5
        int? IHtmlInputElement.MaxLength { get; set; }
        public string Min { get; set; } //HTML5
        bool IHtmlInputElement.Multiple { get; set; } //HTML5
        public string Name { get; set; }
        string IHtmlInputElement.Pattern { get; set; } //HTML5
        string IHtmlInputElement.Placeholder { get; set; } //HTML5
        public bool Readonly { get; set; }
        public bool Required { get; set; } //HTML5
        int? IHtmlInputElement.Size { get; set; }
        string IHtmlInputElement.Src { get; set; }
        public string Step { get; set; } //HTML5
        public string Type { get; set; }
        string IHtmlInputElement.Width { get; set; } //HTML5

        public string Id { get; set; }
        public string Title { get; set; }
        public string Class { get; set; }
        public dynamic HtmlAttributes { get; set; }

        public HtmlNumberInput()
        {
            Type = "number";
        }
        public HtmlNumberInput(string type)
        {
            if (type.EqualsAny(new[] { "datetime", "date", "month", "week", "time", "datetime-local" }, StringComparison.OrdinalIgnoreCase))
                Type = type;
        }
    }
    public class HtmlRangeInput : IHtmlElement, IHtmlInputElement
    {
        public string AutoFocusAttr { get { return AutoFocus ? "autofocus" : null; } } //HTML5
        string IHtmlInputElement.CheckedAttr { get; }
        public string DisabledAttr { get { return Disabled ? "disabled" : null; } }
        string IHtmlInputElement.MultipleAttr { get; } //HTML5
        string IHtmlInputElement.ReadonlyAttr { get; }
        string IHtmlInputElement.RequiredAttr { get; } //HTML5

        string IHtmlInputElement.Accept { get; set; }
        string IHtmlInputElement.Alt { get; set; }
        public string AutoComplete { get; set; } //HTML5
        public bool AutoFocus { get; set; } //HTML5
        bool IHtmlInputElement.Checked { get; set; }
        public bool Disabled { get; set; }
        public string Form { get; set; } //HTML5
        string IHtmlInputElement.Height { get; set; } //HTML5
        public string List { get; set; } //HTML5
        public string Max { get; set; } //HTML5
        int? IHtmlInputElement.MaxLength { get; set; }
        public string Min { get; set; } //HTML5
        bool IHtmlInputElement.Multiple { get; set; } //HTML5
        public string Name { get; set; }
        string IHtmlInputElement.Pattern { get; set; } //HTML5
        string IHtmlInputElement.Placeholder { get; set; } //HTML5
        bool IHtmlInputElement.Readonly { get; set; }
        bool IHtmlInputElement.Required { get; set; } //HTML5
        int? IHtmlInputElement.Size { get; set; }
        string IHtmlInputElement.Src { get; set; }
        public string Step { get; set; } //HTML5
        public string Type { get; set; }
        string IHtmlInputElement.Width { get; set; } //HTML5

        public string Id { get; set; }
        public string Title { get; set; }
        public string Class { get; set; }
        public dynamic HtmlAttributes { get; set; }

        public HtmlRangeInput()
        {
            Type = "range";
        }
    }
    public class HtmlColorInput : IHtmlElement, IHtmlInputElement
    {
        public string AutoFocusAttr { get { return AutoFocus ? "autofocus" : null; } } //HTML5
        string IHtmlInputElement.CheckedAttr { get; }
        public string DisabledAttr { get { return Disabled ? "disabled" : null; } }
        string IHtmlInputElement.MultipleAttr { get; } //HTML5
        string IHtmlInputElement.ReadonlyAttr { get; }
        string IHtmlInputElement.RequiredAttr { get; } //HTML5

        string IHtmlInputElement.Accept { get; set; }
        string IHtmlInputElement.Alt { get; set; }
        public string AutoComplete { get; set; } //HTML5
        public bool AutoFocus { get; set; } //HTML5
        bool IHtmlInputElement.Checked { get; set; }
        public bool Disabled { get; set; }
        public string Form { get; set; } //HTML5
        string IHtmlInputElement.Height { get; set; } //HTML5
        public string List { get; set; } //HTML5
        string IHtmlInputElement.Max { get; set; } //HTML5
        int? IHtmlInputElement.MaxLength { get; set; }
        string IHtmlInputElement.Min { get; set; } //HTML5
        bool IHtmlInputElement.Multiple { get; set; } //HTML5
        public string Name { get; set; }
        string IHtmlInputElement.Pattern { get; set; } //HTML5
        string IHtmlInputElement.Placeholder { get; set; } //HTML5
        bool IHtmlInputElement.Readonly { get; set; }
        bool IHtmlInputElement.Required { get; set; } //HTML5
        int? IHtmlInputElement.Size { get; set; }
        string IHtmlInputElement.Src { get; set; }
        string IHtmlInputElement.Step { get; set; } //HTML5
        public string Type { get; set; }
        string IHtmlInputElement.Width { get; set; } //HTML5

        public string Id { get; set; }
        public string Title { get; set; }
        public string Class { get; set; }
        public dynamic HtmlAttributes { get; set; }

        public HtmlColorInput()
        {
            Type = "color";
        }
    }
    public class HtmlCheckboxInput : IHtmlElement, IHtmlInputElement
    {
        public string AutoFocusAttr { get { return AutoFocus ? "autofocus" : null; } } //HTML5
        public string CheckedAttr { get { return Checked ? "checked" : null; } }
        public string DisabledAttr { get { return Disabled ? "disabled" : null; } }
        string IHtmlInputElement.MultipleAttr { get; } //HTML5
        string IHtmlInputElement.ReadonlyAttr { get; }
        public string RequiredAttr { get { return Required ? "required" : null; } } //HTML5

        string IHtmlInputElement.Accept { get; set; }
        string IHtmlInputElement.Alt { get; set; }
        string IHtmlInputElement.AutoComplete { get; set; } //HTML5
        public bool AutoFocus { get; set; } //HTML5
        public bool Checked { get; set; }
        public bool Disabled { get; set; }
        public string Form { get; set; } //HTML5
        string IHtmlInputElement.Height { get; set; } //HTML5
        string IHtmlInputElement.List { get; set; } //HTML5
        string IHtmlInputElement.Max { get; set; } //HTML5
        int? IHtmlInputElement.MaxLength { get; set; }
        string IHtmlInputElement.Min { get; set; } //HTML5
        bool IHtmlInputElement.Multiple { get; set; } //HTML5
        public string Name { get; set; }
        string IHtmlInputElement.Pattern { get; set; } //HTML5
        string IHtmlInputElement.Placeholder { get; set; } //HTML5
        bool IHtmlInputElement.Readonly { get; set; }
        public bool Required { get; set; } //HTML5
        int? IHtmlInputElement.Size { get; set; }
        string IHtmlInputElement.Src { get; set; }
        string IHtmlInputElement.Step { get; set; } //HTML5
        public string Type { get; set; }
        string IHtmlInputElement.Width { get; set; } //HTML5

        public string Id { get; set; }
        public string Title { get; set; }
        public string Class { get; set; }
        public dynamic HtmlAttributes { get; set; }

        public HtmlCheckboxInput()
        {
            Type = "checkbox";
        }
    }
    public class HtmlRadioInput : IHtmlElement, IHtmlInputElement
    {
        public string AutoFocusAttr { get { return AutoFocus ? "autofocus" : null; } } //HTML5
        public string CheckedAttr { get { return Checked ? "checked" : null; } }
        public string DisabledAttr { get { return Disabled ? "disabled" : null; } }
        string IHtmlInputElement.MultipleAttr { get; } //HTML5
        string IHtmlInputElement.ReadonlyAttr { get; }
        public string RequiredAttr { get { return Required ? "required" : null; } } //HTML5

        string IHtmlInputElement.Accept { get; set; }
        string IHtmlInputElement.Alt { get; set; }
        string IHtmlInputElement.AutoComplete { get; set; } //HTML5
        public bool AutoFocus { get; set; } //HTML5
        public bool Checked { get; set; }
        public bool Disabled { get; set; }
        public string Form { get; set; } //HTML5
        string IHtmlInputElement.Height { get; set; } //HTML5
        string IHtmlInputElement.List { get; set; } //HTML5
        string IHtmlInputElement.Max { get; set; } //HTML5
        int? IHtmlInputElement.MaxLength { get; set; }
        string IHtmlInputElement.Min { get; set; } //HTML5
        bool IHtmlInputElement.Multiple { get; set; } //HTML5
        public string Name { get; set; }
        string IHtmlInputElement.Pattern { get; set; } //HTML5
        string IHtmlInputElement.Placeholder { get; set; } //HTML5
        bool IHtmlInputElement.Readonly { get; set; }
        public bool Required { get; set; } //HTML5
        int? IHtmlInputElement.Size { get; set; }
        string IHtmlInputElement.Src { get; set; }
        string IHtmlInputElement.Step { get; set; } //HTML5
        public string Type { get; set; }
        string IHtmlInputElement.Width { get; set; } //HTML5

        public string Id { get; set; }
        public string Title { get; set; }
        public string Class { get; set; }
        public dynamic HtmlAttributes { get; set; }

        public HtmlRadioInput()
        {
            Type = "radio";
        }
    }
    public class HtmlFileInput : IHtmlElement, IHtmlInputElement
    {
        public string AutoFocusAttr { get { return AutoFocus ? "autofocus" : null; } } //HTML5
        string IHtmlInputElement.CheckedAttr { get; }
        public string DisabledAttr { get { return Disabled ? "disabled" : null; } }
        public string MultipleAttr { get { return Multiple ? "multiple" : null; } } //HTML5
        string IHtmlInputElement.ReadonlyAttr { get; }
        public string RequiredAttr { get { return Required ? "required" : null; } } //HTML5

        public string Accept { get; set; }
        string IHtmlInputElement.Alt { get; set; }
        string IHtmlInputElement.AutoComplete { get; set; } //HTML5
        public bool AutoFocus { get; set; } //HTML5
        bool IHtmlInputElement.Checked { get; set; }
        public bool Disabled { get; set; }
        public string Form { get; set; } //HTML5
        string IHtmlInputElement.Height { get; set; } //HTML5
        string IHtmlInputElement.List { get; set; } //HTML5
        string IHtmlInputElement.Max { get; set; } //HTML5
        int? IHtmlInputElement.MaxLength { get; set; }
        string IHtmlInputElement.Min { get; set; } //HTML5
        public bool Multiple { get; set; } //HTML5
        public string Name { get; set; }
        string IHtmlInputElement.Pattern { get; set; } //HTML5
        string IHtmlInputElement.Placeholder { get; set; } //HTML5
        bool IHtmlInputElement.Readonly { get; set; }
        public bool Required { get; set; } //HTML5
        int? IHtmlInputElement.Size { get; set; }
        string IHtmlInputElement.Src { get; set; }
        string IHtmlInputElement.Step { get; set; } //HTML5
        public string Type { get; set; }
        string IHtmlInputElement.Width { get; set; } //HTML5

        public string Id { get; set; }
        public string Title { get; set; }
        public string Class { get; set; }
        public dynamic HtmlAttributes { get; set; }

        public HtmlFileInput()
        {
            Type = "file";
        }
    }
    public class HtmlImageInput : IHtmlElement, IHtmlInputElement
    {
        public string AutoFocusAttr { get { return AutoFocus ? "autofocus" : null; } } //HTML5
        string IHtmlInputElement.CheckedAttr { get; }
        public string DisabledAttr { get { return Disabled ? "disabled" : null; } }
        string IHtmlInputElement.MultipleAttr { get; } //HTML5
        string IHtmlInputElement.ReadonlyAttr { get; }
        string IHtmlInputElement.RequiredAttr { get; } //HTML5

        string IHtmlInputElement.Accept { get; set; }
        public string Alt { get; set; }
        string IHtmlInputElement.AutoComplete { get; set; } //HTML5
        public bool AutoFocus { get; set; } //HTML5
        bool IHtmlInputElement.Checked { get; set; }
        public bool Disabled { get; set; }
        public string Form { get; set; } //HTML5
        public string Height { get; set; } //HTML5
        string IHtmlInputElement.List { get; set; } //HTML5
        string IHtmlInputElement.Max { get; set; } //HTML5
        int? IHtmlInputElement.MaxLength { get; set; }
        string IHtmlInputElement.Min { get; set; } //HTML5
        bool IHtmlInputElement.Multiple { get; set; } //HTML5
        public string Name { get; set; }
        string IHtmlInputElement.Pattern { get; set; } //HTML5
        string IHtmlInputElement.Placeholder { get; set; } //HTML5
        bool IHtmlInputElement.Readonly { get; set; }
        bool IHtmlInputElement.Required { get; set; } //HTML5
        int? IHtmlInputElement.Size { get; set; }
        public string Src { get; set; }
        string IHtmlInputElement.Step { get; set; } //HTML5
        public string Type { get; set; }
        public string Width { get; set; } //HTML5

        public string Id { get; set; }
        public string Title { get; set; }
        public string Class { get; set; }
        public dynamic HtmlAttributes { get; set; }

        public HtmlImageInput()
        {
            Type = "image";
        }
    }

}
