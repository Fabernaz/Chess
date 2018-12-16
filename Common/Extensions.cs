using System.Collections;
using System.Collections.Generic;

namespace Common
{
    public static class Extensions
    {
        public static void AddMany<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
                list.Add(item);
        }

        public static void AddMany(this IList list, IEnumerable items)
        {
            foreach (var item in items)
                list.Add(item);
        }
    }
}
