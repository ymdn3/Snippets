using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common.Aspmvc.Models
{
    public class EvalstringBag : DynamicObject
    {
        private const string eval = "eval()||";
        private Dictionary<string, string> _bag = new Dictionary<string, string>();

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var v = default(string);
            result = _bag.TryGetValue(binder.Name, out v) ? eval + v : v;
            return true;
            //return base.TryGetMember(binder, out result);
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _bag.Remove(binder.Name);
            if (value != null)
                _bag[binder.Name] = (string)value;
            return true;
            //return base.TrySetMember(binder, value);
        }

    }
}
