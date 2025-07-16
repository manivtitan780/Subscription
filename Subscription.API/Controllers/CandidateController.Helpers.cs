#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           CandidateController.Helpers.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-16-2025 16:00
// Last Updated On:     07-16-2025 16:00
// *****************************************/

#endregion

namespace Subscription.API.Controllers;

/// <summary>
/// Partial class containing helper methods for CandidateController.
/// Includes database execution helpers, email processing, and utility methods.
/// </summary>
public partial class CandidateController
{
    // Memory optimization: Centralized query execution helpers to eliminate connection leaks and code duplication
    private async Task<ActionResult<string>> ExecuteQueryAsync(string procedureName, Action<SqlCommand> parameterBinder, string logContext, string errorMessage)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new(procedureName, _connection);
        _command.CommandType = CommandType.StoredProcedure;

        parameterBinder(_command);

        string _result = "[]";
        try
        {
            await _connection.OpenAsync();
            _result = (await _command.ExecuteScalarAsync())?.ToString() ?? "[]";
        }
        catch (SqlException ex)
        {
            Log.Error(ex, "Error executing {logContext} query. {ExceptionMessage}", logContext, ex.Message);
            return StatusCode(500, errorMessage);
        }
        // Memory optimization: Removed manual CloseAsync() - await using handles disposal automatically

        return Ok(_result);
    }

    // Memory optimization: ExecuteReaderAsync helper for complex data retrieval operations
    private async Task<ActionResult<T>> ExecuteReaderAsync<T>(string procedureName, Action<SqlCommand> parameterBinder, Func<SqlDataReader, Task<T>> readerProcessor, string logContext,
                                                              string errorMessage)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new(procedureName, _connection);
        _command.CommandType = CommandType.StoredProcedure;

        parameterBinder(_command);

        try
        {
            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();
            T _result = await readerProcessor(_reader);
            return Ok(_result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error executing {logContext} query. {ExceptionMessage}", logContext, ex.Message);
            return StatusCode(500, errorMessage);
        }
        // Memory optimization: Removed manual CloseAsync() - await using handles disposal automatically
    }

    // Memory optimization: ExecuteScalarAsync helper for single value operations
    private async Task<ActionResult<T>> ExecuteScalarAsync<T>(string procedureName, Action<SqlCommand> parameterBinder, string logContext, string errorMessage)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new(procedureName, _connection);
        _command.CommandType = CommandType.StoredProcedure;

        parameterBinder(_command);

        try
        {
            await _connection.OpenAsync();
            object _result = await _command.ExecuteScalarAsync();
            return Ok((T)Convert.ChangeType(_result ?? default(T), typeof(T)));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error executing {logContext} query. {ExceptionMessage}", logContext, ex.Message);
            return StatusCode(500, errorMessage);
        }
        // Memory optimization: Removed manual CloseAsync() - await using handles disposal automatically
    }

    private string GetMimeType(string fileName)
    {
        string extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt" => "text/plain",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };
    }

    // Memory optimization: Centralized email processing helper to eliminate code duplication
    private async Task ProcessEmailTemplatesAsync(List<EmailTemplates> templates, Dictionary<string, string> replacements, byte[] attachmentBytes = null, string attachmentName = null, string mimeType = null)
    {
        if (templates.Count == 0) return;

        foreach (EmailTemplates template in templates)
        {
            // Apply replacements to template
            string subject = template.Subject;
            string body = template.Template;

            foreach (KeyValuePair<string, string> replacement in replacements)
            {
                subject = subject.Replace(replacement.Key, replacement.Value);
                body = body.Replace(replacement.Key, replacement.Value);
            }

            // Memory optimization: Use injected SmtpClient instead of creating new instances
            using MailMessage mailMessage = new()
            {
                From = new("jolly@hire-titan.com", "Mani Bhai"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add("manivenkit@gmail.com"); // TODO: After testing, update with actual recipients

            // Add attachment if provided
            if (attachmentBytes != null && !string.IsNullOrEmpty(attachmentName))
            {
                MemoryStream attachmentStream = new(attachmentBytes);
                Attachment attachment = new(attachmentStream, attachmentName, mimeType ?? "application/octet-stream");
                mailMessage.Attachments.Add(attachment);
            }

            await smtpClient.SendMailAsync(mailMessage);
        }
    }

    // Memory optimization: Centralized candidate email processing helper
    private async Task ProcessCandidateEmailAsync(List<EmailTemplates> templates, CandidateDetails candidateDetails, string stateName, string userName, byte[] attachmentBytes = null, string attachmentName = null, string mimeType = null)
    {
        Dictionary<string, string> replacements = new()
        {
            {"$TODAY$", DateTime.Today.CultureDate()},
            {"$CANDIDATE_NAME$", $"{candidateDetails.FirstName} {candidateDetails.LastName}"},
            {"$CANDIDATE_EMAIL$", candidateDetails.Email},
            {"$USER_NAME$", userName},
            {"$STATE$", stateName}
        };

        await ProcessEmailTemplatesAsync(templates, replacements, attachmentBytes, attachmentName, mimeType);
    }

    // Memory optimization: Centralized requisition email processing helper
    private async Task ProcessRequisitionEmailAsync(List<EmailTemplates> templates, string firstName, string lastName, string reqCode, string reqTitle, string company, string submissionNotes, string submissionStatus, string user, byte[] attachmentBytes = null, string attachmentName = null)
    {
        Dictionary<string, string> replacements = new()
        {
            {"$TODAY$", DateTime.Today.CultureDate()},
            {"$CANDIDATE_NAME$", $"{firstName} {lastName}"},
            {"$REQ_CODE$", reqCode},
            {"$REQ_TITLE$", reqTitle},
            {"$COMPANY$", company},
            {"$SUBMISSION_NOTES$", submissionNotes},
            {"$SUBMISSION_STATUS$", submissionStatus},
            {"$USER_NAME$", user}
        };

        await ProcessEmailTemplatesAsync(templates, replacements, attachmentBytes, attachmentName);
    }
}