using System;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/*
 * This page allows a person to find which electorates they live in,
 * and hence which MPs represent them.
 *
 * This is used in two possible places:
 * (1) if the person clicks on 'My MP' when setting question metadata,
 * we need to know who their MPs are. After this page,
 * there will be a list of MPs loaded for them to choose from.
 * This is indicated by setting LaunchMPsSelectionPageNext to true.
 * 
 * (2) if the person tries to vote or post a question.
 * In this case, they have generated a name via RegisterPage1
 * and can skip this step (so showSkip should be set to true).
 * And we don't follow with a list of MPs for them to chose from.
 */
namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FindMPsPage : ContentPage
    {
        private string _doneButtonText;
        public FindMPsPage()
        {
            InitializeComponent();
            _doneButtonText = AppResources.SaveButtonText;
            
            HighlightText(SelectEleButton, SelectEleButtonUnderLine);
            UnHighlightText(TypeAddButton, TypeAddButtonUnderLine);
        }
        public FindMPsPage(Registration registration)
        {
            InitializeComponent();
            BindingContext = new FindMPsViewModel(registration);
            _doneButtonText = AppResources.Continue;
        }

        protected override void OnAppearing()
        {
            AutomationProperties.SetIsInAccessibleTree(DoneButton, true);
            AutomationProperties.SetHelpText(DoneButton, _doneButtonText);
            
            DoneButton.Text = _doneButtonText;
        }

        private void SelectEleButton_OnClicked(object sender, EventArgs e)
        {
            HighlightText(SelectEleButton, SelectEleButtonUnderLine);
            UnHighlightText(TypeAddButton, TypeAddButtonUnderLine);
        }

        private void TypeAddButton_OnClicked(object sender, EventArgs e)
        {
            UnHighlightText(SelectEleButton,SelectEleButtonUnderLine);
            HighlightText(TypeAddButton, TypeAddButtonUnderLine);
        }

        private void HighlightText(Button btn, BoxView bv)
        {
            btn.TextColor = (Color)Application.Current.Resources["ButtonColor"];
            bv.Color = (Color)Application.Current.Resources["ButtonColor"];
        }
        private void UnHighlightText(Button btn, BoxView bv)
        {
            btn.TextColor = (Color)Application.Current.Resources["MediumDarkShadeOfGray"];
            bv.Color = Color.Transparent;
        }
    }
}