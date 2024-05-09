#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           CandidateController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          05-06-2024 20:05
// Last Updated On:     05-09-2024 19:05
// *****************************************/

#endregion

#region Using

#endregion

namespace Subscription.API.Controllers;

[ApiController, Route("api/[controller]/[action]")]
public class CandidateController(IConfiguration configuration) : ControllerBase
{
	[HttpGet]
	public async Task<Dictionary<string, object>> GetGridCandidates([FromBody] CandidateSearch searchModel = null)
	{
		await using SqlConnection _connection = new(Start.ConnectionString);
		List<Candidate> _candidates = [];

		await using SqlCommand _command = new("GetCandidates", _connection);
		_command.CommandType = CommandType.StoredProcedure;
		_command.Int("RecordsPerPage", searchModel.ItemCount);
		_command.Int("PageNumber", searchModel.Page);
		_command.Int("SortColumn", searchModel.SortField);
		_command.TinyInt("SortDirection", searchModel.SortDirection);
		_command.Varchar("Name", 30, searchModel.Name);
		//_command.Varchar("Phone", 20, searchModel.Phone);
		//_command.Varchar("Email", 255, searchModel.EmailAddress);
		_command.Bit("MyCandidates", !searchModel.AllCandidates);
		_command.Bit("IncludeAdmin", searchModel.IncludeAdmin);
		_command.Varchar("Keywords", 2000, searchModel.Keywords);
		_command.Varchar("Skill", 2000, searchModel.Skills);
		_command.Bit("SearchState", !searchModel.CityZip);
		_command.Varchar("City", 30, searchModel.CityName);
		_command.Varchar("State", 1000, searchModel.StateID);
		_command.Int("Proximity", searchModel.Proximity);
		_command.TinyInt("ProximityUnit", searchModel.ProximityUnit);
		_command.Varchar("Eligibility", 10, searchModel.Eligibility);
		_command.Varchar("Reloc", 10, searchModel.Relocate);
		_command.Varchar("JobOptions", 10, searchModel.JobOptions);
		//_command.Varchar("Communications",10, searchModel.Communication);
		_command.Varchar("Security", 10, searchModel.SecurityClearance);
		_command.Varchar("User", 10, searchModel.User);
		//_command.Bit("ActiveRequisitionsOnly", searchModel.ActiveRequisitionsOnly);
		//_command.Int("OptionalCandidateID", candidateID);
		//_command.Bit("ThenProceed", thenProceed);

		await _connection.OpenAsync();
		await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

		await _reader.ReadAsync();
		int _count = _reader.GetInt32(0);

		await _reader.NextResultAsync();
		while (await _reader.ReadAsync())
		{
			string _location = _reader.GetString(4);
			if (_location.StartsWith(","))
			{
				_location = _location[1..].Trim();
			}

			_candidates.Add(new()
							{
								ID = _reader.GetInt32(0), Name = _reader.GetString(1), Phone = _reader.GetString(2), Email = _reader.GetString(3), Location = _location, Updated = _reader.GetString(5),
								Status = _reader.GetString(6), MPC = _reader.GetBoolean(7), Rating = _reader.GetByte(8), OriginalResume = false, FormattedResume = false
							});
		}

		await _reader.CloseAsync();

		await _connection.CloseAsync();

		return new()
			   {
				   {
					   "Candidates", _candidates
				   },
				   {
					   "Count", _count
				   }
			   };
	}
}