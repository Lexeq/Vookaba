using System.Collections;
using System.Collections.Generic;

namespace Vookaba.Services.DTO
{
    public class PartialList<T> : IEnumerable<T>
    {
        public int TotalCount { get; set; }

        public IList<T> CurrentItems { get; set; }

        public IEnumerator<T> GetEnumerator() => CurrentItems.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => CurrentItems.GetEnumerator();
    }
}
