using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RightToAskClient.Maui.HttpClients;
using RightToAskClient.Maui.Models.ServerCommsData;
using RightToAskClient.Maui.ViewModels;

namespace RightToAskClient.Maui.Models
{
    /* This class initialises all the data about other RTA participants that we
     * get from the server. At the moment, that's just a list of IDs.
     */
    public class UpdatableOtherUserData
    {
        private List<String> _otherParticipantIDs = new List<String>();
        private List<Person> _otherParticipants = new List<Person>();

        public List<Person> OtherParticipants
        {
            get => _otherParticipants;
        }


        private bool _isInitialised; // Defaults to false.

        public bool IsInitialised
        {
            get => _isInitialised;
        }


        public async Task<Result<bool>> TryInitialisingFromServer()
        {
            Result<List<string>>? serverUserList = await RTAClient.GetUserList();
            if (serverUserList is null)
            {
                return new Result<bool>() { Err = "Could not reach server." };
            }

            // Success. Set list of selectable committees and update filters to reflect new list.
            if (String.IsNullOrEmpty(serverUserList.Err))
            {
                _isInitialised = true;
                _otherParticipantIDs = serverUserList.Ok;

                foreach (string userId in _otherParticipantIDs)
                {
                    var userDetails = await RTAClient.GetUserById(userId);
                    if (String.IsNullOrEmpty(userDetails.Err))
                    {
                        OtherParticipants.Add(new Person(userDetails.Ok)); 
                    }
                }
                App.ReadingContext.Filters.InitSelectableLists();
                return new Result<bool>() { Ok = true };
            }

            return new Result<bool>()
            {
                Err = serverUserList.Err
            };
        }
    }
}