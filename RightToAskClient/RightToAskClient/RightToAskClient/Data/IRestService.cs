using System.Collections.Generic;
using System.Threading.Tasks;
using RightToAskClient.Models;

namespace RightToAskClient.Data
{
	public interface IRestService
	{
		Task<Result<List<string>>> RefreshDataAsync ();

		Task<Result<bool>> SaveTodoItemAsync (Registration item);
		Task<Result<List<string>>> GetUserList();

		Task<Result<GeoscapeAddressFeatureCollection>> GetGeoscapeAddressData(string address);
	}
}
