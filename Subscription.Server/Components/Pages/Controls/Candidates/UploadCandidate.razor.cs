#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           UploadCandidate.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          05-07-2025 16:05
// Last Updated On:     05-21-2025 15:05
// *****************************************/

#endregion

#region Using

using System.Text.Json;

#endregion

namespace Subscription.Server.Components.Pages.Controls.Candidates;

public partial class UploadCandidate : ComponentBase
{
    private bool _isCancelled;

    private CandidateDetails CandidateDetails { get; } = new();

    [Parameter]
    public EventCallback<CloseEventArgs> Close { get; set; }

    private EditContext Context { get; set; }

    private SfDialog Dialog { get; set; }

    private SfDataForm EditCandidateForm { get; set; }

    // Add the missing properties
    //private object Model { get; set; } = new();

    //private FluentValidationValidator _candidateDetailsValidator { get; set; }

    private ParsedCandidate Model { get; } = new();

    [Parameter]
    public int RequisitionID { get; set; }

    [Parameter]
    public bool ShowSubmissions { get; set; }

    [Parameter]
    public string User { get; set; } = "";

    private bool VisibleSpinner { get; set; }

    private async Task CancelDialog(MouseEventArgs args)
    {
        _isCancelled = true;
        await Dialog.HideAsync();
    }

    private async Task CloseDialog(CloseEventArgs args)
    {
        if (!_isCancelled)
        {
            await Close.InvokeAsync(args);
        }
    }

    // Event handlers for uploader
    private void OnFileRemoved(RemovingEventArgs args)
    {
        Model.Files = null;
        //Context.NotifyFieldChanged(Context.Field(nameof(Model.Files)));
    }

    private void OnFileSelected(SelectedEventArgs file)
    {
        if (Model.Files is null)
        {
            Model.Files = [file.FilesData[0].Name];
        }
        else
        {
            Model.Files.Clear();
            Model.Files.Add(file.FilesData[0].Name);
        }

        //Context.NotifyFieldChanged(Context.Field(nameof(Model.Files)));
    }

    public async Task ShowDialog() => await Dialog.ShowAsync();

    private async Task UploadCandidateResume()
    {
        if (CandidateDetails.TextResume.NotNullOrWhiteSpace())
        {
            RestClient client = new(Start.AzureOpenAIEndpoint);
            RestRequest request = new("", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("api-key", Start.AzureOpenAIKey);
            var requestBody = new
                              {
                                  messages = new[]
                                             {
                                                 new {role = "system", content = "You are a concise resume summarizer."},
                                                 new {role = "user", content = $"{Start.Prompt}{CandidateDetails.TextResume}"}
                                             },
                                  temperature = 0.3,
                                  max_tokens = 1000
                              };

            request.AddJsonBody(requestBody);

            RestResponse response = await client.ExecuteAsync(request).ConfigureAwait(false);

            if (response.IsSuccessful)
            {
                using JsonDocument _doc = JsonDocument.Parse(response.Content ?? string.Empty);
                string _content = _doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                if (_content != null)
                {
                    JsonDocument _json = JsonDocument.Parse(_content);
                    JsonElement _root = _json.RootElement;

                    CandidateDetails.Keywords = _root.TryGetProperty("Keywords", out JsonElement kw) ? kw.GetString() : "";
                    CandidateDetails.Summary = _root.TryGetProperty("Summary", out JsonElement sum) ? sum.GetString() : "";
                    CandidateDetails.Title = _root.TryGetProperty("Title", out JsonElement title) ? title.GetString() : "";
                    CandidateDetails.FirstName = _root.TryGetProperty("FirstName", out JsonElement fName) ? fName.GetString() : "";
                    CandidateDetails.LastName = _root.TryGetProperty("LastName", out JsonElement lName) ? lName.GetString() : "";
                    CandidateDetails.Address1 = _root.TryGetProperty("Address", out JsonElement addr) && addr.GetString().NotNullOrWhiteSpace() ? addr.GetString() : "";
                    CandidateDetails.City = _root.TryGetProperty("City", out JsonElement city) && city.GetString().NotNullOrWhiteSpace() ? city.GetString() : "";
                    CandidateDetails.ZipCode = _root.TryGetProperty("Zip", out JsonElement zip) && zip.GetString().NotNullOrWhiteSpace() ? zip.GetString() : "";
                    CandidateDetails.Email = _root.TryGetProperty("Email", out JsonElement email) && email.GetString().NotNullOrWhiteSpace() ? email.GetString() : "";
                    CandidateDetails.Phone1 = _root.TryGetProperty("Phone", out JsonElement phone) && phone.GetString().NotNullOrWhiteSpace() ? phone.GetString() : "";
                    if (Model.SubmissionNotes.NullOrWhiteSpace())
                    {
                        Model.SubmissionNotes = CandidateDetails.Summary;
                    }
                    Model.RequisitionID = RequisitionID;
                }
            }

            Dictionary<string, string> _parameters = new()
                                                     {
                                                         {"userName", User}
                                                     };

            CandidateDetailsResume _candDetailsResume = new()
                                                        {
                                                            CandidateDetails = CandidateDetails,
                                                            ParsedCandidate = Model
                                                        };

            await General.ExecuteRest<int>("Candidate/SaveCandidateWithResume", _parameters, _candDetailsResume).ConfigureAwait(false);
            Model.Clear();
            await Dialog.HideAsync().ConfigureAwait(false);
        }
    }

    private async Task UploadDocument(UploadChangeEventArgs file)
    {
        string _resumeText = "";
        MemoryStream _addedDocument = new(60 * 1024 * 1024);
        foreach (UploadFiles _file in file.Files)
        {
            _addedDocument.SetLength(0);
            Stream _str = _file.File.OpenReadStream(60 * 1024 * 1024);
            await _str.CopyToAsync(_addedDocument);
            Model.FileName = _file.FileInfo.Name;
            Model.Mime = _file.FileInfo.MimeContentType;
            Model.DocumentBytes = _addedDocument.ToArray();
            _addedDocument.Position = 0;
            _str.Close();
            string _extension = Path.GetExtension(_file.FileInfo.Name)?.ToLowerInvariant();
            _resumeText = _extension switch
                          {
                              ".doc" or ".docx" or ".rtf" => _addedDocument.ReadFromWord(),
                              ".pdf" => _addedDocument.ReadFromPdf(),
                              ".txt" => await _addedDocument.ReadFromText(),
                              _ => ""
                          };
        }

        _addedDocument.Close();

        CandidateDetails.TextResume = _resumeText;
    }
}