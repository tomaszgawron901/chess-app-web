using ChessApp.Web.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChessApp.Web.Components
{
    public partial class TimerComponent: ComponentBase, IDisposable
    {
        private readonly static int _above10secPeriod = 1000;
        private readonly static int _below10secPeriod = 100;
        private readonly static string _above10secFormat = @"mm\:ss";
        private readonly static string _below10secFormat = @"s\.f";

        public int FormatPartitionInMs { get; init; } = 10_000;

        private CancellationTokenSource? _cts;
        private Task? _timerTask;
        
        protected string timeString { get; private set; }
        public TimeSpan time { get; private set; }
        
        public TimerComponent()
        {
            time = TimeSpan.Zero;
            timeString = time.ToString(_below10secFormat);
            
            
        }

        private void SetTimeString(string timeString)
        {
            this.timeString = timeString;
            StateHasChanged();
        }

        public async Task SetClock(SharedClock clock)
        {
            await StopClock();
            SetTime(TimeSpan.FromMilliseconds(clock.Time), clock.Time >= FormatPartitionInMs ? _above10secFormat : _below10secFormat);
            if (clock.Started)
            {
                _cts = new CancellationTokenSource();
                _timerTask = CountDownTime(_cts.Token);
            }
        }
        
        public async Task StopClock()
        {
            if (_timerTask is null || _timerTask.IsCompleted) { return; }

            _cts?.Cancel();
            await _timerTask;
            _cts?.Dispose();
        }

        private async Task AlignTimeToWholeAndDisplay(int period, string format, CancellationToken token = default)
        {
            int rest = (int)time.TotalMilliseconds % period;
            await Task.Delay(rest, token);
            SetTime(time - TimeSpan.FromMilliseconds(rest), format);
        }

        private async Task CountDownTimeAndDisplayPeriodicaly(int period, string format, TimeSpan endTime, CancellationToken token = default)
        {
            PeriodicTimer timer = new(TimeSpan.FromMilliseconds(period));
            try
            {
                while (time >= endTime && await timer.WaitForNextTickAsync(token))
                {
                    SetTime(time - TimeSpan.FromMilliseconds(period), format);
                }
            }
            catch(OperationCanceledException ocex)
            {
                throw ocex;
            }
            finally
            {
                timer.Dispose();
            }
        }

        private async Task CountDownTime(CancellationToken token = default(CancellationToken))
        {
            try
            {
                if (time >= TimeSpan.FromSeconds(FormatPartitionInMs))
                {
                    await AlignTimeToWholeAndDisplay(_above10secPeriod, _above10secFormat, token);
                    await CountDownTimeAndDisplayPeriodicaly(_above10secPeriod, _above10secFormat, TimeSpan.FromSeconds(10), token);
                }
                else
                {
                    await AlignTimeToWholeAndDisplay(_below10secPeriod, _below10secFormat, token);
                }
                await CountDownTimeAndDisplayPeriodicaly(_below10secPeriod, _below10secFormat, TimeSpan.Zero, token);
                SetTime(TimeSpan.Zero, _below10secFormat);
                
            }
            catch(OperationCanceledException){}
        }

        private void SetTime(TimeSpan time, string format)
        {
            this.time = time;
            SetTimeString(this.time.ToString(format));
        }
        
        public void Dispose()
        {
            _cts?.Dispose();
        }
    }
}
