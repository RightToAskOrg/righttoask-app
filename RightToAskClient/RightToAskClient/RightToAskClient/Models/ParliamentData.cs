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
using RightToAskClient.Helpers;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.Resx;
using Xamarin.CommunityToolkit.Converters;

namespace RightToAskClient.Models
{
    // This class reads in information about electorates, MPs, etc, from static files.
    public static class ParliamentData
    {
	    public static readonly UpdatableParliamentAndMPData MPAndOtherData = new UpdatableParliamentAndMPData();

	    public static ObservableCollection<MP> AllMPs
	    {
		    get
		    {
			    return new ObservableCollection<MP>(MPAndOtherData.AllMPs);
		    }
	    }

	    public static List<RegionsContained> VicRegions
	    {
		    get
		    {
			    return MPAndOtherData.VicRegions;
		    }

	    }

	    public static List<string> StatesAndTerritories
	    {
		    get
		    {
			    var emptyList = new List<StateEnum>();
			    StateEnum[] statesarray =  Enum.GetValues(typeof(StateEnum)) as StateEnum[] ?? Array.Empty<StateEnum>();
				emptyList.AddRange(statesarray);
				return	emptyList.Select(s => s.ToString()).ToList();
		    }
		}

	    public enum StateEnum
	    {
		    [EnumMember(Value = "ACT")]
		    ACT,
		    [EnumMember(Value = "NSW")]
		    NSW,
		    [EnumMember(Value = "NT")]
		    NT,
		    [EnumMember(Value = "QLD")]
		    QLD,
		    [EnumMember(Value = "SA")]
		    SA,
		    [EnumMember(Value = "TAS")]
		    TAS,
		    [EnumMember(Value = "VIC")]
		    VIC,
		    [EnumMember(Value = "WA")]
		    WA
	    }

	    public static List<string> StateStrings => Enum.GetNames(typeof(StateEnum)).ToList();

	    public static Result<StateEnum> StateStringToEnum(string state)
	    {
			var success = Enum.TryParse(state, true, out StateEnum value);
			if (success)
			{
				return new Result<StateEnum>() { Ok = value };
			}
			else
			{
				var error = "Couldn't parse state: " + state;
				Debug.WriteLine(error);
				return new Result<StateEnum>() { Err = error};
			}
	    }
	    
	    public static readonly Dictionary<StateEnum, List<ParliamentData.Chamber>> Parliaments = new Dictionary<StateEnum, List<Chamber>>
	    {
		    { StateEnum.ACT, new List<Chamber> { Chamber.ACT_Legislative_Assembly } },
		    { StateEnum.NSW, new List<Chamber> { Chamber.NSW_Legislative_Assembly, Chamber.NSW_Legislative_Council } },
		    { StateEnum.NT, new List<Chamber> { Chamber.NT_Legislative_Assembly } },
		    { StateEnum.QLD, new List<Chamber> { Chamber.Qld_Legislative_Assembly } },
		    { StateEnum.SA, new List<Chamber> { Chamber.SA_House_Of_Assembly, Chamber.SA_Legislative_Council } },
		    { StateEnum.TAS, new List<Chamber> { Chamber.Tas_House_Of_Assembly, Chamber.Tas_Legislative_Council } },
		    { StateEnum.VIC, new List<Chamber> { Chamber.Vic_Legislative_Assembly, Chamber.Vic_Legislative_Council } },
		    { StateEnum.WA, new List<Chamber> { Chamber.WA_Legislative_Assembly, Chamber.WA_Legislative_Council } },
	    };

	    // These are deliberately sorted in a way that hopefully reflects the order of importance and locality - first
	    // the House of Representatives, then the state lower house (or only) chambers, then the Senate, then the state
	    // upper house chambers. Of course, this is a matter of opinion. This reflects the order the MP lists will be
	    // shown.
	    
	    [JsonConverter(typeof(JsonStringEnumConverter))]
	    public enum Chamber
	    {
		    [EnumMember(Value = "Australian_House_Of_Representatives")]
		    Australian_House_Of_Representatives,

		    [EnumMember(Value = "ACT_Legislative_Assembly")]
		    ACT_Legislative_Assembly,

		    [EnumMember(Value = "NSW_Legislative_Assembly")]
		    NSW_Legislative_Assembly,

		    [EnumMember(Value = "NT_Legislative_Assembly")]
		    NT_Legislative_Assembly,

		    [EnumMember(Value = "Qld_Legislative_Assembly")]
		    Qld_Legislative_Assembly,

		    [EnumMember(Value = "SA_House_Of_Assembly")]
		    SA_House_Of_Assembly,

		    [EnumMember(Value = "Tas_House_Of_Assembly")]
		    Tas_House_Of_Assembly,

		    [EnumMember(Value = "Vic_Legislative_Assembly")]
		    Vic_Legislative_Assembly,

		    [EnumMember(Value = "WA_Legislative_Assembly")]
		    WA_Legislative_Assembly,

		    [EnumMember(Value = "Australian_Senate")]
		    Australian_Senate,

		    [EnumMember(Value = "NSW_Legislative_Council")]
		    NSW_Legislative_Council,

		    [EnumMember(Value = "SA_Legislative_Council")]
		    SA_Legislative_Council,

		    [EnumMember(Value = "Tas_Legislative_Council")]
		    Tas_Legislative_Council,

		    [EnumMember(Value = "Vic_Legislative_Council")]
		    Vic_Legislative_Council,

		    [EnumMember(Value = "WA_Legislative_Council")]
		    WA_Legislative_Council
	    }

	    /* Somewhat repetitious of the chambers and states, but necessary because 'jurisdiction' is used to describe
	     * committees which may be joint, and therefore not connected with a single chamber. So this is all the states,
	     * plus all the chambers, plus a special value 'Federal' for joint federal committees.
	     */
	    [JsonConverter(typeof(JsonStringEnumConverter))]
	    public enum Jurisdiction
	    {
		    [EnumMember(Value = "ACT")]
		    ACT,
		    
		    [EnumMember(Value = "NSW")]
		    NSW,
		    
		    [EnumMember(Value = "NT")]
		    NT,
		    
		    [EnumMember(Value = "QLD")]
		    QLD,
		    
		    [EnumMember(Value = "SA")]
		    SA,
		    
		    [EnumMember(Value = "TAS")]
		    TAS,
		    
		    [EnumMember(Value = "VIC")]
		    VIC,
		    
		    [EnumMember(Value = "WA")]
		    WA,
		    
		    [EnumMember(Value = "Federal")]
		    Federal,
		    
		    [EnumMember(Value = "ACT_Legislative_Assembly")]
		    ACT_Legislative_Assembly,

		    [EnumMember(Value = "Australian_House_Of_Representatives")]
		    Australian_House_Of_Representatives,

		    [EnumMember(Value = "Australian_Senate")]
		    Australian_Senate,

		    [EnumMember(Value = "NSW_Legislative_Assembly")]
		    NSW_Legislative_Assembly,

		    [EnumMember(Value = "NSW_Legislative_Council")]
		    NSW_Legislative_Council,

		    [EnumMember(Value = "NT_Legislative_Assembly")]
		    NT_Legislative_Assembly,

		    [EnumMember(Value = "Qld_Legislative_Assembly")]
		    Qld_Legislative_Assembly,

		    [EnumMember(Value = "SA_House_Of_Assembly")]
		    SA_House_Of_Assembly,

		    [EnumMember(Value = "SA_Legislative_Council")]
		    SA_Legislative_Council,

		    [EnumMember(Value = "Tas_House_Of_Assembly")]
		    Tas_House_Of_Assembly,

		    [EnumMember(Value = "Tas_Legislative_Council")]
		    Tas_Legislative_Council,

		    [EnumMember(Value = "Vic_Legislative_Assembly")]
		    Vic_Legislative_Assembly,

		    [EnumMember(Value = "Vic_Legislative_Council")]
		    Vic_Legislative_Council,

		    [EnumMember(Value = "WA_Legislative_Assembly")]
		    WA_Legislative_Assembly,

		    [EnumMember(Value = "WA_Legislative_Council")]
		    WA_Legislative_Council
	    }
	    
	    [JsonConverter(typeof(JsonStringEnumConverter))]
	    public enum CommitteeType
	    {
		    [EnumMember(Value = "ADMIN")]
		    ADMIN,
		    
		    [EnumMember(Value = "STAND")]
		    STAND,
		    
		    [EnumMember(Value = "SELECT")]
		    SELECT,
		    
		    [EnumMember(Value = "SESSION")]
		    SESSION
	    }

	    // Find all the MPs given a state and a list of electorates.
	    // States are "electorates" in the Senate, i.e. of the form
	    // ElectorateWithChamber(ParliamentData.Chamber.Australian_Senate, state));
	    public static List<MP> FindAllMPsGivenElectorates(List<ElectorateWithChamber> electorates)
	    {
		    return MPAndOtherData.AllMPs.Where(mp =>
			    
			    // Those who represent an exactly-matching electorate/region.
			    electorates.Exists(e => e.Equals(mp.electorate))
			    
				// Add the MPs in single-electorate houses, i.e. the upper houses of some parliaments.
			    || electorates.Exists(e => e.chamber == mp.electorate.chamber && IsSingleElectorate(e.chamber))
			    
			    ).OrderBy(mp => mp.electorate.chamber).ToList();
	    }

	    public static readonly ObservableCollection<Authority> AllAuthorities =
		    new ObservableCollection<Authority>(ReadAuthoritiesFromFiles());
	    
	    public static List<string> AllFederalElectorates => 
				    MPAndOtherData.ListElectoratesInChamber(Chamber.Australian_House_Of_Representatives);
	    // If state matches anything in the regions list, return only the electorates in the state. 
	    // Otherwise return the lot. 
	    public static List<string> HouseOfRepsElectorates(string state)
	    {
		    if (MPAndOtherData.IsInitialised)
		    {
			    List<string> electoratesList = MPAndOtherData.FederalElectoratesByState.
				    Find(rc => rc.super_region.Equals(state, StringComparison.OrdinalIgnoreCase))?
				    .regions.OrderBy(r=>r).ToList() ?? new List<string>();

			    return electoratesList.Any()
				    ? electoratesList
				    : MPAndOtherData.ListElectoratesInChamber(Chamber.Australian_House_Of_Representatives);
		    }

			return new List<string>();
	    }
	    
	    private static List<Chamber> StateLowerHouseChambers = new List<Chamber>
	    {
		    Chamber.ACT_Legislative_Assembly,
		    Chamber.NSW_Legislative_Assembly,
		    Chamber.NT_Legislative_Assembly,
		    Chamber.Qld_Legislative_Assembly,
		    Chamber.SA_House_Of_Assembly,
		    Chamber.Vic_Legislative_Assembly,
		    Chamber.Tas_House_Of_Assembly,
		    Chamber.WA_Legislative_Assembly
	    };

	    private static List<Chamber> StateUpperHouseChambers = new List<Chamber>
	    {
		    Chamber.NSW_Legislative_Council,
		    Chamber.SA_Legislative_Council,
		    Chamber.Vic_Legislative_Council,
		    Chamber.Tas_Legislative_Council,
		    Chamber.WA_Legislative_Council
	    };

	    public static bool IsLowerHouseChamber(Chamber chamber)
	    {
		    return StateLowerHouseChambers.Contains(chamber);
	    }

	    public static bool IsUpperHouseChamber(Chamber chamber)
	    {
		    return StateUpperHouseChambers.Contains(chamber);
	    }

	    // Note that after 2024 WA LC will be here too.
	    private static bool IsSingleElectorate(Chamber chamber)
	    {
		    return chamber == Chamber.SA_Legislative_Council ||
		           chamber == Chamber.NSW_Legislative_Council;

	    }

	    private static List<Authority> ReadAuthoritiesFromFiles()
	    {
		    var AllAuthoritiesFromFile = new List<Authority>();
		    FileIO.ReadDataFromCSV("all-authorities.csv", AllAuthoritiesFromFile, ParseCSVLineAsAuthority);
		    return AllAuthoritiesFromFile;
	    }

	    // This parses a line from Right To Know's CSV file as an Authority.
	    // It is, obviously, very specific to the expected file format.
	    // Ignore any line that doesn't produce at least 3 words.
	    private static Authority? ParseCSVLineAsAuthority(string line)
	    {
		    string[] words = line.Split(',');
		    if (words.Length >= 3)
		    {

			    Authority newAuthority = new Authority()
			    {
				    AuthorityName = words[0],
				    NickName = words[1],
				    RightToKnowURLSuffix = words[2]
			    };
			    return newAuthority;
		    }
		    else
		    {
			    return null;
		    }
	    }


		/* Finds all the chambers in which a citizen of this state is represented,
		 * including the House of Representatives and the Senate.
		 * If the string input doesn't match any states, it simply
		 * returns the federal chambers.
		 */
		public static List<Chamber> FindFederalChambers()
		{
			return new List<Chamber>()
			{
				Chamber.Australian_House_Of_Representatives,
				Chamber.Australian_Senate
			};
		}
		public static List<Chamber> FindChambers(StateEnum state, bool stateKnown)
		{
			var chambersForTheState = FindFederalChambers();

			if (stateKnown)
			{
				switch (state)
				{
					case (StateEnum.ACT):
						chambersForTheState.Add(Chamber.ACT_Legislative_Assembly);
						break;
					case (StateEnum.NSW):
						chambersForTheState.Add(Chamber.NSW_Legislative_Assembly);
						chambersForTheState.Add(Chamber.NSW_Legislative_Council);
						break;
					case (StateEnum.NT):
						chambersForTheState.Add(Chamber.NT_Legislative_Assembly);
						break;
					case (StateEnum.QLD):
						chambersForTheState.Add(Chamber.Qld_Legislative_Assembly);
						break;
					case (StateEnum.SA):
						chambersForTheState.Add(Chamber.SA_House_Of_Assembly);
						chambersForTheState.Add(Chamber.SA_Legislative_Council);
						break;
					case (StateEnum.VIC):
						chambersForTheState.Add(Chamber.Vic_Legislative_Assembly);
						chambersForTheState.Add(Chamber.Vic_Legislative_Council);
						break;
					case (StateEnum.TAS):
						chambersForTheState.Add(Chamber.Tas_House_Of_Assembly);
						chambersForTheState.Add(Chamber.Tas_Legislative_Council);
						break;
					case (StateEnum.WA):
						chambersForTheState.Add(Chamber.WA_Legislative_Assembly);
						chambersForTheState.Add(Chamber.WA_Legislative_Council);
						break;
				}
			}
			return chambersForTheState;
		}

        // What to tell the user about their other state electorate, given that they've chosen the one they're allowed
        // to choose. For some states, there is no other house (it's always the upper house). For others, we can 
        // infer a specific upper house state. For yet others, the upper house is a single electorate.
        // When called with neither state nor commonwealth regions, it returns the right message (which is a function of
        // state) and a blank region.
		public static (string, string, string) InferOtherChamberInfoGivenOneRegion(StateEnum state, string stateRegion, string commRegion)
		{
			string region = String.Empty;
			string inferredHouseMessage = String.Empty;
			string choosableHouseMessage = AppResources.LegislativeAssemblyText;
			
			
			if (!HasUpperHouse(state))
			{
				 inferredHouseMessage = String.Format(AppResources.NoUpperHousePickerTitle, state);
			}
			else if (UpperHouseIsSingleElectorate(state))
			{
				 inferredHouseMessage = String.Format(AppResources.UpperHouseIsSingleElectorateText, state);
			} 
			else if (state == StateEnum.WA)
			{
				 inferredHouseMessage = AppResources.UpperHouseWANotWorkingText;
			}
			else
			{
				var electorateList = FindAllRelevantElectorates(state, stateRegion, commRegion);
				if (state == StateEnum.VIC)
				{
					inferredHouseMessage = AppResources.UpperHouseElectorateText;
					region = electorateList.Find(ec => ec.chamber == Chamber.Vic_Legislative_Council)?.region ?? "";
				}
				else if (state == StateEnum.TAS)
				{
					inferredHouseMessage = AppResources.TasLowerHouseElectorateText;
					choosableHouseMessage = AppResources.UpperHouseElectorateText;
					region = electorateList.Find(ec => ec.chamber == Chamber.Tas_House_Of_Assembly)?.region ?? "";
				}
			}

			return (choosableHouseMessage, inferredHouseMessage, region);
		}
		
		// Used because the Geoscape API returns electorates in all Uppercase, which messes with the URL for the webview that displays the map of electorates
		public static string ConvertGeoscapeElectorateToStandard(string state, string electorate)
		{
			List<string> options = AllFederalElectorates;
			string result = "";
			for (int i = 0; i < options.Count - 1; i++)
			{
				if (options[i].ToUpper() == electorate.ToUpper())
				{
					result = options[i];
				}
			}
			return result;
		}




	    public static List<string> ListElectoratesInStateLowerHouse(StateEnum state)
	    {
		    Result<Chamber> possibleChamber = GetLowerHouseChamber(state);
		    if (MPAndOtherData.IsInitialised && possibleChamber.Err.IsNullOrEmpty())
		    {
			    return MPAndOtherData.ListElectoratesInChamber(possibleChamber.Ok);
		    }

		    return new List<string>();
	    }

	    // Call only when state known.
	    // TODO This can probably guarantee a result given a valid state.
	    public static Result<Chamber> GetLowerHouseChamber(StateEnum state)
	    {
		    List<Chamber> chamberList = FindChambers(state, true).Where(p => IsLowerHouseChamber(p)).ToList();
		    if (chamberList.IsNullOrEmpty() || chamberList.Count() > 1)
		    {
			    Debug.WriteLine("Error: " + chamberList.Count() + " lower house chambers in " + state);
			    return new Result<Chamber>() { Err = "Can't get lower house chamber." };
		    }

		    return new Result<Chamber>() { Ok = chamberList[0] };
	    }

	    // Not every state has an upper house, so failing to find one is not necessarily an error.
	    // So we return empty but don't log an error. 
	    // After that, it's just like the lower-house lookup.
	    public static List<string> ListElectoratesInStateUpperHouse(StateEnum state)
	    {
		    if (!HasUpperHouse(state))
		    {
			    return new List<string>();
		    }

		    Result<Chamber> possibleChamber = GetUpperHouseChamber(state);
		    if (MPAndOtherData.IsInitialised && possibleChamber.Err.IsNullOrEmpty())
		    {
			    return MPAndOtherData.ListElectoratesInChamber(possibleChamber.Ok);
		    }

		    return new List<string>();
	    }

	    // Some states have no upper house, in which case this returns an error.
	    public static Result<Chamber> GetUpperHouseChamber(StateEnum state)
	    {
		    if (HasUpperHouse(state))
		    {
			    List<Chamber> chamberList = FindChambers(state,true).Where(c => IsUpperHouseChamber(c)).ToList();
			    if (chamberList.IsNullOrEmpty() || chamberList.Count > 1)
			    {
				    Debug.WriteLine("Error: " + chamberList.Count() + " lower house chambers in " + state);
				    return new Result<Chamber>() { Err = "Error: wrong number of lower houses for " + state };
			    }

			    return new Result<Chamber>() { Ok = chamberList[0] };
		    }

		    return new Result<Chamber>() { Err = "This state has no upper house." };
	    }

	    public static bool HasUpperHouse(StateEnum state)
	    {
		    return Parliaments[state].Count() == 2;
	    }

	    // Note that this is currently *not* true for WA, but will be when proposed electoral reforms
	    // are enacted.
	    private static bool UpperHouseIsSingleElectorate(StateEnum state)
	    {
		    return state == StateEnum.NSW || state == StateEnum.SA;
	    }

	    public static readonly List<string> Domains =
		    new List<string>(((Enum.GetValues(typeof(Chamber)) as IEnumerable<Chamber>))
			    .Select(c => GetDomain(c)).Distinct());
	    public static string GetDomain(Chamber electorateChamber) => electorateChamber switch
	    {
		    Chamber.ACT_Legislative_Assembly => "parliament.act.gov.au",
		    Chamber.Australian_Senate => "aph.gov.au",
		    Chamber.Australian_House_Of_Representatives => "aph.gov.au",
		    Chamber.NSW_Legislative_Assembly => "parliament.nsw.gov.au",
		    Chamber.NSW_Legislative_Council => "parliament.nsw.gov.au",
		    Chamber.NT_Legislative_Assembly => "parliament.nt.gov.au",
		    Chamber.Qld_Legislative_Assembly => "parliament.qld.gov.au",
		    Chamber.SA_House_Of_Assembly =>  "parliament.sa.gov.au",
		    Chamber.SA_Legislative_Council =>  "parliament.sa.gov.au",
		    Chamber.Tas_House_Of_Assembly => "parliament.tas.gov.au",
		    Chamber.Tas_Legislative_Council => "parliament.tas.gov.au",
		    Chamber.Vic_Legislative_Assembly => "parliament.vic.gov.au",
		    Chamber.Vic_Legislative_Council => "parliament.vic.gov.au",
		    Chamber.WA_Legislative_Assembly => "parliament.wa.gov.au",
		    Chamber.WA_Legislative_Council => "parliament.wa.gov.au"
	    };
    
        public static List<ElectorateWithChamber> GetElectoratesFromGeoscapeAddress(StateEnum state, GeoscapeAddressFeature addressData)
        {
	        var commElectoralRegion = addressData.Properties?.CommonwealthElectorate?.CommElectoralName ?? "";
	        var stateElectoralRegion = addressData.Properties?.StateElectorate?.StateElectoralName ?? "";

	        return FindAllRelevantElectorates(state, stateElectoralRegion, commElectoralRegion);
        }
        
	    // Finds as many electorates as can be inferred from the given information.
	    // This is highly dependent on specific details of states and hence full of special cases.
	    // 
	    public static List<ElectorateWithChamber> FindAllRelevantElectorates(StateEnum state, string stateRegion,
		    string commRegion)
	    {
		    List<ElectorateWithChamber> electorateList = new List<ElectorateWithChamber>();
		    
		    // House of Representatives Electorate
		    if (!String.IsNullOrEmpty(commRegion))
		    {
			    electorateList.Add(new ElectorateWithChamber(Chamber.Australian_House_Of_Representatives,
				    commRegion));
		    }

		    // State Lower House Electorates. Tas is special because the region returned by Geoscape
		    // is the Upper House (Legislative Council) Electorate - Lower House Electorates match Commonwealth ones.
		    // For all the rest, Geoscape's stateRegion is the Lower House Electorate.
		    if (state == StateEnum.TAS && !String.IsNullOrEmpty(commRegion))
		    {
			    electorateList.Add(new ElectorateWithChamber(Chamber.Tas_House_Of_Assembly, commRegion));
		    }
		    else if (state != StateEnum.TAS && !String.IsNullOrEmpty(stateRegion))
		    {
			    // guaranteed of a result here.
			    var chamberResult = GetLowerHouseChamber(state);
			    if (String.IsNullOrEmpty(chamberResult.Err))
			    {
				    electorateList.Add(new ElectorateWithChamber(chamberResult.Ok, stateRegion));
			    }
		    }


		    // TODO move states to enum.
		    // Each state is a Senate 'region'
		    electorateList.Add(new ElectorateWithChamber(Chamber.Australian_Senate, state.ToString()));
		    
		    // State Upper House electorates
		    if (state == StateEnum.TAS && !String.IsNullOrEmpty(stateRegion))
		    {
			    electorateList.Add(new ElectorateWithChamber(Chamber.Tas_Legislative_Council, stateRegion));
		    }
			    // TODO: Do likewise for WA.
		    else if (state == StateEnum.VIC && !String.IsNullOrEmpty(stateRegion))
		    {
			    var superRegionsContained 
				    = VicRegions.Find(rc => rc.regions.Contains(stateRegion,StringComparer.OrdinalIgnoreCase ));
			    if (superRegionsContained != null)
			    {
				    electorateList.Add(new ElectorateWithChamber(Chamber.Vic_Legislative_Council,
					    superRegionsContained.super_region));
			    }
		    }
		    else if (HasUpperHouse(state))
		    {
			    // TODO state to enum or check for Err result.
			    // In every other state that has an upper house, they're a single electorate not divided into regions.
			    electorateList.Add(new ElectorateWithChamber(GetUpperHouseChamber(state).Ok, ""));
		    }

		    return electorateList;
	    }
	    
	    private static List<string> FindAllElectoratesGivenPredicate(List<ElectorateWithChamber> electorates, Predicate<ElectorateWithChamber> func)
	    {
		    var tempList = new List<string>();
		    return electorates.Where(e => func(e)).Select(ec => ec.region).ToList();
	    }
        public static string FindOneElectorateGivenPredicate(List<ElectorateWithChamber> electorates, Predicate<ElectorateWithChamber> func)
        {
            var electoratePair = electorates.Find(func);
            return electoratePair?.region ?? "";
        }

        
        public static Result<Uri> StringToValidParliamentaryUrl(string value)
        {
            try
            {
                var link = new Uri(value);
                // Valid url, but not one of the allowed ones.
                if (!link.IsWellFormedOriginalString())
                {
                    return new Result<Uri>()
                    {
                        Err = AppResources.LinkNotWellFormedErrorText
                    };
                }
                if (!ParliamentData.Domains.Contains(link.Host))
                {
                    return new Result<Uri>()
                    {
                        Err = AppResources.ParliamentaryURLErrorText
                    };
                }
                else
                {
                    return new Result<Uri>()
                    {
                        Ok = link
                    };
                }
            }
            // Not a valid url 
            catch (Exception e)
            {
                return new Result<Uri>()
                {
                    Err = e.Message,
                };
            }
        }
    }
}