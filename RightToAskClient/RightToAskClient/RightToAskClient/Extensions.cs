using System.Collections.Generic;
using System.Linq;

namespace RightToAskClient
{
    public static class Extensions 
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return !(enumerable?.Any() ?? false);

        }
    }
    
}