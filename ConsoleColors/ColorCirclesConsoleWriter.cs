namespace ConsoleColors
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading;

    public class ColorCirclesConsoleWriter : ConsoleWriterBase
    {
        private int maxColorEnum = 15;

        /// <summary>
        /// Use decimal chars from a newly generated guid to generate a salt value. Otherwise, pseudo-random 
        /// values won't really be very random if starting multiple threads at once.
        /// </summary>
        private Random rand = new Random(int.Parse(Regex.Replace(Guid.NewGuid().ToString(), "[^\\d]", string.Empty).Substring(0, 4)));

        public ColorCirclesConsoleWriter(object lockObj, int width, int height, int sleepTime)
        {
            this.LockObj = lockObj;
            this.Width = width;
            this.Height = height;
            this.SleepTime = sleepTime;
            this.Run = true;
        }

        public override void Go()
        {
            var radius = 6;
            var angle = 0;

            var col = rand.Next(radius * 2, this.Width - radius);
            var row = rand.Next(0 + radius, this.Height - radius);
            var backgroundColor = (ConsoleColor)rand.Next(0, maxColorEnum);

            var startCol = col;
            var startRow = row;
            var circumferenceTraveled = 0;

            while (this.Run)
            {
                lock (this.LockObj)
                {
                    Console.CursorLeft = Math.Min(this.Width, Math.Max(0, col + (int)(Math.Cos(angle) * radius)));
                    Console.CursorTop = Math.Min(this.Height, Math.Max(0, row + (int)(Math.Sin(angle) * radius)));
                    Console.BackgroundColor = backgroundColor;
                    Console.ForegroundColor = (ConsoleColor)rand.Next(0, maxColorEnum);
                    Console.Write((char)(rand.Next(32, 126))); // Printable ascii latters
                    circumferenceTraveled++;
                }

                angle++;

                if (circumferenceTraveled >= radius * Math.PI)
                {
                    // Get start position for next circle.
                    angle = 0;
                    circumferenceTraveled = 0;
                    col = Math.Min(this.Width, Math.Max(0, col + rand.Next(-10, 10)));
                    row = Math.Min(this.Height, Math.Max(0, row + rand.Next(-10, 10)));
                    backgroundColor = (ConsoleColor)rand.Next(0, maxColorEnum);
                }

                Thread.Sleep(this.SleepTime);
            }
        }
    }
}