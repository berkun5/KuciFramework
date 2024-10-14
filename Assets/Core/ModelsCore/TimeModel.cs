using System;
using Kuci.Core.Extensions;
using Kuci.Logger;
using UnityEngine;

namespace Kuci.Models
{
    public class TimeModel : ModelBase
    {
        public event Action<int> SecondPassed;
        public event Action<int> FrameTicked;
        public static DateTime CurrentTime => DateTime.UtcNow;
        public static double CurrentUnixTimeStamp => _currentUnixTimeStamp;
        
        private static double _currentUnixTimeStamp;
        private long _nextSecondsTick = long.MaxValue;
        private int _tickedSeconds = 0;
        
        protected override void OnInit()
        {
            _nextSecondsTick = (long)CurrentTime.ToUnixTimestamp() + 1;
        }

        protected override void OnUpdate()
        { 
            base.OnUpdate();
            _currentUnixTimeStamp = CurrentTime.ToUnixTimestamp();
            TickTimeInSeconds();

        }

        private void TickTimeInSeconds()
        {
            FrameTicked?.Invoke(Time.frameCount);
            
            if (CurrentUnixTimeStamp < _nextSecondsTick)
            {
                return;
            }

            _tickedSeconds++;
            SecondPassed?.Invoke(_tickedSeconds);
            _nextSecondsTick = (long)CurrentUnixTimeStamp + 1;
        }
        
        // New method to get the current time in hh:mmPM/AM format
        public string GetFormattedCurrentTime()
        {
            return DateTime.Now.ToString("hh:mmtt", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}