using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace KOILib.Common.Log4
{
    /// <summary>
    /// ログ出力規定クラス
    /// </summary>
    public abstract class LoggerBase
    {
        #region Static Members
        /// <summary>
        /// ログ出力ファイルパスを設定する環境変数名。
        /// log4net設定ファイル・出力ファイルパスにて、${LOG4NET_LOGFILE_PATH} と記述すると、この環境変数の値を参照することができる。
        /// </summary>
        protected static string LOG4NET_LOGFILE_PATH = "LOG4NET_LOGFILE_PATH";

        /// <summary>
        /// Loggerクラスインスタンス
        /// </summary>
        protected static LoggerBase _Instance { get; set; }

        /// <summary>
        /// log4netを指定のファイルで初期化します
        /// </summary>
        /// <param name="confFilepath">log4net設定ファイルのパス</param>
        protected static void Initialize(string confFilepath)
        {
            //log4net初期化
            Initialize(confFilepath, null);
        }
        /// <summary>
        /// log4netを指定のファイルで初期化します
        /// </summary>
        /// <param name="confFilepath">log4net設定ファイルのパス</param>
        /// <param name="logFilepath">出力先ログファイルのパス(フォルダ階層も含めて定義できます)</param>
        protected static void Initialize(string confFilepath, string logFilepath)
        {
            //ログファイル出力先をプログラム側から指定するための設定
            if (!string.IsNullOrEmpty(logFilepath))
                Environment.SetEnvironmentVariable(LOG4NET_LOGFILE_PATH, logFilepath);

            //log4net初期化
            var fi = new System.IO.FileInfo(confFilepath);
            log4net.Config.XmlConfigurator.Configure(fi);

            //パターン「%property{pid}」でプロセスIDを出力できる設定
            GlobalContext.Properties["pid"] = Process.GetCurrentProcess().Id;

            //Loggerクラスインスタンスの生成が継承先で必要です
        }
        #endregion

        /// <summary>
        /// スタックトレース監視対象とするクラス名（Fullname、前方一致）
        /// </summary>
        protected abstract string[] InvokerDeclarations { get; }

        /// <summary>
        /// ILog実装
        /// </summary>
        protected ILog _LogImpl;

        /// <summary>
        /// Fatalログ出力
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public void Fatal(Exception ex)
        {
            var method = SearchInvoker(ex);
            Fatal(method, ex);
        }

        /// <summary>
        /// Fatalログ出力
        /// </summary>
        /// <param name="method">System.Reflection.MethodBase.GetCurrentMethod()</param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public virtual void Fatal(MethodBase method, Exception ex)
        {
            Write(LogLevel.Fatal, method, ex);
        }

        /// <summary>
        /// Errorログ出力
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public void Error(Exception ex)
        {
            var method = SearchInvoker(ex);
            Error(method, ex);
        }

        /// <summary>
        /// Errorログ出力
        /// </summary>
        /// <param name="method">System.Reflection.MethodBase.GetCurrentMethod()</param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public virtual void Error(MethodBase method, Exception ex)
        {
            Write(LogLevel.Error, method, ex);
        }

        /// <summary>
        /// Warnログ出力
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public void Warn(Exception ex)
        {
            var method = SearchInvoker(ex);
            Warn(method, ex);
        }

        /// <summary>
        /// Warnログ出力
        /// </summary>
        /// <param name="method">System.Reflection.MethodBase.GetCurrentMethod()</param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public virtual void Warn(MethodBase method, Exception ex)
        {
            Write(LogLevel.Warn, method, ex);
        }

        /// <summary>
        /// Warnログ出力
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public void Warn(string msg, params object[] args)
        {
            var method = SearchInvoker();
            if (args.Length > 0)
                Warn(method, msg, args);
            else
                Warn(method, msg);
        }

        /// <summary>
        /// Warnログ出力
        /// </summary>
        /// <param name="method">System.Reflection.MethodBase.GetCurrentMethod()</param>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual void Warn(MethodBase method, string msg, params object[] args)
        {
            if (args.Length > 0)
                Write(LogLevel.Warn, method, msg, args);
            else
                Write(LogLevel.Warn, method, msg);
        }

        /// <summary>
        /// Infoログ出力
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public void Info(string msg, params object[] args)
        {
            var method = SearchInvoker();
            if (args.Length > 0)
                Info(method, msg, args);
            else
                Info(method, msg);
        }

        /// <summary>
        /// Infoログ出力
        /// </summary>
        /// <param name="method">System.Reflection.MethodBase.GetCurrentMethod()</param>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual void Info(MethodBase method, string msg, params object[] args)
        {
            if (args.Length > 0)
                Write(LogLevel.Info, method, msg, args);
            else
                Write(LogLevel.Info, method, msg);
        }

        /// <summary>
        /// Debugログ出力
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public void Debug(string msg, params object[] args)
        {
            var method = SearchInvoker();
            if (args.Length > 0)
                Debug(method, msg, args);
            else
                Debug(method, msg);
        }

        /// <summary>
        /// Debugログ出力
        /// </summary>
        /// <param name="method">System.Reflection.MethodBase.GetCurrentMethod()</param>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual void Debug(MethodBase method, string msg, params object[] args)
        {
            if (args.Length > 0)
                Write(LogLevel.Debug, method, msg, args);
            else
                Write(LogLevel.Debug, method, msg);
        }

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="level"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public void Write(LogLevel level, Exception ex)
        {
            var method = SearchInvoker(ex);
            Write(level, method, ex);
        }

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="level"></param>
        /// <param name="method">System.Reflection.MethodBase.GetCurrentMethod()</param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public virtual void Write(LogLevel level, MethodBase method, Exception ex)
        {
            var t = BuildMessage(method, ex.Message);
            WriteByCase(level, t, ex);
        }

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="level"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public void Write(LogLevel level, string format, params object[] args)
        {
            var method = SearchInvoker();
            if (args.Length > 0)
                Write(level, method, format, args);
            else
                Write(level, method, format);
        }

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="level"></param>
        /// <param name="method">System.Reflection.MethodBase.GetCurrentMethod()</param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual void Write(LogLevel level, MethodBase method, string format, params object[] args)
        {
            var t = BuildMessage(method, format);
            if (args.Length > 0)
                WriteByCase(level, t, args);
            else
                WriteByCase(level, t, default(Exception));
        }

        /// <summary>
        /// スタックトレースを総なめし、クラス名が InvokerDeclarations に前方一致する最初のメソッドを返す。
        /// 呼び元を指定しない場合に使用する。
        /// </summary>
        /// <returns></returns>
        protected MethodBase SearchInvoker()
        {
            return SearchInvoker(null);
        }
        protected MethodBase SearchInvoker(Exception ex)
        {
            var trace = ex == null 
                ? new StackTrace(fNeedFileInfo: true) 
                : new StackTrace(ex, fNeedFileInfo: true);
            if (trace == null)
                return default(MethodBase);

            var logMethod = trace.GetFrames()
                .Select(frame => frame.GetMethod())
                .Where(method => method.DeclaringType != null)
                .Where(method => InvokerDeclarations
                    .Any(w => method.DeclaringType.FullName.StartsWith(w, StringComparison.OrdinalIgnoreCase)))
                .FirstOrDefault();

            return logMethod;
        }

        /// <summary>
        /// 「[クラス名.メソッド名] - メッセージ」文字列構築
        /// </summary>
        /// <param name="method"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected string BuildMessage(MethodBase method, string msg)
        {
            // クラスまたはメソッドが不明な場合の表記
            const string UNKNOWN = "(unknown)";
            var cname = (method == null ? UNKNOWN : method.DeclaringType == null ? UNKNOWN : method.DeclaringType.Name);
            var mname = (method == null ? UNKNOWN : method.Name);
            return string.Format("[{0}.{1}] - {2}", cname, mname, msg);
        }

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="level"></param>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        private void WriteByCase(LogLevel level, string msg, Exception ex)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    if (ex == null)
                        _LogImpl.Debug(msg);
                    else
                        _LogImpl.Debug(msg, ex);
                    break;
                case LogLevel.Info:
                    if (ex == null)
                        _LogImpl.Info(msg);
                    else
                        _LogImpl.Info(msg, ex);
                    break;
                case LogLevel.Warn:
                    if (ex == null)
                        _LogImpl.Warn(msg);
                    else
                        _LogImpl.Warn(msg, ex);
                    break;
                case LogLevel.Error:
                    if (ex == null)
                        _LogImpl.Error(msg);
                    else
                        _LogImpl.Error(msg, ex);
                    break;
                case LogLevel.Fatal:
                    if (ex == null)
                        _LogImpl.Fatal(msg);
                    else
                        _LogImpl.Fatal(msg, ex);
                    break;
            }
        }

        /// <summary>
        /// ログ出力
        /// （～Format()版）
        /// </summary>
        /// <param name="level"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private void WriteByCase(LogLevel level, string format, object[] args)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    _LogImpl.DebugFormat(format, args);
                    break;
                case LogLevel.Info:
                    _LogImpl.InfoFormat(format, args);
                    break;
                case LogLevel.Warn:
                    _LogImpl.WarnFormat(format, args);
                    break;
                case LogLevel.Error:
                    _LogImpl.ErrorFormat(format, args);
                    break;
                case LogLevel.Fatal:
                    _LogImpl.FatalFormat(format, args);
                    break;
            }
        }

        /// <summary>
        /// 非公開
        /// (継承は可能とする)
        /// </summary>
        protected LoggerBase(string name)
        {
            //ILog実装を生成
            _LogImpl = LogManager.GetLogger(name);
        }
    }
}
