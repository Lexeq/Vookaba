namespace OakChan.ViewModels
{
    public class ThreadViewModelBase
    {
        public string BoardKey { get; set; }

        public int ThreadId { get; set; }

        public string Subject { get; set; }

        public bool IsPinned { get; set; }

        public bool IsReadOnly { get; set; }
    }
}