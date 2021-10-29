using System.Collections.Generic;
using System.Threading.Tasks;
using RightToAskClient.Models;

namespace RightToAskClient.Data
{
	public interface IRestService
	{
		Task<Result<List<string>>> RefreshDataAsync ();

		Task SaveTodoItemAsync (Registration item, bool isNewItem);
	}
}
