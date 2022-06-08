using System;
using System.Collections.Generic;
using System.Linq;
using RightToAskClient.Models;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class MPRegistrationVerificationViewModel : BaseViewModel
    {
        // #region Properties
        public string Title = "Select the MP you are or you work for";
        
        // If registered as representing an MP.
        // 
        private string _MPrepresentingName = "This is a test MP name";
        public string MPRepresentingName
        {
            get => _MPrepresentingName;
            private set => SetProperty(ref _MPrepresentingName, value);
        }

        private SelectableList<MP> _selectableMPList = new SelectableList<MP>(new List<MP>(), new List<MP>());

        public SelectableList<MP> SelectableMPList
        {
            get => _selectableMPList;
            set => SetProperty(ref _selectableMPList, value);
        }
        
        // If the person has stated that they are an MP or staffer, return that one (there should be only one).
        // Otherwise, a new/blank one.
        private MP _MPRepresenting = new MP();
        public MP MPRepresenting
        {
            get => _MPRepresenting;
            private set => SetProperty(ref _MPRepresenting, value);
            // get => _selectableMPList.SelectedEntities.Any() ? _selectableMPList.SelectedEntities.First() : new MP();
        }
        private List<MP> _selectedMPsForRegistration = new List<MP>();
        // This is initialized properly in the constructor, if we're looking at our own account.

        private string _mpRegistrationPIN = "";

        public string MPRegistrationPIN
        {
            get => _mpRegistrationPIN;
            set => SetProperty(ref _mpRegistrationPIN, value);
        }

        public bool ReturnToAccountPage = false;

        // constructor
        public MPRegistrationVerificationViewModel()
        {
            // TODO This page is responsible for storing MP reg somehow:
            // FIXME. We want to store the selected MP to some local variable before we do anything else.
            // Indeed, we should probably change the type so it's just an MP rather than a SelectableList.
            // StoreMPRegistration();
            MessagingCenter.Subscribe<SelectableListViewModel, MP>(this, "ReturnToAccountPage", (sender, arg) =>
            {
                MPRepresenting = arg;
                ReturnToAccountPage = true;
                MessagingCenter.Unsubscribe<RegistrationViewModel>(this, "ReturnToAccountPage");
            });

            SendMPVerificationEmailCommand = new Command(() => { SendMPRegistrationToServer(); });
            SubmitMPRegistrationPINCommand = new AsyncCommand(async () =>
            {
                var success = SendMPRegistrationPINToServer();
                if (success)
                {
                    SaveMPRegistrationToPreferences();
                }
                // navigate back to account page
                if (ReturnToAccountPage)
                {
                    await App.Current.MainPage.Navigation.PopToRootAsync();
                }
            });
            /*
            RegisterOrgButtonCommand = new Command(() =>
            {
                RegisterOrgButtonText = "Registering not implemented yet";
            });
            */
        }
        
        // commands
        public Command SendMPVerificationEmailCommand { get; }
        public IAsyncCommand SubmitMPRegistrationPINCommand { get; }
        
        
        
        // methods
        
        private string _registerMPReportLabelText = "";
        public string RegisterMPReportLabelText
        {
            get
            {
                var selected = _selectableMPList.SelectedEntities;
                if (selected.Count() > 0)
                {
                    _registerMPReportLabelText = "You must select exactly one MP. Please try again.";
                } else if (selected.Count() == -1)
                {
                    _registerMPReportLabelText = "";
                }
                else
                {
                    _registerMPReportLabelText = "You selected" + selected.First().ShortestName;
                }
                return _registerMPReportLabelText;
            }
            // private set => SetProperty(ref _registerMPReportLabelText, value);
        }

        private bool _showRegisterMPReportLabel = false;
        public bool ShowRegisterMPReportLabel
        {
            get => _showRegisterMPReportLabel;
            set => SetProperty(ref _showRegisterMPReportLabel, value);
        }
        
        private void SendMPRegistrationToServer()
        {
            Console.WriteLine("The MP registration to send to the server is "+MPRepresentingName);
        }

        // TODO - save to preferences.
        private void SaveMPRegistrationToPreferences()
        {
            Console.WriteLine("The MP registration to save to preferences is "+MPRepresentingName);
        }

        private void StoreMPRegistration()
        {
            List<MP> selections = _selectableMPList.SelectedEntities as List<MP>;
            int numSelections = selections.Any() ? selections.Count() : 0;
            string msg = numSelections switch
            {
                0 => "Oops, no selections",
                1 => selections[0].ShortestName,
                _ => "You can only register as one MP at a time",
            };
                
            Console.WriteLine("The MP registration to save to preferences is "+msg);
            MPRepresentingName = msg; 
        }

        
        private bool SendMPRegistrationPINToServer()
        {
            Console.WriteLine("The PIN to send to the server is "+MPRegistrationPIN);
            return true;
        }
    }
}