using System.Collections;

namespace GlideNGlow.Common.Extensions;

public static class GroupExtensions
{
    public static IGrouping<TKey, TElement> OrderGroupingBy<TKey, TElement, TOrderKey>(this IGrouping<TKey, TElement> grouping,
        Func<TElement, TOrderKey> orderKeySelector)
    {
        return new GroupingImpl<TKey, TElement>(grouping.Key, grouping.OrderBy(orderKeySelector));
    }
    
    public static IGrouping<TKey, TElement> OrderGrouping<TKey, TElement>(this IGrouping<TKey, TElement> grouping)
    {
        return new GroupingImpl<TKey, TElement>(grouping.Key, grouping.Order());
    }
    
    public static IGrouping<TKey, TElement> OrderGrouping<TKey, TElement>(this IGrouping<TKey, TElement> grouping, IComparer<TElement> comparer)
    {
        return new GroupingImpl<TKey, TElement>(grouping.Key, grouping.Order(comparer));
    }

    private class GroupingImpl<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly List<TElement> _elements;

        public TKey Key { get; }

        internal GroupingImpl(TKey key, IEnumerable<TElement> elements)
        {
            Key = key;
            _elements = elements.ToList();
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}