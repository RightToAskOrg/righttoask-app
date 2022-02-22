using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RightToAskClient.Annotations;
using Xamarin.CommunityToolkit.ObjectModel;

namespace RightToAskClient.Models
{
    public class Address : ObservableObject
    {
        private string _streetNumberAndName = "";
        public string StreetNumberAndName
        {
            get => _streetNumberAndName;
            set => SetProperty(ref _streetNumberAndName, value);
        }
        private string _cityOrSuburb = "";
        public string CityOrSuburb
        {
            get => _cityOrSuburb;
            set => SetProperty(ref _cityOrSuburb, value);
        }
        private string _postCode = "";
        public string Postcode
        {
            get => _postCode;
            set => SetProperty(ref _postCode, value);
        }

        public Result<bool> SeemsValid()
        {
            string err = "";
            if (String.IsNullOrWhiteSpace(StreetNumberAndName))
            {
                err += "Please enter a street number and name.";
            }
            if (String.IsNullOrWhiteSpace(CityOrSuburb))
            {
                err += "Please enter a city or suburb.";
            }
            if (String.IsNullOrWhiteSpace(Postcode))
            {
                err += "Please enter a city or suburb.";
            }

            if (String.IsNullOrEmpty(err))
            {
                return new Result<bool> { Ok = true };
            }

            return new Result<bool> { Err = err };
        }

        public override string ToString()
        {
            return StreetNumberAndName + ", "
                                       + CityOrSuburb + " "
                                       + Postcode;

        }
    }
}