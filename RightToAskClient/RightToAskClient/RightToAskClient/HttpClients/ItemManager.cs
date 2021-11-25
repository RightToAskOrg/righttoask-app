using System.Collections.Generic;
using System.Threading.Tasks;
using RightToAskClient.Models;

namespace RightToAskClient.Data
{
	public class ItemManager
	{
		IRestService restService;

		public ItemManager (IRestService service)
		{
			restService = service;
		}

		/*
		public Task<Result<List<string>>> GetUsersAsync ()
		{
			return restService.GetUserList();	
		}
		*/

		/*
		public Task<Result<bool>> SaveTaskAsync (Registration item)
		{
			return restService.SaveTodoItemAsync (item);
		}
		*/

		/*
		public Task<Result<GeoscapeAddressFeatureCollection>> GetGeoscapeAddressDataAsync(string address)
		{
			return restService.GetGeoscapeAddressData(address);
		}
		*/
	}
}
