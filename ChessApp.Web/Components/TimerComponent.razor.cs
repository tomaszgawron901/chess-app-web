using ChessApp.Web.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChessApp.Web.Components
{
    public partial class TimerComponent: ComponentBase, IDisposable
    {
        private CancellationTokenSource? _cts;
        private Task? _timerTask;
        
        protected string timeString { get; private set; }
        public TimeSpan time { get; private set; }
        public int frequency { get; private set; }
        
        public TimerComponent()
        {
            time = TimeSpan.Zero;
            timeString = TimeToString(time);
        }

        private void SetTimeString(string timeString)
        {
            this.timeString = timeString;
            StateHasChanged();
        }

        public void StartClock()
        {
            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            _timerTask = Task.Run(async () => {
                frequency = time >= TimeSpan.FromSeconds(10) ? 1000 : 100;
                int rest = (int)time.TotalMilliseconds % frequency;
                await Task.Delay(rest, token);
                if (token.IsCancellationRequested) { return; }
                SetTime(time - TimeSpan.FromMilliseconds(rest));

                PeriodicTimer timer;
                if (time >= TimeSpan.FromSeconds(10))
                {
                    frequency = 1000;
                    timer = new PeriodicTimer(TimeSpan.FromMilliseconds(frequency));
                    try
                    {
                        while (time >= TimeSpan.FromSeconds(10) && await timer.WaitForNextTickAsync(token))
                        {
                            SetTime(time - TimeSpan.FromMilliseconds(frequency));
                        }
                    }
                    catch (OperationCanceledException) { }
                    timer.Dispose();
                }

                frequency = 100;
                timer = new PeriodicTimer(TimeSpan.FromMilliseconds(frequency));
                try
                {
                    while (time >= TimeSpan.Zero && await timer.WaitForNextTickAsync(token))
                    {
                        SetTime(time - TimeSpan.FromMilliseconds(frequency));
                    }
                }
                catch (OperationCanceledException) { }
                timer.Dispose();

                SetTime(TimeSpan.Zero);
                
            }, token);
        }
        
        

        public async Task StopClock()
        {
            if (_timerTask is null || _timerTask.IsCompleted) { return; }

            _cts?.Cancel();
            await _timerTask;
            _cts?.Dispose();
        }

        private void SetTime(TimeSpan time)
        {
            this.time = time;
            SetTimeString(TimeToString(this.time));
        }
        
        public async Task SetClock(SharedClock clock)
        {
            await StopClock();
            SetTime(TimeSpan.FromMilliseconds(clock.Time));
            if (clock.Started)
            {
                StartClock();
            }
        }

        private static string TimeToString(TimeSpan time)
        {
            if (time >= TimeSpan.FromSeconds(10))
            {
                return time.ToString(@"mm\:ss");
            }
            else
            {
                return time.ToString(@"s\.f");
            }
            
        }

        public void Dispose()
        {
            _cts?.Dispose();
        }
    }
}
