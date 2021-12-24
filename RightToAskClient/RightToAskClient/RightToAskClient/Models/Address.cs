using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RightToAskClient.Annotations;

namespace RightToAskClient.Models
{
    public class Address : INotifyPropertyChanged
    {
        public string StreetNumberAndName { get; set; } = "";
        public string CityOrSuburb { get; set; } = "";
        public string Postcode { get; set; } = "";
        
        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Result<bool> seemsValid()
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