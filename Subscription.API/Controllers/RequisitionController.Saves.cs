#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           RequisitionController.Saves.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-19-2025 20:07
// Last Updated On:     07-19-2025 20:27
// *****************************************/

#endregion

namespace Subscription.API.Controllers;

public partial class RequisitionController
{
    [HttpPost]
    public async Task<ActionResult<string>> ChangeRequisitionStatus(int requisitionID, string statusCode, string user)
    {
        return await ExecuteQueryAsync("ChangeRequisitionStatus", command =>
                                                                  {
                                                                      command.Int("RequisitionID", requisitionID);
                                                                      command.Char("Status", 3, statusCode);
                                                                      command.Varchar("User", 10, user);
                                                                  }, "ChangeRequisitionStatus", "Error changing requisition status.");
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveNotes(CandidateNotes candidateNote, int requisitionID, string user)
    {
        if (candidateNote == null)
        {
            return Ok("[]");
        }

        return await ExecuteQueryAsync("SaveNote", command =>
                                                   {
                                                       command.Int("Id", candidateNote.ID);
                                                       command.Int("CandidateID", requisitionID);
                                                       command.Varchar("Note", -1, candidateNote.Notes);
                                                       command.Varchar("EntityType", 5, "REQ");
                                                       command.Varchar("User", 10, user);
                                                   }, "SaveNotes", "Error saving notes.");
    }

    [HttpPost]
    public async Task<ActionResult<int>> SaveRequisition(RequisitionDetails requisition, string user, string jsonPath = "", string emailAddress = "maniv@titan-techs.com")
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("SaveRequisition", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("RequisitionId", requisition.RequisitionID, true);
        _command.Int("Company", requisition.CompanyID);
        _command.Int("HiringMgr", requisition.ContactID);
        _command.Varchar("City", 50, requisition.City);
        _command.Int("StateId", requisition.StateID);
        _command.Varchar("Zip", 10, requisition.ZipCode);
        _command.TinyInt("IsHot", requisition.PriorityID);
        _command.Varchar("Title", 200, requisition.PositionTitle);
        _command.Varchar("Description", -1, requisition.Description);
        _command.Int("Positions", requisition.Positions);
        _command.DateTime("ExpStart", requisition.ExpectedStart);
        _command.DateTime("Due", requisition.DueDate);
        _command.Int("Education", requisition.EducationID);
        _command.Varchar("Skills", 2000, requisition.SkillsRequired);
        _command.Varchar("OptionalRequirement", 8000, requisition.Optional);
        _command.Char("JobOption", 1, requisition.JobOptionID);
        _command.Int("ExperienceID", requisition.ExperienceID);
        _command.Int("Eligibility", requisition.EligibilityID);
        _command.Varchar("Duration", 50, requisition.Duration);
        _command.Char("DurationCode", 1, requisition.DurationCode);
        _command.Decimal("ExpRateLow", 9, 2, requisition.ExpRateLow);
        _command.Decimal("ExpRateHigh", 9, 2, requisition.ExpRateHigh);
        _command.Decimal("ExpLoadLow", 9, 2, requisition.ExpLoadLow);
        _command.Decimal("ExpLoadHigh", 9, 2, requisition.ExpLoadHigh);
        _command.Decimal("SalLow", 9, 2, requisition.SalaryLow);
        _command.Decimal("SalHigh", 9, 2, requisition.SalaryHigh);
        _command.Bit("ExpPaid", requisition.ExpensesPaid);
        _command.Char("Status", 3, requisition.StatusCode);
        _command.Bit("Security", requisition.SecurityClearance);
        _command.Decimal("PlacementFee", 8, 2, requisition.PlacementFee);
        _command.Varchar("BenefitsNotes", -1, requisition.BenefitNotes);
        _command.Bit("OFCCP", requisition.OFCCP);
        _command.Varchar("User", 10, user);
        _command.Varchar("Assign", 550, requisition.AssignedTo);
        _command.Varchar("MandatoryRequirement", 8000, requisition.Mandatory);
        try
        {
            string _reqCode = "";

            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            if (await _reader.ReadAsync())
            {
                _reqCode = _reader.NString(0);
            }

            await _reader.NextResultAsync();
            List<EmailTemplates> _templates = [];
            Dictionary<string, string> _emailAddresses = new(), _emailCC = new();

            while (await _reader.ReadAsync())
            {
                _templates.Add(new(_reader.NString(0), _reader.NString(1), _reader.NString(2)));
            }

            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _emailAddresses.Add(_reader.NString(0), _reader.NString(1));
            }

            await _reader.NextResultAsync();
            string _stateName = "";
            if (await _reader.ReadAsync())
            {
                _stateName = _reader.GetString(0);
            }

            if (_templates.Count <= 0)
            {
                return Ok(0);
            }

            EmailTemplates _templateSingle = _templates[0];
            if (!_templateSingle.CC.NullOrWhiteSpace())
            {
                string[] _ccArray = _templateSingle.CC.Split(",");
                foreach (string _cc in _ccArray)
                {
                    _emailCC.Add(_cc, _cc);
                }
            }

            _templateSingle.Subject = _templateSingle.Subject.Replace("{TODAY}", DateTime.Today.CultureDate())
                                                     .Replace("{RequisitionID}", _reqCode)
                                                     .Replace("{RequisitionTitle}", requisition.PositionTitle)
                                                     .Replace("{Company}", requisition.CompanyName)
                                                     .Replace("{RequisitionDescription}", requisition.Description)
                                                     .Replace("{RequisitionLocation}", GenerateLocation(requisition, _stateName))
                                                     .Replace("{LoggedInUser}", user);
            _templateSingle.Template = _templateSingle.Template.Replace("{TODAY}", DateTime.Today.CultureDate())
                                                      .Replace("{RequisitionID}", _reqCode)
                                                      .Replace("{RequisitionTitle}", requisition.PositionTitle)
                                                      .Replace("{Company}", requisition.CompanyName)
                                                      .Replace("{RequisitionDescription}", requisition.Description)
                                                      .Replace("{RequisitionLocation}", GenerateLocation(requisition, _stateName))
                                                      .Replace("{LoggedInUser}", user);

            MailMessage _mailMessage = new()
                                       {
                                           From = new("maniv@hire-titan.com", "Mani-Meow"),
                                           Subject = _templateSingle.Subject,
                                           Body = _templateSingle.Template,
                                           IsBodyHtml = true
                                       };
            _mailMessage.To.Add("manivenkit@gmail.com"); //TODO: After testing remove this and enable the below code
            // _mailMessage.To.Add("jolly@hire-titan.com");
            /*foreach (KeyValues _emailAddress in _emailAddresses)
            {
                _mailMessage.To.Add(new MailAddress(_emailAddress.KeyValue, _emailAddress.Text));
            }
            foreach (KeyValues _cc in _emailCC)
            {
                _mailMessage.CC.Add(new MailAddress(_cc.KeyValue, _cc.Text));
            }*/

            await smtpClient.SendMailAsync(_mailMessage);
            return Ok(0);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving requisition. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost, RequestSizeLimit(62914560)]
    public async Task<ActionResult<string>> UploadDocument(IFormFile file)
    {
        string _fileName = file.FileName; //Request.Form["filename"];
        string _requisitionID = Request.Form["requisitionID"].ToString();
        string _internalFileName = Guid.NewGuid().ToString("N");

        // Create a BlobStorage instance
        IAzureBlobStorage _storage = StorageFactory.Blobs.AzureBlobStorageWithSharedKey(Start.AccountName, Start.AzureKey);

        // Create the folder path
        string _blobPath = $"{Start.AzureBlobContainer}/Requisition/{_requisitionID}/{_internalFileName}";

        await using (Stream stream = file.OpenReadStream())
        {
            await _storage.WriteAsync(_blobPath, stream);
        }

        await using SqlConnection _connection = new(Start.ConnectionString);
        List<RequisitionDocuments> _documents = new();
        try
        {
            string _returnVal = "[]";

            await using SqlCommand _command = new("SaveRequisitionDocuments", _connection);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Int("RequisitionId", _requisitionID);
            _command.Varchar("DocumentName", 255, Request.Form["name"].ToString());
            _command.Varchar("DocumentLocation", 255, _fileName);
            _command.Varchar("DocumentNotes", 2000, Request.Form["notes"].ToString());
            _command.Varchar("InternalFileName", 50, _internalFileName);
            _command.Varchar("DocsUser", 10, Request.Form["user"].ToString());
            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();
            while (_reader.Read())
            {
                _returnVal = _reader.NString(0);
            }

            return Ok(_returnVal);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving requisition document. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}