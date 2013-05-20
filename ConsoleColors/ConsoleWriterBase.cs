namespace ConsoleColors
{
    public abstract class ConsoleWriterBase
    {
        protected ConsoleWriterBase(object lockObj, int width, int height, int sleepTime)
        {
            this.LockObj = lockObj;
            this.Width = width;
            this.Height = height;
            this.SleepTime = sleepTime;
            this.Run = true;
        }

        public object LockObj { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int SleepTime { get; set; }

        public bool Run { get; set; }

        public abstract void Go();
    }
}
