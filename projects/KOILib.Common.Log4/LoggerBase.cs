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
        public Task Fatal(Exception ex)
        {
            var method = SearchInvoker();
            return Fatal(method, ex);
        }

        /// <summary>
        /// Fatalログ出力
        /// </summary>
        /// <param name="method">System.Reflection.MethodBase.GetCurrentMethod()</param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public virtual Task Fatal(MethodBase method, Exception ex)
        {
            return Write(LogLevel.Fatal, method, ex);
        }

        /// <summary>
        /// Errorログ出力
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public Task Error(Exception ex)
        {
            var method = SearchInvoker();
            return Error(method, ex);
        }

        /// <summary>
        /// Errorログ出力
        /// </summary>
        /// <param name="method">System.Reflection.MethodBase.GetCurrentMethod()</param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public virtual Task Error(MethodBase method, Exception ex)
        {
            return Write(LogLevel.Error, method, ex);
        }

        /// <summary>
        /// Warnログ出力
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public Task Warn(Exception ex)
        {
            var method = SearchInvoker();
            return Warn(method, ex);
        }

        /// <summary>
        /// Warnログ出力
        /// </summary>
        /// <param name="method">System.Reflection.MethodBase.GetCurrentMethod()</param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public virtual Task Warn(MethodBase method, Exception ex)
        {
            return Write(LogLevel.Warn, method, ex);
        }

        /// <summary>
        /// Warnログ出力
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task Warn(string msg, params object[] args)
        {
            var method = SearchInvoker();
            if (args.Length > 0)
                return Warn(method, msg, args);
            else
                return Warn(method, msg);
        }

        /// <summary>
        /// Warnログ出力
        /// </summary>
        /// <param name="method">System.Reflection.MethodBase.GetCurrentMethod()</param>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual Task Warn(MethodBase method, string msg, params object[] args)
        {
            if (args.Length > 0)
                return Write(LogLevel.Warn, method, msg, args);
            else
                return Write(LogLevel.Warn, method, msg);
        }

        /// <summary>
        /// Infoログ出力
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task Info(string msg, params object[] args)
        {
            var method = SearchInvoker();
            if (args.Length > 0)
                return Info(method, msg, args);
            else
                return Info(method, msg);
        }

        /// <summary>
        /// Infoログ出力
        /// </summary>
        /// <param name="method">System.Reflection.MethodBase.GetCurrentMethod()</param>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual Task Info(MethodBase method, string msg, params object[] args)
        {
            if (args.Length > 0)
                return Write(LogLevel.Info, method, msg, args);
            else
                return Write(LogLevel.Info, method, msg);
        }

        /// <summary>
        /// Debugログ出力
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task Debug(string msg, params object[] args)
        {
            var method = SearchInvoker();
            if (args.Length > 0)
                return Debug(method, msg, args);
            else
                return Debug(method, msg);
        }

        /// <summary>
        /// Debugログ出力
        /// </summary>
        /// <param name="method">System.Reflection.MethodBase.GetCurrentMethod()</param>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual Task Debug(MethodBase method, string msg, params object[] args)
        {
            if (args.Length > 0)
                return Write(LogLevel.Debug, method, msg, args);
            else
                return Write(LogLevel.Debug, method, msg);
        }

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="level"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public Task Write(LogLevel level, Exception ex)
        {
            var method = SearchInvoker();
            return Write(level, method, ex);
        }

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="level"></param>
        /// <param name="method">System.Reflection.MethodBase.GetCurrentMethod()</param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public virtual Task Write(LogLevel level, MethodBase method, Exception ex)
        {
            var t = BuildMessage(method, ex.Message);
            return WriteAsyncByCase(level, t, ex);
        }

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="level"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task Write(LogLevel level, string format, params object[] args)
        {
            var method = SearchInvoker();
            if (args.Length > 0)
                return Write(level, method, format, args);
            else
                return Write(level, method, format);
        }

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="level"></param>
        /// <param name="method">System.Reflection.MethodBase.GetCurrentMethod()</param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual Task Write(LogLevel level, MethodBase method, string format, params object[] args)
        {
            var t = BuildMessage(method, format);
            if (args.Length > 0)
                return WriteAsyncByCase(level, t, args);
            else
                return WriteAsyncByCase(level, t, default(Exception));
        }

        /// <summary>
        /// スタックトレースを総なめし、クラス名が InvokerDeclarations に前方一致する最初のメソッドを返す。
        /// 呼び元を指定しない場合に使用する。
        /// </summary>
        /// <returns></returns>
        protected MethodBase SearchInvoker()
        {
            var trace = new StackTrace(fNeedFileInfo: true);
            if (trace == null)
                return default(MethodBase);

            var logMethod = trace.GetFrames()
                .Select(frame => frame.GetMethod())
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
            var cname = (method != null ? method.DeclaringType.Name : UNKNOWN);
            var mname = (method != null ? method.Name : UNKNOWN);
            return string.Format("[{0}.{1}] - {2}", cname, mname, msg);
        }

        /// <summary>
        /// ログ出力非同期タスク実行
        /// </summary>
        /// <param name="level"></param>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        private async Task WriteAsyncByCase(LogLevel level, string msg, Exception ex)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    if (ex == null)
                        await Task.Run(() => _LogImpl.Debug(msg)).ConfigureAwait(false);
                    else
                        await Task.Run(() => _LogImpl.Debug(msg, ex)).ConfigureAwait(false);
                    break;
                case LogLevel.Info:
                    if (ex == null)
                        await Task.Run(() => _LogImpl.Info(msg)).ConfigureAwait(false);
                    else
                        await Task.Run(() => _LogImpl.Info(msg, ex)).ConfigureAwait(false);
                    break;
                case LogLevel.Warn:
                    if (ex == null)
                        await Task.Run(() => _LogImpl.Warn(msg)).ConfigureAwait(false);
                    else
                        await Task.Run(() => _LogImpl.Warn(msg, ex)).ConfigureAwait(false);
                    break;
                case LogLevel.Error:
                    if (ex == null)
                        await Task.Run(() => _LogImpl.Error(msg)).ConfigureAwait(false);
                    else
                        await Task.Run(() => _LogImpl.Error(msg, ex)).ConfigureAwait(false);
                    break;
                case LogLevel.Fatal:
                    if (ex == null)
                        await Task.Run(() => _LogImpl.Fatal(msg)).ConfigureAwait(false);
                    else
                        await Task.Run(() => _LogImpl.Fatal(msg, ex)).ConfigureAwait(false);
                    break;
            }
        }

        /// <summary>
        /// ログ出力非同期タスク実行
        /// （～Format()版）
        /// </summary>
        /// <param name="level"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task WriteAsyncByCase(LogLevel level, string format, object[] args)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    await Task.Run(() => _LogImpl.DebugFormat(format, args)).ConfigureAwait(false);
                    break;
                case LogLevel.Info:
                    await Task.Run(() => _LogImpl.InfoFormat(format, args)).ConfigureAwait(false);
                    break;
                case LogLevel.Warn:
                    await Task.Run(() => _LogImpl.WarnFormat(format, args)).ConfigureAwait(false);
                    break;
                case LogLevel.Error:
                    await Task.Run(() => _LogImpl.ErrorFormat(format, args)).ConfigureAwait(false);
                    break;
                case LogLevel.Fatal:
                    await Task.Run(() => _LogImpl.FatalFormat(format, args)).ConfigureAwait(false);
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
