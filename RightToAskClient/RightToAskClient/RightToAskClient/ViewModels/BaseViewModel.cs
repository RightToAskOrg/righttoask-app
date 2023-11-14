using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Maui.Extensions;
using RightToAskClient.Resx;
using RightToAskClient.Views.Popups;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using CommunityToolkit.Mvvm.Input;

namespace RightToAskClient.ViewModels
{
    public class BaseViewModel : ObservableObject
    {
        public ContentPage Page { get; set; } = new ContentPage();

        // constructor
        public BaseViewModel()
        {
            PopupLabelText = "TestText";
            HomeButtonCommand = new AsyncRelayCommand (async () =>
            {
                var popup = new TwoButtonPopup(AppResources.GoHomePopupTitle, AppResources.GoHomePopupText,
                    AppResources.CancelButtonText, AppResources.GoHomeButtonText, false);
                //TODO:
                //var popupResult = await Application.Current.MainPage.Navigation.ShowPopupAsync(popup);
                //if (popup.HasApproved(popupResult))
                //{
                //    await Application.Current.MainPage.Navigation.PopToRootAsync();
                //}
            });
            //TODO:
            //InfoPopupCommand = new AsyncRelayCommand(async () =>
            //{
            //    //Page.Navigation.ShowPopup(new InfoPopup());
            //    var popup = new InfoPopup(PopupHeaderText,PopupLabelText, AppResources.OKText);
            //    _ = await Application.Current.MainPage.Navigation.ShowPopupAsync(popup);
            //});
            //TCCommand = new AsyncRelayCommand(async () =>
            //{
            //    var popup = new TermAndConditionPopup();
            //    _ = await Application.Current.MainPage.Navigation.ShowPopupAsync(popup);
            //});
        }

        private string _title = string.Empty;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _popupHeaderText = "";

        public string PopupHeaderText
        {
            get => _popupHeaderText;
            set => SetProperty(ref _popupHeaderText, value);
        }

        private string _popupLabelText = "";

        public string PopupLabelText
        {
            get => _popupLabelText;
            set => SetProperty(ref _popupLabelText, value);
        }

        private string _reportLabelText = "";

        public string ReportLabelText
        {
            get => _reportLabelText;
            set
            {
                SetProperty(ref _reportLabelText, value);
                ReportLabelIsVisible = !String.IsNullOrEmpty(_reportLabelText);
            }
        }

        private bool _reportLabelIsVisible;
        public bool ReportLabelIsVisible
        {
            get => _reportLabelIsVisible;
            set => SetProperty(ref _reportLabelIsVisible, value); 
        }

        // commands
        public IAsyncRelayCommand HomeButtonCommand { get; }
        public IAsyncRelayCommand InfoPopupCommand { get; }
        public IAsyncRelayCommand TCCommand { get; }
    }
}