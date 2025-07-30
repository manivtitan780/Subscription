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
            return NotFound("[]");
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
        if (requisition == null)
        {
            return BadRequest("Requisition details cannot be null.");
        }

        try
        {
            string _reqCode = "";
            List<EmailTemplates> _templates = new();
            Dictionary<string, string> _emailAddresses = new();
            string _stateName = "";

            await ExecuteReaderAsync("SaveRequisition", command =>
            {
                command.Int("RequisitionId", requisition.RequisitionID, true);
                command.Int("Company", requisition.CompanyID);
                command.Int("HiringMgr", requisition.ContactID);
                command.Varchar("City", 50, requisition.City);
                command.Int("StateId", requisition.StateID);
                command.Varchar("Zip", 10, requisition.ZipCode);
                command.TinyInt("IsHot", requisition.PriorityID);
                command.Varchar("Title", 200, requisition.PositionTitle);
                command.Varchar("Description", -1, requisition.Description);
                command.Int("Positions", requisition.Positions);
                command.DateTime("ExpStart", requisition.ExpectedStart);
                command.DateTime("Due", requisition.DueDate);
                command.Int("Education", requisition.EducationID);
                command.Varchar("Skills", 2000, requisition.SkillsRequired);
                command.Varchar("OptionalRequirement", 8000, requisition.Optional);
                command.Char("JobOption", 1, requisition.JobOptionID);
                command.Int("ExperienceID", requisition.ExperienceID);
                command.Int("Eligibility", requisition.EligibilityID);
                command.Varchar("Duration", 50, requisition.Duration);
                command.Char("DurationCode", 1, requisition.DurationCode);
                command.Decimal("ExpRateLow", 9, 2, requisition.ExpRateLow);
                command.Decimal("ExpRateHigh", 9, 2, requisition.ExpRateHigh);
                command.Decimal("ExpLoadLow", 9, 2, requisition.ExpLoadLow);
                command.Decimal("ExpLoadHigh", 9, 2, requisition.ExpLoadHigh);
                command.Decimal("SalLow", 9, 2, requisition.SalaryLow);
                command.Decimal("SalHigh", 9, 2, requisition.SalaryHigh);
                command.Bit("ExpPaid", requisition.ExpensesPaid);
                command.Char("Status", 3, requisition.StatusCode);
                command.Bit("Security", requisition.SecurityClearance);
                command.Decimal("PlacementFee", 8, 2, requisition.PlacementFee);
                command.Varchar("BenefitsNotes", -1, requisition.BenefitNotes);
                command.Bit("OFCCP", requisition.OFCCP);
                command.Varchar("User", 10, user);
                command.Varchar("Assign", 550, requisition.AssignedTo);
                command.Varchar("MandatoryRequirement", 8000, requisition.Mandatory);
            }, async reader =>
            {
                if (await reader.ReadAsync())
                {
                    _reqCode = reader.NString(0);
                }

                await reader.NextResultAsync();
                while (await reader.ReadAsync())
                {
                    _templates.Add(new(reader.NString(0), reader.NString(1), reader.NString(2)));
                }

                await reader.NextResultAsync();
                while (await reader.ReadAsync())
                {
                    _emailAddresses.Add(reader.NString(0), reader.NString(1));
                }

                await reader.NextResultAsync();
                if (await reader.ReadAsync())
                {
                    _stateName = reader.GetString(0);
                }

                return Task.CompletedTask; // Dummy return
            }, "SaveRequisition", "Error saving requisition data.");

            if (_templates.Count > 0)
            {
                EmailTemplates _templateSingle = _templates[0];

                string location = GenerateLocation(requisition, _stateName);
                var replacements = new Dictionary<string, string>
                {
                    { "{TODAY}", DateTime.Today.CultureDate() },
                    { "{RequisitionID}", _reqCode },
                    { "{RequisitionTitle}", requisition.PositionTitle },
                    { "{Company}", requisition.CompanyName },
                    { "{RequisitionDescription}", requisition.Description },
                    { "{RequisitionLocation}", location },
                    { "{LoggedInUser}", user }
                };

                int totalReplacementLength = replacements.Values.Sum(v => v?.Length ?? 0);

                StringBuilder subjectBuilder = new(_templateSingle.Subject, _templateSingle.Subject.Length + totalReplacementLength);
                StringBuilder bodyBuilder = new(_templateSingle.Template, _templateSingle.Template.Length + totalReplacementLength);

                foreach (var (key, value) in replacements)
                {
                    subjectBuilder.Replace(key, value);
                    bodyBuilder.Replace(key, value);
                }

                MailMessage _mailMessage = new()
                {
                    From = new("maniv@hire-titan.com", "Mani-Meow"),
                    Subject = subjectBuilder.ToString(),
                    Body = bodyBuilder.ToString(),
                    IsBodyHtml = true
                };

                _mailMessage.To.Add("manivenkit@gmail.com"); //TODO: After testing remove this and enable the below code

                await smtpClient.SendMailAsync(_mailMessage);
            }

            return Ok(0);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving requisition. {ExceptionMessage}", ex.Message);
            return StatusCode(500, "An unexpected error occurred while saving the requisition.");
        }
    }

    [HttpPost, RequestSizeLimit(62914560)]
    public async Task<ActionResult<string>> UploadDocument(IFormFile file)
    {
        if (file == null)
        {
            return BadRequest("File cannot be null.");
        }

        try
        {
            string _reuisitionID = Request.Form["requisitionID"].ToString();
            string _internalFileName = Guid.NewGuid().ToString("N");

            string _blobPath = $"{Start.AzureBlobContainer}/Requisition/{_reuisitionID}/{_internalFileName}";
            await General.UploadToBlob(file, _blobPath); // Assuming General.UploadToBlob is a static helper

            return await ExecuteQueryAsync("SaveRequisitionDocuments", command =>
            {
                command.Int("RequisitionId", _reuisitionID.ToInt32());
                command.Varchar("DocumentName", 255, Request.Form["name"].ToString());
                command.Varchar("DocumentLocation", 255, file.FileName);
                command.Varchar("DocumentNotes", 2000, Request.Form["notes"].ToString());
                command.Varchar("InternalFileName", 50, _internalFileName);
                command.Varchar("DocsUser", 10, Request.Form["user"].ToString());
            }, "UploadDocument", "An unexpected error occurred while uploading the document.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error uploading requisition document. {ExceptionMessage}", ex.Message);
            return StatusCode(500, "An unexpected error occurred while uploading the document.");
        }
    }
}