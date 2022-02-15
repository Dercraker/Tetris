using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;

namespace Tetris
{
    public class Timer
    {
        public int time { get; set; }

        public int reverseTotalTime { get; set; }

        private DispatcherTimer Chrono = null!;
        private DispatcherTimer ReverseTotalTimer = null!;

        public Timer()
        {
            reverseTotalTime = 0;
        }
        public void SetTimer()
        {
            Chrono = new DispatcherTimer();
            Chrono.Interval = new TimeSpan(0, 0, 1);
            Chrono.Tick += Timer_Tick;
            Chrono.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            time++;
        }
        public void StopTimer()
        {
            Chrono.Stop();
        }
        public void SetReverseTimer()
        {
            Chrono = new DispatcherTimer();
            Chrono.Interval = new TimeSpan(0, 0, 1);
            Chrono.Tick += ReverseTimer_Tick;
            Chrono.Start();
        }
        private void ReverseTimer_Tick(object sender, EventArgs e)
        {
            time--;
        }
        public void SetTotalTimer()
        {
            ReverseTotalTimer = new DispatcherTimer();
            ReverseTotalTimer.Interval = new TimeSpan(0, 0, 1);
            ReverseTotalTimer.Tick += TotalTimer_Tick;
            ReverseTotalTimer.Start();
        }
        private void TotalTimer_Tick(object sender, EventArgs e)
        {
            reverseTotalTime++;
        }
        public void StopTotalTimer()
        {
            ReverseTotalTimer.Stop();
        }
    }
}
