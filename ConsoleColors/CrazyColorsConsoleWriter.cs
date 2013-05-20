namespace ConsoleColors
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Threading;

    public class CrazyColorsConsoleWriter : ConsoleWriterBase
    {
        private const int MaxColorEnum = 15;

        private readonly bool printStatistics;

        /// <summary>
        /// Use decimal chars from a newly generated guid to generate a salt value. Otherwise, pseudo-random 
        /// values won't really be very random if starting multiple threads at once.
        /// </summary>
        private readonly Random rand = new Random(int.Parse(Regex.Replace(Guid.NewGuid().ToString(), "[^\\d]", string.Empty).Substring(0, 4)));

        private readonly List<string> coordinates = new List<string>();

        public CrazyColorsConsoleWriter(object lockObj, int width, int height, int sleepTime, bool printStatistics)
        {
            this.LockObj = lockObj;
            this.Width = width;
            this.Height = height;
            this.SleepTime = sleepTime;
            this.Run = true;

            this.printStatistics = printStatistics;
        }

        public override void Go()
        {
            var cellCount = this.Width * this.Height;
            var start = DateTime.Now;

            while (this.Run)
            {
                lock (this.LockObj)
                {
                    Console.ForegroundColor = (ConsoleColor)this.rand.Next(0, MaxColorEnum);
                    Console.BackgroundColor = (ConsoleColor)this.rand.Next(0, MaxColorEnum);
                    Console.CursorLeft = this.rand.Next(0, this.Width);
                    Console.CursorTop = this.rand.Next(0, this.Height);
                    Console.Write((char)this.rand.Next(32, 126)); // Printable ascii latters
                }

                this.PrinstStatistics(start, cellCount, Console.CursorLeft, Console.CursorTop);

                Thread.Sleep(this.SleepTime);
            }
        }

        private void PrinstStatistics(DateTime start, int cellCount, int left, int top)
        {
            if (!this.printStatistics)
            {
                return;
            }

            var coordinate = left.ToString(CultureInfo.InvariantCulture) + ":" + top.ToString(CultureInfo.InvariantCulture);

            if (!this.coordinates.Contains(coordinate))
            {
                this.coordinates.Add(coordinate);
            }

            this.Run = this.Run && this.coordinates.Count < cellCount;

            lock (this.LockObj)
            {
                Console.CursorLeft = 0;
                Console.CursorTop = this.Height - 2;
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("Time Lapsed: " + (DateTime.Now - start));
                Console.WriteLine(this.coordinates.Count.ToString(CultureInfo.InvariantCulture) + " of " + cellCount.ToString(CultureInfo.InvariantCulture) + " cells populated.");
            }
        }
    }
}
