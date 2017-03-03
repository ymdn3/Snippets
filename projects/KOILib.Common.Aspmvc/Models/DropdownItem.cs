using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Aspmvc.Models
{
    /// <summary>
    /// HTMLドロップダウンリスト項目モデル
    /// </summary>
    public class DropdownItem
    {
        public static readonly DropdownItem Empty = new DropdownItem();

        public string Text { get; set; }
        public string Value { get; set; }

        public DropdownItem()
        {
            Text = "";
            Value = "";
        }
    }
}
