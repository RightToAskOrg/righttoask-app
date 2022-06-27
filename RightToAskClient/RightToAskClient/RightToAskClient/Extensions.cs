using System.Collections.Generic;
using System.Linq;

namespace RightToAskClient
{
    public static class Extensions 
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? enumerable)
        {
            return !(enumerable?.Any() ?? false);

        }

        public static bool HasSameElements<T>(this IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var set1 = new HashSet<T>(list1);
            return set1.SetEquals(list2);
        }
    }
    
}