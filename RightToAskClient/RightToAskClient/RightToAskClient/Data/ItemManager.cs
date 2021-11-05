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

		public Task<Result<List<string>>> GetTasksAsync ()
		{
			return restService.RefreshDataAsync ();	
		}

		public Task<Result<bool>> SaveTaskAsync (Registration item, bool isNewItem = false)
		{
			return restService.SaveTodoItemAsync (item, isNewItem);
		}
	}
}
