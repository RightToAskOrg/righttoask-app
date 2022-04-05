﻿using RightToAskClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdvancedSearchFiltersPage : ContentPage
    {
        ReadingPageViewModel vm;
        public AdvancedSearchFiltersPage()
        {
            vm = new ReadingPageViewModel();
            BindingContext = vm;
            InitializeComponent();
        }

        public AdvancedSearchFiltersPage(ReadingPageViewModel readingPageVM)
        {
            vm = readingPageVM;
            vm.Title = "Advanced Search Filters";
            BindingContext = vm;
        }
    }
}