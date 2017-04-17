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
        /// 監視起動時イベント
        /// </summary>
        public event EventHandler WatcherStarted;

        /// <summary>
        /// 監視停止時イベント
        /// </summary>
        public event EventHandler WatcherStopped;

        /// <summary>
        /// 監視タイミング時イベント
        /// </summary>
        public event EventHandler WatcherTimeElapsed;

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
        /// 監視中であるかどうかを返します。
        /// </summary>
        public bool IsWatching
        {
            get { return watchTimer.Enabled; }
        }

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

            //起動イベントトリガー
            OnWatcherStarted(EventArgs.Empty);
        }

        /// <summary>
        /// 監視起動イベントトリガー
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnWatcherStarted(EventArgs e)
        {
            if (WatcherStarted != null)
                WatcherStarted.Invoke(this, e);
        }

        /// <summary>
        /// 監視を停止します
        /// </summary>
        public virtual void Stop()
        {
            //監視タイマー停止
            watchTimer.Stop();

            //停止イベントトリガー
            if (WatcherStopped != null)
                WatcherStopped.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 監視停止イベントトリガー
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnWatcherStopped(EventArgs e)
        {
            if (WatcherStopped != null)
                WatcherStopped.Invoke(this, e);
        }

        /// <summary>
        /// 次回監視時刻を更新します。
        /// </summary>
        private void UpdateNextWatchTime()
        {
            nextWatchTime = DateTime.UtcNow.Add(watchInterval);
        }

        /// <summary>
        /// 監視タイミング処理
        /// </summary>
        public virtual void TimerElapse()
        {
            try
            {
                //次回監視時刻を経過していない場合、何もしない
                if (DateTime.UtcNow < nextWatchTime) { return; }

                //監視タイミングイベントトリガー
                if (WatcherTimeElapsed != null)
                    WatcherTimeElapsed.Invoke(this, EventArgs.Empty);

                //次回監視時刻の設定
                UpdateNextWatchTime();
            }
            catch
            {
                //サービス停止
                Stop();
            }
        }

        /// <summary>
        /// 監視タイミングイベントトリガー
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnWatcherTimeElapsed(EventArgs e)
        {
            if (WatcherTimeElapsed != null)
                WatcherTimeElapsed.Invoke(this, e);
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
                TimerElapse();
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
        public void Dispose()
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
            subscriberWatchTimerElapsed = EventSubscriber.Create(watchTimer, nameof(watchTimer.Elapsed), (ElapsedEventHandler)Timer_Elapsed);
        }
    }
}
