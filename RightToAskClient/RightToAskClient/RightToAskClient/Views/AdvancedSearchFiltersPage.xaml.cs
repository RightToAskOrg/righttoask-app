using System;
using RightToAskClient.Models;
using RightToAskClient.ViewModels;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdvancedSearchFiltersPage : ContentPage
    {
        public AdvancedSearchFiltersPage(FilterChoices FilterChoice)
        {
            InitializeComponent();
            BindingContext = new FilterViewModel(FilterChoice);
        }  
    }
}