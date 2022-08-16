using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RightToAskClient.Helpers
{
    public static class Extensions
    {
        public static async Task PopGoToAsync(this Shell sh, string path)
        {
            var stackCount = sh.Navigation.NavigationStack.Count;
            for (int i = stackCount - 1; i > 0; i--)
            {
                sh.Navigation.RemovePage(sh.Navigation.NavigationStack[i]);
            }
            await sh.GoToAsync(path);
        }
        
        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? enumerable)
        {
            return !(enumerable?.Any() ?? false);

        }

        public static bool HasSameElements<T>(this IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var set1 = new HashSet<T>(list1);
            return set1.SetEquals(list2);
        }
        
        public static string JoinFilter(string separator, IEnumerable<string> strings)
        {
            return string.Join(separator, strings.Where(s => !string.IsNullOrEmpty(s)));
        }
        public static string JoinFilter(string separator, params string[] str)
        {
            return string.Join(separator, str?.Where(s => !string.IsNullOrEmpty(s)));
        }
    }
}
