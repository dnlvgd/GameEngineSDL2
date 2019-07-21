using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace gpp2019_haifischflosse
{
    public static class FrameInfo
    {
        public static long FrameCount { get; private set; } = 0;    //amount of frames since the game loop start 
        public static double FPS { get; private set; } = 0;         //frames per second
        public static double AverageFPS { get; private set; } = 0;  //average frames per second
        public static uint ResetInterval { get; set; } = 5;         //resets the average frames per second after the given value (after the amount of passed frames)

        public static void CalculateFrameInfo(double time, double deltaTime)
        {
            //ONLY call this method after the new elapsed time was calculated
            FrameCount++;

            //prevents the average FPS value from becoming too heavily weighted / nearly equal to a value over time
            long tmpFrameCount = FrameCount;
            double tmpTime = time;
            if (FrameCount % ResetInterval == 0)
            {
                tmpFrameCount = 1;
                tmpTime = deltaTime;
            }

            AverageFPS = tmpFrameCount / MillisecondsToSec(tmpTime);
            FPS = 1.0D / MillisecondsToSec(deltaTime);
        }

        public static string GetFpsText()
        {
            return "FPS: " + Math.Round(FPS, 2);
        }

        public static SDL_Color GetFpsColor()
        {
            //default color for FPS = white
            SDL_Color fpsColor = new SDL_Color { r = 255, g = 255, b = 255, a = 255 };

            //0 - 29 FPS
            if (FPS < 30.0f)
            {
                //red
                fpsColor = new SDL_Color { r = 238, g = 44, b = 44, a = 255 };
            }
            //30 - 59 FPS
            else if (FPS >= 30.0f && FPS < 60.0f)
            {
                //orange
                fpsColor = new SDL_Color { r = 255, g = 165, b = 0, a = 255 };
            }
            //60 - ~ FPS
            else if (FPS >= 60.0f)
            {
                //green
                fpsColor = new SDL_Color { r = 0, g = 205, b = 0, a = 255 };
            }

            return fpsColor;
        }

        private static double SecondsToMilli(double seconds)
        {
            return seconds * 1000.0D; // 1 sec = 1000 ms 
        }

        private static double MillisecondsToSec(double milliseconds)
        {
            return milliseconds / 1000.0D;  // 1 ms = 0.001 sec
        }

        public static void PrintNewFrameRateInfo()
        {
            Console.WriteLine(
                "FrameCount: {0}" +
                " | Average FPS: {1}" +
                " | FPS: {2}",
                FrameCount,
                AverageFPS,
                FPS
            );
        }
    }
}
