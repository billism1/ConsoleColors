namespace ConsoleColors
{
    public abstract class ConsoleWriterBase
    {
        public object LockObj { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int SleepTime { get; set; }

        public bool Run { get; set; }

        public abstract void Go();
    }
}
