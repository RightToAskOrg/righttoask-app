using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading;
using Org.BouncyCastle.Tls;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.Resx;

namespace RightToAskClient.Models
{
    // This class reads in information about committees and upcoming hearings.
    public static class CommitteesAndHearingsData
    {
        public static readonly UpdatableCommitteesAndHearingsData CommitteesData =
            new UpdatableCommitteesAndHearingsData();
    }
}