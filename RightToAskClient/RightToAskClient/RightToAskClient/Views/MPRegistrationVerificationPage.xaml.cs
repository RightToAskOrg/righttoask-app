using System;
using System.Linq;
using RightToAskClient.Helpers;
using RightToAskClient.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MPRegistrationVerificationPage : ContentPage
    {
        private Entry[] VerifyCodeEntries;
        // Shell navigation requires a default constructor
        public MPRegistrationVerificationPage()
        {
            InitializeComponent();
            VerifyCodeEntries = CodeLayout.Children.OfType<Entry>().ToArray();
            DomainPicker.ItemsSource = ParliamentaryURICreator.ValidParliamentaryDomains;
        }

        private void ThatsMeRadioButton_OnTapped(object sender, EventArgs e)
        {
            ThatsMeRadioButton.IsChecked = true;
        }
        
        private void IAmAStafferRadioButton_OnTapped(object sender, EventArgs e)
        {
            IAmAStafferRadioButton.IsChecked = true;
        }

        private async void VerifyCodeEntries_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var key = e.NewTextValue;
            if (key.Length > 1)
            {
                // paste from the clipboard
                // Stupid xamarin, the clipboard returns me different strings when there is no string in the clipboard!!!
                string clipboardText = await Clipboard.GetTextAsync();
                try
                {
                    Int32.Parse(clipboardText);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    var newChar = key.Replace(e.OldTextValue, "");
                    //needs to consider 2 same chars situation
                    if (newChar.IsNullOrEmpty())
                        newChar = e.OldTextValue;
                    
                    (sender as Entry)!.Text = newChar;
                    return;
                }
                
                if (!clipboardText.IsNullOrEmpty())
                {
                    int[] digitArray = clipboardText.Select(c => int.Parse(c.ToString())).ToArray();
                    for (int i = 0; i < digitArray.Length; i++)
                    {
                        if(i >= 6) break;
                        VerifyCodeEntries[i].Text = "" + digitArray[i];
                    }

                    await Clipboard.SetTextAsync("");
                }
                
            }
            else
            {
                // type from the keyboard
                if (key.IsNullOrEmpty())
                {
                    //type "delete" Key: back forward
                    for (int i = 1; i < VerifyCodeEntries.Length; i++)
                    {
                        if (sender == VerifyCodeEntries[i])
                        {
                            VerifyCodeEntries[i - 1].Focus();  
                            VerifyCodeEntries[i - 1].CursorPosition = VerifyCodeEntries[i - 1].Text.Length; 
                        }
                    }
                }
                else
                {
                    //go forward
                    for (int i = 0; i < VerifyCodeEntries.Length - 1; i++)
                    {
                        if (sender == VerifyCodeEntries[i])
                        {
                            VerifyCodeEntries[i + 1].Focus();
                        }
                    }
                }
            }
        }

        private void SubmitButton_OnClicked(object sender, EventArgs e)
        {
            //combine the pin
            var vm = BindingContext as MPRegistrationVerificationViewModel;
            vm.MpRegistrationPin = string.Join("", VerifyCodeEntries.Select(entry => entry.Text));
            vm.SubmitMPRegistrationPinCommand.ExecuteAsync();
        }
    }
}