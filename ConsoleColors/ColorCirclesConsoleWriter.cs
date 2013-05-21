namespace ConsoleColors
{
    using System;
    using System.Collections.Generic;
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

            var cellsTraveled = 0;

            var coordinates = new List<Tuple<int, int>>();

            while (this.Run)
            {
                var left = Math.Min(this.Width, Math.Max(0, col + (int)(Math.Cos(angle) * Radius)));
                var top = Math.Min(this.Height, Math.Max(0, row + (int)(Math.Sin(angle) * Radius)));
                coordinates.Add(new Tuple<int, int>(left, top));

                lock (this.LockObj)
                {
                    Console.CursorLeft = left;
                    Console.CursorTop = top;
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
                    col = Math.Min(this.Width - Radius, Math.Max(Radius, col + this.rand.Next(-10, 11)));
                    row = Math.Min(this.Height - Radius, Math.Max(Radius, row + this.rand.Next(-10, 11)));
                    backgroundColor = (ConsoleColor)this.rand.Next(0, MaxColorEnum);
                    this.rand = new Random(int.Parse(Regex.Replace(Guid.NewGuid().ToString(), "[^\\d]", string.Empty).Substring(0, 4)));

                    if (this.timedDelete)
                    {
                        var savedCoordinates = new List<Tuple<int, int>>(coordinates); // Create new list so we can clear the permanent one.
                        var clearThread = new Thread(() => this.BlackOutShape(savedCoordinates));
                        clearThread.Start();
                    }

                    coordinates.Clear();
                }

                Thread.Sleep(this.SleepTime);
            }
        }

        /// <summary>
        /// Called inside a thread to delete a previously drawn shape.
        /// We can't actually detect if a cell has been overwritten (by another thread's writer) since the shape was made, so the 
        /// next best thing is to black-out the cell and not worry about it.
        /// </summary>
        /// <param name="coordinates"></param>
        private void BlackOutShape(IEnumerable<Tuple<int, int>> coordinates)
        {
            Thread.Sleep(Math.Max(1, this.SleepTime) * 100); // Sleep before cleaning up the shape.

            foreach (var coordinate in coordinates)
            {
                lock (this.LockObj)
                {
                    Console.CursorLeft = coordinate.Item1;
                    Console.CursorTop = coordinate.Item2;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(' ');
                }

                Thread.Sleep(this.SleepTime);
            }
        }
    }
}