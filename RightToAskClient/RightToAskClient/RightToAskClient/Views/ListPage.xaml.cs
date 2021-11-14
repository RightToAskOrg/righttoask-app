using System;
using RightToAskClient.Models;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
	public partial class ListPage
	{
		public ListPage ()
		{
			InitializeComponent ();
			// DoCryptoSigningTest();
		}

	
		protected async override void OnAppearing ()
		{
			base.OnAppearing ();
			var httpResponse = await App.RegItemManager.GetTasksAsync ();
			// if (httpResponse == null)
			//{
			// 	listView.Header = "Error reaching server. Check your Internet connection.";
			// } 
			if (String.IsNullOrEmpty(httpResponse.Err))
			{
				listView.ItemsSource = httpResponse.Ok;
			}
			else
			{
				listView.Header = "Error reaching server: "+httpResponse.Err;
			}
		}

	

		async void OnAddItemClicked (object sender, EventArgs e)
		{
            await Navigation.PushAsync(new TodoItemPage()
            {
                BindingContext = new Registration() 
                {
                    // ID = Guid.NewGuid().ToString()
                }
            });
		}

		async void OnItemSelected (object sender, SelectedItemChangedEventArgs e)
		{
	        var reg = new Registration()
	        {
				uid   = e.SelectedItem as string
	        };
	        
            await Navigation.PushAsync(new TodoItemPage()
            {
                BindingContext = reg 
            });
		}
		
		// TODO: Put into a proper unit test
		/*
		private void DoCryptoSigningTest()
		{
			string message = TestMessage.Text;

			var signature = SignatureService.SignMessage(message);
			TestSig.Text = signature.ToString();

			if (SignatureService.VerifySignature(message, signature, SignatureService.myPublicKey))
				// if (sigService.VerifySignature("NotTheMessage", signature, Constants.myPublicKey))
			{
				SigningTestOutcome.Text = "Successful verification";
			}
			else
			{
				SigningTestOutcome.Text = "Failed verification";
			}
		}
		*/

	}
}
