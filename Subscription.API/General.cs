#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           General.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          4-17-2024 14:39
// Last Updated On:     4-17-2024 20:54
// *****************************************/

#endregion

#region Using

using System.Data.Common;

using ProfSvc_Classes;

#endregion

namespace Subscription.API;

public static class General
{
	public static async Task SetCache()
	{
		RedisService _service = new(Start.CacheServer, Start.CachePort.ToInt32(), Start.Access, false);
		bool _keyExists = await _service.CheckKeyExists(CacheObjects.Companies.ToString());

		if (!_keyExists)
		{
			await using SqlConnection _connection = new(Start.ConnectionString);
			await _connection.OpenAsync();

			await using SqlCommand _command = new("SetCache", _connection);
			_command.CommandType = CommandType.StoredProcedure;
			_command.Bit("@Companies", true);
			_command.Bit("@CompanyContact", true);
			_command.Bit("@Designations", true);
			_command.Bit("@DocumentType", true);
			_command.Bit("@Education", true);
			_command.Bit("@Eligibility", true);
			_command.Bit("@Experience", true);
			_command.Bit("@JobOptions", true);
			_command.Bit("@LeadIndustry", true);
			_command.Bit("@LeadSource", true);
			_command.Bit("@LeadStatus", true);
			_command.Bit("@NAICS", true);
			_command.Bit("@Roles", true);
			_command.Bit("@Skills", true);
			_command.Bit("@States", true);
			_command.Bit("@Status", true);
			_command.Bit("@TaxTerms", true);
			_command.Bit("@Users", true);
			_command.Bit("@Workflow", true);
			_command.Bit("@ZipCodes", true);

			await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

			List<IntValues> _companies = [];
			await SetIntValues(_reader, _companies);

			await _reader.NextResultAsync();
			List<CompanyContactList> _companyContacts = [];
			while (await _reader.ReadAsync())
			{
				_companyContacts.Add(new()
									 {
										 ID = _reader.GetInt32(0),
										 ContactName = _reader.GetString(2),
										 CompanyID = _reader.GetInt32(1)
									 });
			}

			await _reader.NextResultAsync();
			List<IntValues> _titles = [];
			await SetIntValues(_reader, _titles);

			await _reader.NextResultAsync();
			List<IntValues> _documentTypes = [];
			await SetIntValues(_reader, _documentTypes);

			await _reader.NextResultAsync();
			List<IntValues> _educations = [];
			await SetIntValues(_reader, _educations);

			await _reader.NextResultAsync();
			List<IntValues> _eligibilities = [];
			await SetIntValues(_reader, _eligibilities);

			await _reader.NextResultAsync();
			List<IntValues> _experiences = [];
			await SetIntValues(_reader, _experiences);

			await _reader.NextResultAsync();
			List<KeyValues> _jobOptions = [];
			await SetKeyValues(_reader, _jobOptions);

			await _reader.NextResultAsync();
			List<IntValues> _leadIndustries = [];
			await SetIntValues(_reader, _leadIndustries, 2);

			await _reader.NextResultAsync();
			List<IntValues> _leadSources = [];
			await SetIntValues(_reader, _leadSources, 2);

			await _reader.NextResultAsync();
			List<IntValues> _leadStatuses = [];
			await SetIntValues(_reader, _leadStatuses, 2);

			await _reader.NextResultAsync();
			List<IntValues> _naics = [];
			await SetIntValues(_reader, _naics);

			await _reader.NextResultAsync();
			List<Role> _roles = [];
			while (await _reader.ReadAsync()) //Roles
			{
				_roles.Add(new()
						   {
							   ID = _reader.GetString(0), RoleName = _reader.GetString(1), ViewCandidate = _reader.GetBoolean(2), ViewRequisition = _reader.GetBoolean(3),
							   EditCandidate = _reader.GetBoolean(4), EditRequisition = _reader.GetBoolean(5), ChangeCandidateStatus = _reader.GetBoolean(6),
							   ChangeRequisitionStatus = _reader.GetBoolean(7), SendEmailCandidate = _reader.GetBoolean(8), ForwardResume = _reader.GetBoolean(9),
							   DownloadResume = _reader.GetBoolean(10),
							   SubmitCandidate = _reader.GetBoolean(11), ViewClients = _reader.GetBoolean(12), EditClients = _reader.GetBoolean(13), Description = _reader.GetString(14)
						   });
			}

			await _reader.NextResultAsync();
			List<IntValues> _skills = [];
			await SetIntValues(_reader, _skills);

			await _reader.NextResultAsync();
			List<IntValues> _states = [];
			await SetIntValues(_reader, _states);

			await _reader.NextResultAsync();
			List<StatusCode> _statusCodes = [];
			while (await _reader.ReadAsync())
			{
				_statusCodes.Add(new()
								 {
									 ID = _reader.GetInt32(6), Code = _reader.GetString(0), Status = _reader.GetString(1), Icon = _reader.NString(2), AppliesToCode = _reader.GetString(3),
									 SubmitCandidate = _reader.GetBoolean(4), ShowCommission = _reader.GetBoolean(5)
								 });
			}

			await _reader.NextResultAsync();
			List<KeyValues> _taxTerms = [];
			await SetKeyValues(_reader, _taxTerms);

			await _reader.NextResultAsync();
			List<UserList> _users = [];
			while (await _reader.ReadAsync())
			{
				_users.Add(new()
						   {
							   ID = _reader.GetInt32(0), UserName = _reader.GetString(1), Role = _reader.GetByte(2)
						   });
			}

			await _reader.NextResultAsync();
			List<AppWorkflow> _workflows = [];
			while (await _reader.ReadAsync())
			{
				_workflows.Add(new()
							   {
								   ID = _reader.GetInt32(0), Step = _reader.GetString(1), Next = _reader.NString(2), IsLast = _reader.GetBoolean(3), RoleIDs = _reader.GetString(4),
								   Schedule = _reader.GetBoolean(5), AnyStage = _reader.GetBoolean(6), NextFull = "", RoleFull = ""
							   });
			}

			await _reader.NextResultAsync();
			List<Zip> _zips = [];
			while (await _reader.ReadAsync()) //Zips
			{
				_zips.Add(new(_reader.GetString(0), _reader.GetString(1), _reader.GetString(2), _reader.GetInt32(3)));
			}

			await _reader.CloseAsync();

			await _connection.CloseAsync();

			List<string> _keys =
			[
				CacheObjects.Companies.ToString(), CacheObjects.CompanyContacts.ToString(), CacheObjects.Titles.ToString(), CacheObjects.DocumentTypes.ToString(), CacheObjects.Education.ToString(),
				CacheObjects.Eligibility.ToString(), CacheObjects.Experience.ToString(), CacheObjects.JobOptions.ToString(), CacheObjects.LeadIndustries.ToString(),
				CacheObjects.LeadSources.ToString(), CacheObjects.LeadStatus.ToString(), CacheObjects.NAICS.ToString(), CacheObjects.Roles.ToString(), CacheObjects.Skills.ToString(),
				CacheObjects.States.ToString(), CacheObjects.StatusCodes.ToString(), CacheObjects.TaxTerms.ToString(), CacheObjects.Users.ToString(), CacheObjects.Workflow.ToString(),
				CacheObjects.Zips.ToString()
			];

			List<object> _values =
			[
				_companies, _companyContacts, _titles, _documentTypes, _educations, _eligibilities, _experiences, _jobOptions, _leadIndustries, _leadSources, _leadStatuses, _naics, _roles, _skills,
				_states, _statusCodes, _taxTerms, _users, _workflows, _zips
			];

			await _service.CreateBatchSet(_keys, _values);
		}
	}

	private static async Task SetIntValues(DbDataReader reader, ICollection<IntValues> intValues, byte keyType = 0) //0-Int32, 1=Int16, 2=Byte
	{
		while (await reader.ReadAsync())
		{
			intValues.Add(new()
						  {
							  Value = keyType switch
									{
										0 => reader.GetInt32(0),
										1 => reader.GetInt16(0),
										2 => reader.GetByte(0),
										_ => 0
									},
							  Text = reader.GetString(1)
						  });
		}
	}

	private static async Task SetKeyValues(DbDataReader reader, ICollection<KeyValues> keyValues)
	{
		while (await reader.ReadAsync()) //Job Options
		{
			keyValues.Add(new()
						  {
							  Key = reader.GetString(0),
							  Value = reader.GetString(1)
						  });
		}
	}
}