#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Candidates.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          05-01-2024 15:05
// Last Updated On:     05-01-2024 15:05
// *****************************************/

#endregion

using Syncfusion.Blazor.DropDowns;

namespace Subscription.Server.Components.Pages;

public partial class Candidates
{
	private Task AllAlphabets()
	{
		return null;
	}

	private Task GetAlphabets(char arg)
	{
		return null;
	}

	private Task ClearFilter()
	{
		return null;
	}

	public CompanySearch SearchModel
	{
		get;
		set;
	}

	public bool HasViewRights
	{
		get;
		set;
	} = true;

	public int Count
	{
		get;
		set;
	}

	private Task PageNumberClick(PagerItemClickEventArgs arg)
	{
		return null;
	}

	private Task PageSizeChanged(PageSizeChangedArgs arg)
	{
		return null;
	}

	private Task AutocompleteValueChange(ChangeEventArgs<string, KeyValues> arg)
	{
		return null;
	}

	public class CandidateAdaptor : DataAdaptor
	{
		private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

		/// <summary>
		///     Asynchronously reads company data for the grid view on the Companies page.
		///     This method checks if the CompaniesList is not null and contains data, in which case it does not retrieve new data.
		///     If the CompaniesList is null or empty, it calls the GetCompanyReadAdaptor method to retrieve company data.
		///     If there are any companies in the retrieved data, it selects the first row in the grid view.
		/// </summary>
		/// <param name="dm">The DataManagerRequest object that contains the parameters for the data request.</param>
		/// <param name="key">An optional key to identify a specific data item. Default is null.</param>
		/// <returns>
		///     A Task that represents the asynchronous read operation. The value of the TResult parameter contains the
		///     retrieved data.
		/// </returns>
		public override async Task<object> ReadAsync(DataManagerRequest dm, string key = null)
		{
			if (!await _semaphoreSlim.WaitAsync(TimeSpan.Zero))
			{
				return null;
			}

			//if (_initializationTaskSource == null)
			//{
			//	return null;
			//}

			//await _initializationTaskSource.Task;
			//try
			//{
			//List<Company> _dataSource = [];

			//object _companyReturn = null;
			//try
			//{
			//	Dictionary<string, object> _restResponse = await General.GetRest<Dictionary<string, object>>("Company/GetGridCompanies", null, SearchModel);

			//	if (_restResponse == null)
			//	{
			//		_companyReturn = dm.RequiresCounts ? new DataResult
			//											 {
			//												 Result = _dataSource,
			//												 Count = 0 /*_count*/
			//											 } : _dataSource;
			//	}
			//	else
			//	{
			//		if (NAICS is not {Count: not 0} || State is not {Count: not 0} || Roles is not {Count: not 0})
			//		{
			//			RedisService _service = new(Start.CacheServer, Start.CachePort.ToInt32(), Start.Access, false);
			//			List<string> _keys = [CacheObjects.NAICS.ToString(), CacheObjects.States.ToString(), CacheObjects.Roles.ToString()];

			//			Dictionary<string, string> _values = await _service.BatchGet(_keys);
			//			NAICS = JsonConvert.DeserializeObject<List<IntValues>>(_values["NAICS"] ?? string.Empty);
			//			State = JsonConvert.DeserializeObject<List<IntValues>>(_values["States"] ?? string.Empty);
			//			Roles = JsonConvert.DeserializeObject<List<IntValues>>(_values["Roles"] ?? string.Empty);
			//		}

			//		_dataSource = JsonConvert.DeserializeObject<List<Company>>(_restResponse["Companies"].ToString() ?? string.Empty);
			//		int _count = _restResponse["Count"].ToInt32();
			//		Count = _count;
			//		if (_dataSource == null)
			//		{
			//			_companyReturn = dm.RequiresCounts ? new DataResult
			//												 {
			//													 Result = null,
			//													 Count = 1
			//												 } : null;
			//		}
			//		else
			//		{
			//			_companyReturn = dm.RequiresCounts ? new DataResult
			//												 {
			//													 Result = _dataSource,
			//													 Count = _count /*_count*/
			//												 } : _dataSource;
			//		}
			//	}
			//}
			//catch
			//{
			//if (_dataSource == null)
			//{
			//	_companyReturn = dm.RequiresCounts ? new DataResult
			//										 {
			//											 Result = null,
			//											 Count = 1
			//										 } : null;
			//}
			//else
			//{
			//	_dataSource.Add(new());

			//	_companyReturn = dm.RequiresCounts ? new DataResult
			//										 {
			//											 Result = _dataSource,
			//											 Count = 1
			//										 } : _dataSource;
			//}
			//}

			//if (Count > 0)
			//{
			//	await Grid.SelectRowAsync(0);
			//}

			//return _companyReturn;
			return null;
			//}
			//	catch
			//	{
			//		return null;
			//	}
			//	finally
			//	{
			//		//_semaphoreSlim.Release();
			//	}
			//}

		}
	}
}