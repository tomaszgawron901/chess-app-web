using ChessApp.Web.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChessApp.Web.Components
{
    public class TimerComponentBase: ComponentBase
    {
        private CancellationTokenSource cts;
        public string timeString { get; private set; }
        public int time { get; private set; }
        public int frequency { get; private set; }

        public bool isGoing { get; private set; }

        public TimerComponentBase()
        {
            this.frequency = 1000;
            this.timeString = "00:00";
            this.isGoing = false;
        }

        private void SetTimeString(string timeString)
        {
            this.timeString = timeString;
            this.StateHasChanged();
        }

        private void StartClock()
        {
            this.cts = new CancellationTokenSource();
            var token = this.cts.Token;
            Task.Run(async () => {
                int rest = time % frequency;
                await Task.Delay(rest, token);
                if (token.IsCancellationRequested) { return; }

                this.SetTime(time - rest);
                while (time > 0)
                {
                    await Task.Delay(frequency, token);
                    if (token.IsCancellationRequested) { return; }
                    this.SetTime(time - frequency);
                }
                this.SetTime(0);
            }, token);
            this.isGoing = true;
        }

        private void StopClock()
        {
            if(isGoing)
            {
                this.cts.Cancel();
                this.isGoing = false;
            }
        }

        private void SetTime(int time)
        {
            this.time = time;
            this.SetTimeString(this.TimeToString(this.time));
        }

        private string TimeToString(int time)
        {
            int secs = time / 1000;
            int mins = secs / 60;
            return $"{mins % 100 / 10}{mins % 10}:{secs%60/10}{secs%10}";
        }

        public void SetClock(SharedClock clock)
        {
            this.StopClock();
            this.SetTime((int)clock.Time);
            if (clock.Started)
            {
                this.StartClock();
            }
        }

    }
}
