using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KOILib.Common
{
    /// <summary>
    /// コマンドライン引数クラス
    /// </summary>
    public abstract class CmdParameter
    {
        /// <summary>
        /// 引数情報を保持するDictionary
        /// </summary>
        private Dictionary<string, string> mapArgument;

        /// <summary>
        /// 引数情報を取得するメソッド
        /// </summary>
        /// <typeparam name="TEnum">引数名Enum</typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        protected string GetArgument<TEnum>(TEnum name)
            where TEnum : struct
        {
            return this.mapArgument.ContainsKey(name.ToString()) ? this.mapArgument[name.ToString()] : null;
        }

        /// <summary>
        /// コマンドライン引数の情報を読み込みます
        /// </summary>
        public void LoadCommandLine()
        {
            const string argname = "k";
            const string argvalue = "v";

            var pattern = @"(?<" + argname + @">/[\S-:]+):(?<" + argvalue + @">\S+)";

            this.mapArgument = new Dictionary<string, string>();

            var args = Environment.GetCommandLineArgs();
            foreach (var arg in args)
            {
                if (arg == args[0]) continue; //起動パス(%0)は除く

                var match = Regex.Match(arg, pattern, RegexOptions.Compiled);

                if (!match.Success)
                    throw new ArgumentException("The command line arguments are improperly formed. Use /argname:argvalue.");

                var name = match.Groups[argname].Value.Substring(1).ToLower();

                if (this.mapArgument.ContainsKey(name))
                    this.mapArgument.Remove(name);

                this.mapArgument.Add(name, match.Groups[argvalue].Value);
            }
        }
    }
}
