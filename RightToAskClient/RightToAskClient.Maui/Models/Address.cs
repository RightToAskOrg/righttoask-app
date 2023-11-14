using CommunityToolkit.Mvvm.ComponentModel;

namespace RightToAskClient.Maui.Models
{
    public class Address : ObservableObject
    {
        private string _streetNumberAndName = "";
        public string StreetNumberAndName
        {
            get => _streetNumberAndName;
            set
            {
                if (!string.IsNullOrEmpty(_streetNumberAndName))
                {
                    var changed = SetProperty(ref _streetNumberAndName, value);
                    if (changed)
                    {
                        IndividualParticipant.getInstance().AddressUpdated = true;
                    }
                }
                else
                {
                    _ = SetProperty(ref _streetNumberAndName, value);
                }
            }
        }
        private string _cityOrSuburb = "";
        public string CityOrSuburb
        {
            get => _cityOrSuburb;
            set
            {
                if (!string.IsNullOrEmpty(_cityOrSuburb))
                {
                    var changed = SetProperty(ref _cityOrSuburb, value);
                    if (changed)
                    {
                        IndividualParticipant.getInstance().AddressUpdated = true;
                    }
                }
                else
                {
                    _ = SetProperty(ref _cityOrSuburb, value);
                }
            }
        }
        private string _postCode = "";
        public string Postcode
        {
            get => _postCode;
            set
            {
                if (!string.IsNullOrEmpty(_postCode))
                {
                    var changed = SetProperty(ref _postCode, value);
                    if (changed)
                    {
                        IndividualParticipant.getInstance().AddressUpdated = true;
                    }
                }
                else
                {
                    _ = SetProperty(ref _postCode, value);
                }
            }
        }

        public JOSResult<bool> SeemsValid()
        {
            var err = "";
            if (string.IsNullOrWhiteSpace(StreetNumberAndName))
            {
                err += "Please enter a street number and name.";
            }
            if (string.IsNullOrWhiteSpace(CityOrSuburb))
            {
                err += "Please enter a city or suburb.";
            }
            if (string.IsNullOrWhiteSpace(Postcode))
            {
                err += "Please enter a city or suburb.";
            }

            if (string.IsNullOrEmpty(err))
            {
                return new SuccessResult<bool>(true);
            }

            return new ErrorResult<bool>(err);
        }

        public override string ToString()
        {
            return StreetNumberAndName + ", "
                                       + CityOrSuburb + " "
                                       + Postcode;

        }
    }
}