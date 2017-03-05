using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Aspmvc.Models
{
    /// <summary>
    /// HTMLラジオボタン項目モデル
    /// </summary>
    public class RadioItem
    {
        public static readonly RadioItem Empty = new RadioItem();

        public string Text { get; set; }
        public string Value { get; set; }

        public RadioItem()
        {
            Text = "";
            Value = "";
        }
    }
}
