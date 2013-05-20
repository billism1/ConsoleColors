namespace ConsoleColors
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading;

    public class SineWaveColorsConsoleWriter : ConsoleWriterBase
    {
        private int maxColorEnum = 15;

        /// <summary>
        /// Use decimal chars from a newly generated guid to generate a salt value. Otherwise, pseudo-random 
        /// values won't really be very random if starting multiple threads at once.
        /// </summary>
        private Random rand = new Random(int.Parse(Regex.Replace(Guid.NewGuid().ToString(), "[^\\d]", string.Empty).Substring(0, 4)));

        public SineWaveColorsConsoleWriter(object lockObj, int width, int height, int sleepTime)
        {
            this.LockObj = lockObj;
            this.Width = width;
            this.Height = height;
            this.SleepTime = sleepTime;
            this.Run = true;
        }

        public override void Go()
        {
            var waveHeight = 2;

            var col = 0;
            var currLine = rand.Next(0 + waveHeight, this.Height - waveHeight);

            var backgroundColor = (ConsoleColor)rand.Next(0, maxColorEnum);

            while (this.Run)
            {
                lock (this.LockObj)
                {
                    Console.CursorLeft = col;
                    Console.CursorTop = currLine + (int)(Math.Sin(col) * waveHeight);
                    Console.BackgroundColor = backgroundColor;
                    Console.ForegroundColor = (ConsoleColor)rand.Next(0, maxColorEnum);
                    Console.Write((char)(rand.Next(32, 126))); // Printable ascii latters
                }

                col++;
                if (col > this.Width)
                {
                    col = 0;
                    currLine = rand.Next(0 + waveHeight, this.Height - waveHeight);
                    backgroundColor = (ConsoleColor)rand.Next(0, maxColorEnum);
                    //Thread.Sleep(rand.Next(1, 400));
                }

                Thread.Sleep(this.SleepTime);
            }
        }
    }
}
