using System.Collections;

namespace Vookaba.Services.DTO
{
    public class PartialList<T> : IEnumerable<T>
    {
        public PartialList(int totalCount, IList<T> currentItems)
        {
            TotalCount = totalCount;
            CurrentItems = currentItems;
        }

        public int TotalCount { get; set; }

        public IList<T> CurrentItems { get; set; }

        public IEnumerator<T> GetEnumerator() => CurrentItems.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => CurrentItems.GetEnumerator();
    }
}
