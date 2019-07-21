using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gpp2019_haifischflosse
{
    public static class TimeInfo
    {
        private static Stopwatch timer = new Stopwatch();
        public static double Time { get; private set; } = 0;      //elapsed time (in ms) since the game loop start
        public static double TimeSec => Time / 1000.0;
        public static double DeltaTime { get; private set; }      //display duration (in ms) of the last frame 
        public static double DeltaTimeSec => DeltaTime / 1000.0;  


        public static void Begin()
        {
            timer.Reset();
            timer.Start();
        }

        public static void End()
        {
            timer.Stop();
            CalculateTime();
        }

        private static void CalculateTime()
        {
            DeltaTime = (timer.ElapsedTicks / (double)Stopwatch.Frequency) * 1000.0D; //sec to ms (1 sec = 1000 ms)
            Time += DeltaTime; 
        }

        public static void PrintTimeAndDeltaTime()
        {
            Console.WriteLine(
                "Time: {0}sec {1}ms" +
                " | DeltaTime: {2}sec {3}ms",
                TimeSec,
                Time,
                DeltaTimeSec,
                DeltaTime
            );
        }

        public static void PrintTime()
        {
            Console.WriteLine(
                "Time: {0}sec {1}ms",
                TimeSec,
                Time
            );
        }

        public static void PrintDeltaTime()
        {
            Console.WriteLine(
                "DeltaTime: {0}sec {1}ms",
                DeltaTimeSec,
                DeltaTime
            );
        }

        public static void Sleep(double millisecondsToSleep)
        {
            Thread.Sleep((int)millisecondsToSleep);
        }
    }
}
