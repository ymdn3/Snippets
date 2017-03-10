using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using KOILib.Common;

namespace KOILib.Common.Aspmvc.Filters
{
    /// <summary>
    /// 特定のセッション値をアクションメソッド実行時にパラメータとして含めるフィルター
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    [Obsolete("**marking**")] //TODO おそらく使用しないクラスのはず
    public class ParameterFromSessionAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// セッション値の型。Serializableであり、かつデフォルトコンストラクタを持つこと。
        /// </summary>
        public Type ModelType { get; private set; }

        /// <summary>
        /// アクションメソッドの引数名。実行時にセッション値がセットされます。
        /// </summary>
        public string Name { get; private set; }

        private string _sessionKey;
        /// <summary>
        /// 格納先のセッションキー。
        /// nullのときModelTypeの型名を返します
        /// </summary>
        public string SessionKey
        {
            get { return _sessionKey ?? ModelType.FullName; }
            set { _sessionKey = value; }
        }

        /// <summary>
        /// アクションメソッドの実行後にセッションの値を破棄するかどうか
        /// </summary>
        public bool ToDestroy { get; set; }

        /// <summary>
        /// アクションメソッド実行前
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Name == null)
                return;

            if (ModelType == null)
                ModelType = typeof(object);

            //セッション値の取得
            var @object = filterContext.HttpContext.Session[SessionKey];
            
            //アクションメソッドの引数にセッション値をセット
            filterContext.ActionParameters[Name] = @object;

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// アクションメソッド実行後
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //セッション値の破棄
            if (ToDestroy)
                filterContext.HttpContext.Session.Remove(SessionKey);

            base.OnActionExecuted(filterContext);
        }

        public ParameterFromSessionAttribute(Type modelType, string parameterName)
        {
            ModelType = modelType;
            Name = parameterName;
        }
    }
}
