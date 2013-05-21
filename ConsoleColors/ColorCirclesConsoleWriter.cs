namespace ConsoleColors
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading;

    public class ColorCirclesConsoleWriter : ConsoleWriterBase
    {
        private const int MaxColorEnum = 15;

        private bool timedDelete;

        /// <summary>
        /// Use decimal chars from a newly generated guid to generate a salt value. Otherwise, pseudo-random 
        /// values won't really be very random if starting multiple threads at once.
        /// </summary>
        private Random rand = new Random(int.Parse(Regex.Replace(Guid.NewGuid().ToString(), "[^\\d]", string.Empty).Substring(0, 4)));

        public ColorCirclesConsoleWriter(object lockObj, int width, int height, int sleepTime, bool timedDelete)
            : base(lockObj, width, height, sleepTime)
        {
            this.timedDelete = timedDelete;
        }

        public override void Go()
        {
            const int Radius = 6;
            var angle = 0;

            var col = this.rand.Next(Radius * 2, this.Width - Radius);
            var row = this.rand.Next(0 + Radius, this.Height - Radius);
            var backgroundColor = (ConsoleColor)this.rand.Next(0, MaxColorEnum);

            while (this.Run)
            {
                var left = Math.Min(this.Width, Math.Max(0, col + (int)(Math.Cos(angle) * Radius)));
                var top = Math.Min(this.Height, Math.Max(0, row + (int)(Math.Sin(angle) * Radius)));

                lock (this.LockObj)
                {
                    Console.CursorLeft = left;
                    Console.CursorTop = top;
                    Console.BackgroundColor = backgroundColor;
                    Console.ForegroundColor = (ConsoleColor)this.rand.Next(0, MaxColorEnum);
                    Console.Write((char)this.rand.Next(32, 126)); // Printable ascii latters
                }

                if (this.timedDelete && angle == 0)
                {
                    var col1 = col;
                    var row1 = row;
                    var clearThread = new Thread(() => this.BlackOutShape(col1, row1, Radius));
                    clearThread.Start();
                }

                angle++;

                if (angle >= Radius * Math.PI)
                {
                    // Get start position for next circle.
                    angle = 0;
                    col = Math.Min(this.Width - Radius, Math.Max(Radius, col + this.rand.Next(-10, 11)));
                    row = Math.Min(this.Height - Radius, Math.Max(Radius, row + this.rand.Next(-10, 11)));
                    backgroundColor = (ConsoleColor)this.rand.Next(0, MaxColorEnum);
                    this.rand = new Random(int.Parse(Regex.Replace(Guid.NewGuid().ToString(), "[^\\d]", string.Empty).Substring(0, 4)));
                }

                Thread.Sleep(this.SleepTime);
            }
        }

        /// <summary>
        /// Called inside a thread to delete a previously drawn shape.
        /// We can't actually detect if a cell has been overwritten (by another thread's writer) since the shape was made, so the 
        /// next best thing is to black-out the cell and not worry about it.
        /// </summary>
        private void BlackOutShape(int col, int row, int radius)
        {
            //// TODO: Refactor so this circle is drawn from the same method the other circles are drawn from.

            Thread.Sleep(Math.Max(1, this.SleepTime) * 200); // Sleep before cleaning up the shape.

            var angle = 0;

            while (angle < radius * Math.PI)
            {
                var left = Math.Min(this.Width, Math.Max(0, col + (int)(Math.Cos(angle) * radius)));
                var top = Math.Min(this.Height, Math.Max(0, row + (int)(Math.Sin(angle) * radius)));

                lock (this.LockObj)
                {
                    Console.CursorLeft = left;
                    Console.CursorTop = top;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(' ');
                }

                angle++;
                Thread.Sleep(this.SleepTime);
            }
        }
    }
}