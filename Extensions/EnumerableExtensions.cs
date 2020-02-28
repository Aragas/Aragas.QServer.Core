namespace System.Collections.Generic
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IterationElement<T>> Detailed<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return Iterator();

            IEnumerable<IterationElement<T>> Iterator()
            {
                using var enumerator = source.GetEnumerator();
                var isFirst = true;
                var hasNext = enumerator.MoveNext();
                for (var index = 0; hasNext; index++)
                {
                    var current = enumerator.Current;
                    hasNext = enumerator.MoveNext();
                    yield return new IterationElement<T>(index, current, isFirst, !hasNext);
                    isFirst = false;
                }
            }
        }

        public struct IterationElement<T>
        {
            public int Index { get; }
            public bool IsFirst { get; }
            public bool IsLast { get; }
            public T Value { get; }

            public IterationElement(int index, T value, bool isFirst, bool isLast)
            {
                Index = index;
                IsFirst = isFirst;
                IsLast = isLast;
                Value = value;
            }

            public override bool Equals(object obj)
            {
                if (obj is IterationElement<T> iterationElement)
                {
                    var equals = Index.Equals(iterationElement.Index) && IsFirst.Equals(iterationElement.IsFirst) && IsLast.Equals(iterationElement.IsLast);
                    return Value != null ? equals && Value.Equals(iterationElement.Value) : equals;
                }

                return false;
            }

            public override int GetHashCode() => HashCode.Combine(Index, IsFirst, IsLast, Value);

            public static bool operator ==(IterationElement<T> left, IterationElement<T> right)
            {
                var equals = left.Index == right.Index && left.IsFirst == right.IsFirst && left.IsLast == right.IsLast;
                return (left.Value != null && right.Value != null) ? (equals && ReferenceEquals(left.Value, right.Value)) : equals;
            }

            public static bool operator !=(IterationElement<T> left, IterationElement<T> right)
            {
                return !(left == right);
            }
        }
    }
}