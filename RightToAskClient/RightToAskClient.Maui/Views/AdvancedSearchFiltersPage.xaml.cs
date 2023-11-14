using System;
using RightToAskClient.Maui.Models;
using RightToAskClient.Maui.ViewModels;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace RightToAskClient.Maui.Views
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