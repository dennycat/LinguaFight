using System;
using System.Windows.Threading;

namespace LinguaFight.Logic
{
    public class TimerController
    {
        private DispatcherTimer _timer;
        public event Action? OnTick;

        public TimerController()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += (s, e) => OnTick?.Invoke();
        }

        public void SetInterval(int minutes)
        {
            _timer.Interval = TimeSpan.FromMinutes(minutes);
        }

        public void Start() => _timer.Start();
        public void Pause() => _timer.Stop();
        public void Stop() => _timer.Stop();
    }
}
