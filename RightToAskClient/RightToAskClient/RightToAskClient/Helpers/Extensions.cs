using System;
using System.Collections.Generic;
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
    }
}
