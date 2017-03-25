using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace KOILib.Common
{
    public abstract class WatcherBase : IDisposable
    {
        #region Static Members
        /// <summary>
        /// 監視タイマー分解能(ミリ秒)
        /// </summary>
        private const int InternalTimerResolution = 100;
        #endregion

        /// <summary>
        /// 定時処理本体
        /// </summary>
        protected abstract void TimerElapsedBody();

        /// <summary>
        /// 実行ロックオブジェクト
        /// </summary>
        private readonly object lockObj = new object();

        /// <summary>
        /// 監視タイマーコンポーネント
        /// </summary>
        private Timer watchTimer;

        /// <summary>
        /// 監視タイマーElapsedイベントサブスクライバー
        /// </summary>
        private IDisposable subscriberWatchTimerElapsed;

        /// <summary>
        /// 監視間隔(ミリ秒)
        /// </summary>
        private TimeSpan watchInterval;

        /// <summary>
        /// 次回監視時刻(UTC)
        /// </summary>
        private DateTime nextWatchTime;


        /// <summary>
        /// 監視を起動します
        /// </summary>
        /// <param name="intervalmsec">ポーリング間隔(ミリ秒)</param>
        public virtual void Start(int intervalmsec)
        {
            //ポーリング間隔設定
            watchInterval = new TimeSpan(0, 0, 0, 0, intervalmsec);

            //次回監視時刻を決定
            UpdateNextWatchTime();

            //監視タイマー起動
            watchTimer.Interval = InternalTimerResolution; //internal timer resolution msec.
            watchTimer.Start();
        }

        /// <summary>
        /// 監視を停止します
        /// </summary>
        public virtual void Stop()
        {
            //監視タイマー停止
            watchTimer.Stop();
        }

        /// <summary>
        /// 次回監視時刻を更新します。
        /// </summary>
        private void UpdateNextWatchTime()
        {
            nextWatchTime = DateTime.UtcNow.Add(watchInterval);
        }

        /// <summary>
        /// 監視タイマーイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (lockObj)
            {
                try
                {
                    //次回監視時刻を経過していない場合、何もしない
                    if (DateTime.UtcNow < nextWatchTime) { return; }

                    //定時処理
                    TimerElapsedBody();

                    //次回監視時刻の設定
                    UpdateNextWatchTime();
                }
                catch
                {
                    //サービス停止
                    Stop();
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // マネージ状態を破棄します (マネージ オブジェクト)。
                    if (subscriberWatchTimerElapsed != null)
                        subscriberWatchTimerElapsed.Dispose();
                }

                // アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~WatcherBase() {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        void IDisposable.Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected WatcherBase() : base()
        {
            watchTimer = new Timer();

            //タイマーイベントハンドラ登録
            subscriberWatchTimerElapsed = EventSubscriber.Create(watchTimer, "Elapsed", (ElapsedEventHandler)Timer_Elapsed);
        }
    }
}
