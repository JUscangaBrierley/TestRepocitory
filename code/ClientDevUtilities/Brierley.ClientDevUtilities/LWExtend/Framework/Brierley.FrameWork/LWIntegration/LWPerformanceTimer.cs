using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using System.Diagnostics;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.LWIntegration
{
    public class LWPerformanceTimer
    {
        #region Kernel API
        [DllImport("Kernel32.dll")]
        public static extern void QueryPerformanceCounter(ref long ticks);
        #endregion

        #region Fields
        private long _startTime = -1;
        private long _endTime = -1;
        #endregion

        public LWPerformanceTimer()
        {
            Start();
        }

        private void Start()
        {
            QueryPerformanceCounter(ref _startTime);
        }

        private void Stop()
        {
            QueryPerformanceCounter(ref _endTime);
        }

        public long Difference()
        {
            if (_startTime == -1)
            {
                throw new LWException("Timer not started.");
            }

            if (_endTime == -1)
            {
                Stop();
            }
            return _endTime - _startTime;
        }
    }
}
