using System;
using System.Timers;

namespace UII.Model
{
    public class PLCService
    {
        private readonly Timer _timer;
        private readonly Random _random;

        public event Action<int> DataUpdated;

        public PLCService()
        {
            _random = new Random();
            _timer = new Timer(1000);
            _timer.Elapsed += (s, e) => OnDataUpdated();
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void OnDataUpdated()
        {
            int value = _random.Next(0, 100); // giả lập dữ liệu PLC
            DataUpdated?.Invoke(value);
        }
    }
}
