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
        
        // If registered as representing an MP.
        // 
        private string _MPrepresentingName = "";
        public string MPRepresentingName
        {
            get => _MPrepresentingName;
            private set => SetProperty(ref _MPrepresentingName, value);
        }

        private SelectableList<MP> _selectableMPList = new SelectableList<MP>(new List<MP>(), new List<MP>());
        
        // If the person has stated that they are an MP or staffer, return that one (there should be only one).
        // Otherwise, a new/blank one.
        public MP MPRepresenting
        {
            get => _selectableMPList.SelectedEntities.Any() ? _selectableMPList.SelectedEntities.First() : new MP();
        }
        private List<MP> _selectedMPsForRegistration = new List<MP>();
        // This is initialized properly in the constructor, if we're looking at our own account.

        private string _mpRegistrationPIN = "";

        public string MPRegistrationPIN
        {
            get => _mpRegistrationPIN;
            set => SetProperty(ref _mpRegistrationPIN, value);
        }
        // constructor
        public MPRegistrationVerificationViewModel()
        {
            // TODO This page is responsible for storing MP reg somehow:
            // StoreMPRegistration();
       
            SendMPVerificationEmailCommand = new Command(() => { SendMPRegistrationToServer(); });
            SubmitMPRegistrationPINCommand = new Command(() =>
            {
                var success = SendMPRegistrationPINToServer();
                if (success)
                {
                    SaveMPRegistrationToPreferences();
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
        public Command SubmitMPRegistrationPINCommand { get; }
        
        
        
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