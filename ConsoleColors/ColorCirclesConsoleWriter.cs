namespace ConsoleColors
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading;

    public class ColorCirclesConsoleWriter : ConsoleWriterBase
    {
        private const int MaxColorEnum = 15;

        /// <summary>
        /// Use decimal chars from a newly generated guid to generate a salt value. Otherwise, pseudo-random 
        /// values won't really be very random if starting multiple threads at once.
        /// </summary>
        private readonly Random rand = new Random(int.Parse(Regex.Replace(Guid.NewGuid().ToString(), "[^\\d]", string.Empty).Substring(0, 4)));

        public ColorCirclesConsoleWriter(object lockObj, int width, int height, int sleepTime)
            : base(lockObj, width, height, sleepTime)
        {
        }

        public override void Go()
        {
            const int Radius = 6;
            var angle = 0;

            var col = this.rand.Next(Radius * 2, this.Width - Radius);
            var row = this.rand.Next(0 + Radius, this.Height - Radius);
            var backgroundColor = (ConsoleColor)this.rand.Next(0, MaxColorEnum);

            var cellsTraveled = 0;

            while (this.Run)
            {
                lock (this.LockObj)
                {
                    Console.CursorLeft = Math.Min(this.Width, Math.Max(0, col + (int)(Math.Cos(angle) * Radius)));
                    Console.CursorTop = Math.Min(this.Height, Math.Max(0, row + (int)(Math.Sin(angle) * Radius)));
                    Console.BackgroundColor = backgroundColor;
                    Console.ForegroundColor = (ConsoleColor)this.rand.Next(0, MaxColorEnum);
                    Console.Write((char)this.rand.Next(32, 126)); // Printable ascii latters
                }

                cellsTraveled++;
                angle++;

                if (cellsTraveled >= Radius * Math.PI)
                {
                    // Get start position for next circle.
                    angle = 0;
                    cellsTraveled = 0;
                    col = Math.Min(this.Width, Math.Max(0, col + this.rand.Next(-10, 10)));
                    row = Math.Min(this.Height, Math.Max(0, row + this.rand.Next(-10, 10)));
                    backgroundColor = (ConsoleColor)this.rand.Next(0, MaxColorEnum);
                }

                Thread.Sleep(this.SleepTime);
            }
        }
    }
}