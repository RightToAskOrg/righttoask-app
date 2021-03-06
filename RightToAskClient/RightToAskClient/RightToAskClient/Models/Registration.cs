using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using RightToAskClient.Annotations;
using RightToAskClient.Models.ServerCommsData;
using Xamarin.CommunityToolkit.ObjectModel;

namespace RightToAskClient.Models
{
    public class Registration : ObservableObject
    {
        // TODO: Move MP / staffer reg in here.
        public string display_name { get; set; } = "";
        public string public_key { get; set; } = "";
        private string state { get; set; } = "";

        public string State
        {
            get
            {
                if (0 <= _selectedStateAsIndex && _selectedStateAsIndex < ParliamentData.StatesAndTerritories.Count)
                {
                     return ParliamentData.StatesAndTerritories[_selectedStateAsIndex];       
                }

                return "";
            }
            private set => state = value;
        }

        public string uid { get; set; } = "";

        private int _selectedStateAsIndex = -1;
        
        // State, as an index into ParliamentData.StatesAndTerritories
        // Setting this value also updates the chambers.
        // TODO: Should it clear electorates that are not in the relevant state any more?
        public int SelectedStateAsIndex
        {
            get => _selectedStateAsIndex;
            set => SetProperty(ref _selectedStateAsIndex, value); 
        }
        private List<ElectorateWithChamber> _electorates = new List<ElectorateWithChamber>();

        public Registration()
        {
        }

        public Registration(ServerUser input)
        {
            display_name = input.display_name ?? "";
            public_key = input.public_key ?? "";
            state = input.state ?? "";
            uid = input.uid ?? "";
            _electorates = (input.electorates ?? new ObservableCollection<ElectorateWithChamber>()).ToList();
            // TODO add badges
            // badges = input.badges;
        }

        // Whenever electorates are updated (by this and by AddElectorateRemoveDuplicates), we need to tell the 
        // Filters to update the list of 'my MPs'.
        public ObservableCollection<ElectorateWithChamber> Electorates 
        {
            get { return new ObservableCollection<ElectorateWithChamber>(_electorates); }
            set 
            {
                SetProperty(ref _electorates, value.ToList());
                App.ReadingContext.Filters.UpdateMyMPLists();
            } 
        }

        /* Accept a new electorate and chamber, remove any earlier ones that are inconsistent.
		 * Note: this assumes that nobody is ever represented in two different regions in the one
		 * chamber. This is true throughout Aus, but may be untrue elsewhere. Of course, each region
		 * may have multiple representatives.
		 * Inserting it at the beginning rather than adding at the end is a bit of a hack to
		 * put the Senators last (they're computed first).
		 */
        public void AddElectorateRemoveDuplicates(ElectorateWithChamber newElectorate)
        {
            // potentially remove any duplicates
            _electorates.RemoveAll(e => e.chamber == newElectorate.chamber);
            _electorates.Insert(0, newElectorate);
            OnPropertyChanged("electorates");
            App.ReadingContext.Filters.UpdateMyMPLists();
        }

        private void Electorates_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        // TODO: Perhaps put this in ParliamentData and let it take _electorates as
        // an input.
        public string findElectorateGivenPredicate(Predicate<ElectorateWithChamber> func)
        {
            var electoratePair = _electorates.Find(func);
            if (electoratePair is null)
            {
                return "";
            }

            return electoratePair.region;
        }
        public Result<bool> IsValid()
        {
            List<string> errorFields = new List<string>();

            foreach (PropertyInfo prop in typeof(Registration).GetProperties())
            {
                var value = prop.GetValue(this, null);
                if (value is null || String.IsNullOrWhiteSpace(value.ToString()))
                {
                    errorFields.Add(prop.Name);
                }
            }

            if (errorFields.IsNullOrEmpty() || errorFields.SequenceEqual(new List<string> { "electorates" }))
            {
                return new Result<bool>() { Ok = true };
            }
            return new Result<bool>()
            {
                Err = "Please complete " + String.Join(" and ", errorFields)
            };
        }

        /*
        public void UpdateMultipleElectoratesRemoveDuplicates(ObservableCollection<ElectorateWithChamber> value)
        {
            throw new NotImplementedException();
        }
        */

        public bool Validate()
        {
            bool isValid = false;
            // needs to have a uid and public key
            if (!string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty(public_key) && !string.IsNullOrEmpty(State))
            {
                if (Electorates.Any())
                {
                    bool hasInvalidElectorate = false;
                    foreach(ElectorateWithChamber e in Electorates)
                    {
                        bool validElectorate = e.Validate();
                        if (!validElectorate)
                        {
                            hasInvalidElectorate = true;
                        }
                    }
                    if (hasInvalidElectorate)
                    {
                        isValid = false;
                    }
                    else
                    {
                        isValid = true;
                    }
                }
                else
                {
                    isValid = true;
                }
            }
            return isValid;
        }
    }
}
