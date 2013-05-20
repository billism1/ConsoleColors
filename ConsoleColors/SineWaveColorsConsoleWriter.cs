namespace ConsoleColors
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading;

    public class SineWaveColorsConsoleWriter : ConsoleWriterBase
    {
        private const int MaxColorEnum = 15;

        /// <summary>
        /// Use decimal chars from a newly generated guid to generate a salt value. Otherwise, pseudo-random 
        /// values won't really be very random if starting multiple threads at once.
        /// </summary>
        private readonly Random rand = new Random(int.Parse(Regex.Replace(Guid.NewGuid().ToString(), "[^\\d]", string.Empty).Substring(0, 4)));

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
            const int WaveHeight = 2;

            var col = 0;
            var currLine = this.rand.Next(0 + WaveHeight, this.Height - WaveHeight);

            var backgroundColor = (ConsoleColor)this.rand.Next(0, MaxColorEnum);

            while (this.Run)
            {
                lock (this.LockObj)
                {
                    Console.CursorLeft = col;
                    Console.CursorTop = currLine + (int)(Math.Sin(col) * WaveHeight);
                    Console.BackgroundColor = backgroundColor;
                    Console.ForegroundColor = (ConsoleColor)this.rand.Next(0, MaxColorEnum);
                    Console.Write((char)this.rand.Next(32, 126)); // Printable ascii latters
                }

                col++;
                if (col > this.Width)
                {
                    col = 0;
                    currLine = this.rand.Next(0 + WaveHeight, this.Height - WaveHeight);
                    backgroundColor = (ConsoleColor)this.rand.Next(0, MaxColorEnum);
                }

                Thread.Sleep(this.SleepTime);
            }
        }
    }
}
