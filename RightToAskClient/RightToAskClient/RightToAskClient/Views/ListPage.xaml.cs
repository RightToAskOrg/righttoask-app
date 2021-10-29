﻿using System;
using RightToAskClient.Models;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
	public partial class ListPage : ContentPage
	{
		public ListPage ()
		{
			InitializeComponent ();
		}

		protected async override void OnAppearing ()
		{
			base.OnAppearing ();
			var httpResponse = (await App.RegItemManager.GetTasksAsync ());
			if (httpResponse == null)
			{
				listView.Header = "Error reaching server. Check your Internet connection.";
			} 
			else if (String.IsNullOrEmpty(httpResponse.Err))
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
            await Navigation.PushAsync(new TodoItemPage(true)
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
	        
            await Navigation.PushAsync(new TodoItemPage
            {
                BindingContext = reg 
            });
		}
	}
}
