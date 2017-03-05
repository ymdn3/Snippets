using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Aspmvc.Models
{
    public abstract class ComplexValueItem
    {
        public virtual string Text { get; set; }
        public virtual string Value { get; set; }

        public ComplexValueItem()
        {
            Text = "";
            Value = "";
        }
    }
}
