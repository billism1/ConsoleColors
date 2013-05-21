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

        private delegate ConsoleColor GetConsoleColor();

        private delegate char GetChar();

        public override void Go()
        {
            const int Radius = 6;

            // Set initial location
            var col = this.rand.Next(Radius * 2, this.Width - Radius);
            var row = this.rand.Next(0 + Radius, this.Height - Radius);

            while (this.Run)
            {
                // Draw circle
                this.DrawCircle(
                    col,
                    row,
                    Radius,
                    () => (ConsoleColor)this.rand.Next(0, MaxColorEnum),
                    () => (char)this.rand.Next(32, 126)); // Printable ascii latters

                // Start circle deletion thread if flag is set
                if (this.timedDelete)
                {
                    var col1 = col;
                    var row1 = row;
                    var clearThread = new Thread(() => this.BlackOutShape(col1, row1, Radius));
                    clearThread.Start();
                }

                // Get new location
                col = Math.Min(this.Width - Radius, Math.Max(Radius, col + this.rand.Next(-10, 11)));
                row = Math.Min(this.Height - Radius, Math.Max(Radius, row + this.rand.Next(-10, 11)));

                // Reset random object.
                this.rand = new Random(int.Parse(Regex.Replace(Guid.NewGuid().ToString(), "[^\\d]", string.Empty).Substring(0, 4)));
            }
        }

        private void DrawCircle(int col, int row, int radius, GetConsoleColor getConsoleColor, GetChar getChar)
        {
            var angle = 0;
            var backgroundColor = getConsoleColor();

            while (angle < radius * Math.PI)
            {
                var left = Math.Min(this.Width, Math.Max(0, col + (int)(Math.Cos(angle) * radius)));
                var top = Math.Min(this.Height, Math.Max(0, row + (int)(Math.Sin(angle) * radius)));

                lock (this.LockObj)
                {
                    Console.CursorLeft = left;
                    Console.CursorTop = top;
                    Console.BackgroundColor = backgroundColor;
                    Console.Write(getChar());
                }

                angle++;
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
            Thread.Sleep(Math.Max(1, this.SleepTime) * 200); // Sleep before cleaning up the shape.
            this.DrawCircle(col, row, radius, () => ConsoleColor.Black, () => ' ');
        }
    }
}