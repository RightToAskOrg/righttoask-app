using System;
using Org.BouncyCastle.Bcpg;
using RightToAskClient.CryptoUtils;
using RightToAskClient.Models;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
	public partial class TodoItemPage : ContentPage
	{
		bool isNewItem;

		public TodoItemPage (bool isNew = false)
		{
			InitializeComponent ();
			isNewItem = isNew;
		}

		async void OnSaveButtonClicked (object sender, EventArgs e)
		{
			var newRegistration = (Registration)BindingContext;
			Result<bool> httpResponse;
			httpResponse = (await App.RegItemManager.SaveTaskAsync (newRegistration, isNewItem));
			if(String.IsNullOrEmpty(httpResponse.Err))
			{
				if (httpResponse.Ok)
				{
					reportLabel.Text = "Server signature successfully verified.";
					// await Navigation.PopAsync();
				}
				else
				{
					reportLabel.Text = "Server signature verification failed";
				}
			}
			else
			{
				reportLabel.Text = "Server connection error" + httpResponse?.Err;
			}
		}

		async void OnCancelButtonClicked (object sender, EventArgs e)
		{
			await Navigation.PopAsync ();
		}
		private bool VerifyServerSignature()
		{
			return true;
			// if (SignatureService.VerifySignature(message, signature, SignatureService.serverPublicKey))	
		}	
	}
}
