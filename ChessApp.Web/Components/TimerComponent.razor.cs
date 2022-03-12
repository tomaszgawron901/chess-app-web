using ChessApp.Web.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChessApp.Web.Components
{
    public partial class TimerComponent: ComponentBase, IDisposable
    {
        private CancellationTokenSource cts;
        public string timeString { get; private set; }
        public int time { get; private set; }
        public int frequency { get; private set; }

        public bool isGoing { get; private set; }

        public TimerComponent()
        {
            frequency = 1000;
            timeString = "00:00";
            isGoing = false;
        }

        private void SetTimeString(string timeString)
        {
            this.timeString = timeString;
            StateHasChanged();
        }

        private void StartClock()
        {
            cts = new CancellationTokenSource();
            var token = cts.Token;
            Task.Run(async () => {
                int rest = time % frequency;
                await Task.Delay(rest, token);
                if (token.IsCancellationRequested) { return; }

                SetTime(time - rest);
                while (time > 0)
                {
                    await Task.Delay(frequency, token);
                    if (token.IsCancellationRequested) { return; }
                    SetTime(time - frequency);
                }
                SetTime(0);
            }, token);
            isGoing = true;
        }

        private void StopClock()
        {
            if(isGoing)
            {
                cts.Cancel();
                cts.Dispose();
                isGoing = false;
            }
        }

        private void SetTime(int time)
        {
            this.time = time;
            SetTimeString(TimeToString(this.time));
        }


        public void SetClock(SharedClock clock)
        {
            StopClock();
            SetTime((int)clock.Time);
            if (clock.Started)
            {
                StartClock();
            }
        }

        private static string TimeToString(int time)
        {
            int secs = time / 1000;
            int mins = secs / 60;
            return $"{mins % 100 / 10}{mins % 10}:{secs % 60 / 10}{secs % 10}";
        }

        public void Dispose()
        {
            cts?.Dispose();
        }
    }
}
