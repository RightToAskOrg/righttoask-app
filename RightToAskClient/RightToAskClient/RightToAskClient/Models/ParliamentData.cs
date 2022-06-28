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

	    public static List<RegionsContained> FederalElectoratesAsStates
	    {
		    get
		    {
			    return MPAndOtherData.FederalElectoratesByState;
		    }
	    }

	    public static List<RegionsContained> VicRegions
	    {
		    get
		    {
			    return MPAndOtherData.VicRegions;
		    }

	    }

	    public static readonly List<string> StatesAndTerritories
		    = StatesAsStringList();

	    private static List<string> StatesAsStringList()
	    {
		    var stateList = new List<string>();
		    FieldInfo[] fields = (typeof(State).GetFields());

		    for (int i = 0; i < fields.Length; i++)
		    {
			    stateList.Add(fields[i].GetValue(typeof(State)).ToString());
		    }

		    return stateList;
	    }

	    public static class State
	    {
		    public const string ACT = "ACT";
		    public const string NSW = "NSW";
		    public const string NT = "NT";
		    public const string QLD = "QLD";
		    public const string SA = "SA";
		    public const string TAS = "TAS";
		    public const string VIC = "VIC";
		    public const string WA = "WA";
	    }

	    public enum StateEnum
	    {
		    ACT,
		    NSW,
		    NT,
		    QLD,
		    SA,
		    TAS,
		    VIC,
		    WA
	    }

	    public static readonly Dictionary<string, List<Chamber>> Parliaments = new Dictionary<string, List<Chamber>>
	    {
		    { State.ACT, new List<Chamber> { Chamber.ACT_Legislative_Assembly } },
		    { State.NSW, new List<Chamber> { Chamber.NSW_Legislative_Assembly, Chamber.NSW_Legislative_Council } },
		    { State.NT, new List<Chamber> { Chamber.NT_Legislative_Assembly } },
		    { State.QLD, new List<Chamber> { Chamber.Qld_Legislative_Assembly } },
		    { State.SA, new List<Chamber> { Chamber.SA_House_Of_Assembly, Chamber.SA_Legislative_Council } },
		    { State.TAS, new List<Chamber> { Chamber.Tas_House_Of_Assembly, Chamber.Tas_Legislative_Council } },
		    { State.VIC, new List<Chamber> { Chamber.Vic_Legislative_Assembly, Chamber.Vic_Legislative_Council } },
		    { State.WA, new List<Chamber> { Chamber.WA_Legislative_Assembly, Chamber.WA_Legislative_Council } },
	    };

	    [JsonConverter(typeof(JsonStringEnumConverter))]
	    public enum Chamber
	    {
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
			    
			    ).ToList();
	    }

	    
	    /*
	    public static class Chamber 
	    {
			public const string ACT_Legislative_Assembly = "ACT_Legislative_Assembly";
			public const string Australian_House_Of_Representatives = "Australian_House_Of_Representatives";
		    public const string Australian_Senate = "Australian_Senate";
		    public const string NSW_Legislative_Assembly = "NSW_Legislative_Assembly" ;
			public const string NSW_Legislative_Council ="NSW_Legislative_Council" ;
			public const string NT_Legislative_Assembly = "NT_Legislative_Assembly" ;
		    public const string Qld_Legislative_Assembly ="Qld_Legislative_Assembly" ;
		    public const string SA_Legislative_Assembly = "SA_Legislative_Assembly" ;
			public const string SA_Legislative_Council = "SA_Legislative_Council" ;
			public const string Tas_House_Of_Assembly ="Tas_House_Of_Assembly" ;
			public const string Tas_Legislative_Council ="Tas_Legislative_Council" ;
		    public const string Vic_Legislative_Assembly = "Vic_Legislative_Assembly" ;
			public const string Vic_Legislative_Council = "Vic_Legislative_Council" ;
			public const string WA_Legislative_Assembly = "WA_Legislative_Assembly";
			public const string WA_Legislative_Council = "WA_Legislative_Council";
	    } */

	    public static readonly ObservableCollection<Authority> AllAuthorities =
		    new ObservableCollection<Authority>(ReadAuthoritiesFromFiles());

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
		public static List<Chamber> FindChambers(string state)
		{
			var chambersForTheState = new List<Chamber>();

			if (StatesAndTerritories.Contains(state))
			{
				chambersForTheState.Add(Chamber.Australian_House_Of_Representatives);
				chambersForTheState.Add(Chamber.Australian_Senate);

				switch (state.ToUpper())
				{
					case (State.ACT):
						chambersForTheState.Add(Chamber.ACT_Legislative_Assembly);
						break;
					case (State.NSW):
						chambersForTheState.Add(Chamber.NSW_Legislative_Assembly);
						chambersForTheState.Add(Chamber.NSW_Legislative_Council);
						break;
					case (State.NT):
						chambersForTheState.Add(Chamber.NT_Legislative_Assembly);
						break;
					case (State.QLD):
						chambersForTheState.Add(Chamber.Qld_Legislative_Assembly);
						break;
					case (State.SA):
						chambersForTheState.Add(Chamber.SA_House_Of_Assembly);
						chambersForTheState.Add(Chamber.SA_Legislative_Council);
						break;
					case (State.VIC):
						chambersForTheState.Add(Chamber.Vic_Legislative_Assembly);
						chambersForTheState.Add(Chamber.Vic_Legislative_Council);
						break;
					case (State.TAS):
						chambersForTheState.Add(Chamber.Tas_House_Of_Assembly);
						chambersForTheState.Add(Chamber.Tas_Legislative_Council);
						break;
					case (State.WA):
						chambersForTheState.Add(Chamber.WA_Legislative_Assembly);
						chambersForTheState.Add(Chamber.WA_Legislative_Council);
						break;
				}
			}
			return chambersForTheState;
		}

	    // TODO: add logic for inferred other houses.
	    public static List<ElectorateWithChamber> GetStateElectoratesGivenOneRegion(string state, string region)
	    {
		    Result<Chamber> chamber = GetLowerHouseChamber(state);
		    if (!String.IsNullOrEmpty(chamber.Err))
		    {
			    Debug.WriteLine("Error: Couldn't find lower house chamber for " + state);
			    return new List<ElectorateWithChamber>();
		    }

            return new List<ElectorateWithChamber>() { new ElectorateWithChamber(chamber.Ok, region) };
		}

		// Used because the Geoscape API returns electorates in all Uppercase, which messes with the URL for the webview that displays the map of electorates
		public static string ConvertGeoscapeElectorateToStandard(string state, string electorate)
		{
			List<string> options = ListElectoratesInHouseOfReps(state);
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

	    public static List<string> ListElectoratesInHouseOfReps(string state)
	    {
		    if (MPAndOtherData.IsInitialised)
		    {
			    return MPAndOtherData.ListElectoratesInChamber(Chamber.Australian_House_Of_Representatives);
		    }

		    return new List<string>();
	    }

	    public static List<string> ListElectoratesInStateLowerHouse(string state)
	    {
		    Result<Chamber> possibleChamber = GetLowerHouseChamber(state);
		    if (MPAndOtherData.IsInitialised && possibleChamber.Err.IsNullOrEmpty())
		    {
			    return MPAndOtherData.ListElectoratesInChamber(possibleChamber.Ok);
		    }

		    return new List<string>();
	    }

	    // TODO This can probably guarantee a result (i.e. Chamber) when state -> enum.
	    public static Result<Chamber> GetLowerHouseChamber(string state)
	    {
		    List<Chamber> chamberList = FindChambers(state).Where(p => IsLowerHouseChamber(p)).ToList();
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
	    public static List<string> ListElectoratesInStateUpperHouse(string state)
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

	    public static Result<Chamber> GetUpperHouseChamber(string state)
	    {
		    if (HasUpperHouse(state))
		    {
			    List<Chamber> chamberList = FindChambers(state).Where(c => IsUpperHouseChamber(c)).ToList();
			    if (chamberList.IsNullOrEmpty() || chamberList.Count > 1)
			    {
				    Debug.WriteLine("Error: " + chamberList.Count() + " lower house chambers in " + state);
				    return new Result<Chamber>() { Err = "Error: wrong number of lower houses for " + state };
			    }

			    return new Result<Chamber>() { Ok = chamberList[0] };
		    }

		    return new Result<Chamber>() { Err = "This state has no upper house." };
	    }

	    public static bool HasUpperHouse(string state)
	    {
		    return Parliaments[state].Count() == 2;
	    }

	    public static readonly List<string> Domains =
		    new List<string>((Enum.GetValues(typeof(Chamber)) as IEnumerable<Chamber>)
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
    
	    /*
		public static string GetDomainName(Chamber electorateChamber) 
		{
			switch (electorateChamber)
			{
			case Chamber.Australian_Senate:
			case Chamber.Australian_House_Of_Representatives:
				return "aph.gov.au";
			case Chamber.NSW_Legislative_Assembly:
			case Chamber.NSW_Legislative_Council:
				return "parliament.nsw.gov.au";
			case Chamber.NT_Legislative_Assembly:
				return "parliament.nt.gov.au";
			case Chamber.Qld_Legislative_Assembly:
				return "parliament.qld.gov.au";
			case Chamber.Tas_House_Of_Assembly:
			case Chamber.Tas_Legislative_Council:
				return "parliament.tas.gov.au";
			case Chamber.Vic_Legislative_Assembly:
				case Chamber.Vic_Legislative_Council:
				return "parliament.vic.gov.au";
			case Chamber.WA_Legislative_Assembly:
				case Chamber.WA_Legislative_Council:
				return "parliament.wa.gov.au";
			}

			return "";
		}
		*/
	    
        public static List<ElectorateWithChamber> GetElectoratesFromGeoscapeAddress(string state, GeoscapeAddressFeature addressData)
        {
	        var commElectoralRegion = addressData.Properties?.CommonwealthElectorate?.CommElectoralName ?? "";
	        var stateElectoralRegion = addressData.Properties?.StateElectorate?.StateElectoralName ?? "";

	        return FindAllRelevantElectorates(state, stateElectoralRegion, commElectoralRegion);
        }
        
	    // Finds as many electorates as can be inferred from the given information.
	    // This is highly dependent on specific details of states and hence full of special cases.
	    // This puts them in the order
	    // House of Reps
	    // State Lower (or only) House
	    // Senate
	    // State Upper House (if any)
	    private static List<ElectorateWithChamber> FindAllRelevantElectorates(string state, string stateRegion,
		    string commRegion)
	    {
		    List<ElectorateWithChamber> electorateList = new List<ElectorateWithChamber>();
		    if (String.IsNullOrEmpty(state))
		    {
			    return electorateList;
		    }

		    // TODO move states to enum.
		    // Each state is a Senate 'region'
		    electorateList.Add(new ElectorateWithChamber(ParliamentData.Chamber.Australian_Senate, state));
		    
		    // House of Representatives Electorate
		    if (!String.IsNullOrEmpty(commRegion))
		    {
			    electorateList.Add(new ElectorateWithChamber(Chamber.Australian_House_Of_Representatives,
				    commRegion));
		    }

		    // State Lower House Electorates. Tas is special because the region returned by Geoscape
		    // is the Upper House (Legislative Council) Electorate - Lower House Electorates match Commonwealth ones.
		    // For all the rest, Geoscape's stateRegion is the Lower House Electorate.
		    if (state == State.TAS && !String.IsNullOrEmpty(commRegion))
		    {
			    electorateList.Add(new ElectorateWithChamber(Chamber.Tas_House_Of_Assembly, commRegion));
		    }
		    else if (state != State.TAS && !String.IsNullOrEmpty(stateRegion))
		    {
			    // guaranteed of a result here.
			    var chamberResult = GetLowerHouseChamber(state);
			    if (String.IsNullOrEmpty(chamberResult.Err))
			    {
				    electorateList.Add(new ElectorateWithChamber(chamberResult.Ok, stateRegion));
			    }
		    }


		    // Lower house electorates
		    if (state == State.TAS && !String.IsNullOrEmpty(stateRegion))
		    {
			    electorateList.Add(new ElectorateWithChamber(Chamber.Tas_Legislative_Council, stateRegion));
		    }
			    // TODO: Do likewise for WA.
		    else if (state == State.VIC && !String.IsNullOrEmpty(stateRegion))
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
    }
}