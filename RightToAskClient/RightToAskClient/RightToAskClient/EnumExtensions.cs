using RightToAskClient.Models;

namespace RightToAskClient
{
    public static class EnumExtensions
    {
	    public static string Salutation(this ParliamentData.Chamber chamber)
	    {
		    switch (chamber)
		    {
			    case ParliamentData.Chamber.Australian_Senate:
				    return "Senator";

			    case ParliamentData.Chamber.Australian_House_Of_Representatives:
			    case ParliamentData.Chamber.Tas_House_Of_Assembly:
			    case ParliamentData.Chamber.SA_House_Of_Assembly:
				    return "MP";

			    case ParliamentData.Chamber.ACT_Legislative_Assembly:
			    case ParliamentData.Chamber.NSW_Legislative_Assembly:
			    case ParliamentData.Chamber.NT_Legislative_Assembly:
			    case ParliamentData.Chamber.Qld_Legislative_Assembly:
			    case ParliamentData.Chamber.Vic_Legislative_Assembly:
			    case ParliamentData.Chamber.WA_Legislative_Assembly:
				    return "MLA";

			    case ParliamentData.Chamber.NSW_Legislative_Council:
			    case ParliamentData.Chamber.SA_Legislative_Council:
			    case ParliamentData.Chamber.Tas_Legislative_Council:
			    case ParliamentData.Chamber.Vic_Legislative_Council:
			    case ParliamentData.Chamber.WA_Legislative_Council:
				    return "MLC";

			    default:
				    return "";
		    }
	    }
    }
}