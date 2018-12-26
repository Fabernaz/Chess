using System.Collections;
using System.Collections.Generic;

namespace Common
{
    public static class Extensions
    {
        public static void Add<TKey, TValue>(this IDictionary<TKey, TValue> dict, KeyValuePair<TKey, TValue> pair)
        {
            dict.Add(pair.Key, pair.Value);
        }

        public static void AddRange<T>(this ICollection<T> This, IEnumerable<T> toAdd)
        {
            if (toAdd == null)
                return;

            foreach (var a in toAdd)
                This.Add(a);
        }

        public static void RemoveRange<T>(this ICollection<T> This, IEnumerable<T> toRemove)
        {
            if (toRemove == null)
                return;

            foreach (var a in toRemove)
                This.Remove(a);
        }
    }
}
