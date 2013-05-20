﻿namespace ConsoleColors
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading;

    public class CrazyColorsConsoleWriter : ConsoleWriterBase
    {
        private bool printStatistics;

        private int maxColorEnum = 15;

        /// <summary>
        /// Use decimal chars from a newly generated guid to generate a salt value. Otherwise, pseudo-random 
        /// values won't really be very random if starting multiple threads at once.
        /// </summary>
        private Random rand = new Random(int.Parse(Regex.Replace(Guid.NewGuid().ToString(), "[^\\d]", string.Empty).Substring(0, 4)));

        private List<string> products = new List<string>();

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
                    Console.ForegroundColor = (ConsoleColor)rand.Next(0, maxColorEnum);
                    Console.BackgroundColor = (ConsoleColor)rand.Next(0, maxColorEnum);
                    Console.CursorLeft = rand.Next(0, this.Width);
                    Console.CursorTop = rand.Next(0, this.Height);
                    Console.Write((char)(rand.Next(32, 126))); // Printable ascii latters
                }

                if (this.printStatistics)
                    PrinstStatistics(start, cellCount);

                Thread.Sleep(this.SleepTime);
            }
        }

        private void PrinstStatistics(DateTime start, int cellCount)
        {
            var product = Console.CursorLeft.ToString() + ":" + Console.CursorTop.ToString();

            if (!products.Contains(product))
            {
                products.Add(product);
            }
            this.Run = this.Run && products.Count < cellCount;

            lock (this.LockObj)
            {
                Console.CursorLeft = 0;
                Console.CursorTop = this.Height - 1;
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("Time Lapsed: " + (DateTime.Now - start));
                Console.WriteLine(products.Count.ToString() + " of " + cellCount.ToString() + " cells populated.");
            }
        }
    }
}