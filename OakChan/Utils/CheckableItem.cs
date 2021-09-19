namespace OakChan.Utils
{
    public class CheckableItem<T>
    {
        public T Item { get; set; }

        public bool IsChecked { get; set; }

        public CheckableItem() { }
        public CheckableItem(T item) : this(item, false) { }

        public CheckableItem(T item, bool isChecked)
        {
            Item = item;
            IsChecked = isChecked;
        }
    }
}
