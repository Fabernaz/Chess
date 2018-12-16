using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    internal static class Extensions
    {
        internal static void Add<TKey, TValue>(this IDictionary<TKey, TValue> dict, KeyValuePair<TKey, TValue> pair)
        {
            dict.Add(pair.Key, pair.Value);
        }

        internal static void AddRange<T>(this ICollection<T> This, IEnumerable<T> toAdd)
        {
            if (toAdd == null)
                return;

            foreach (var a in toAdd)
                This.Add(a);
        }

        internal static void RemoveRange<T>(this ICollection<T> This, IEnumerable<T> toRemove)
        {
            if (toRemove == null)
                return;

            foreach (var a in toRemove)
                This.Remove(a);
        }
    }

    public static class Guard
    {
        public static void ArgumentNotNull<T>(T argument, string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);
        }
    }
}
