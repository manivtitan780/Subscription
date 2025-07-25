#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           UploadCandidate.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-24-2025 20:07
// Last Updated On:     07-24-2025 20:40
// *****************************************/

#endregion

#region Using

#endregion

namespace Subscription.Server.Components.Pages.Controls.Candidates;

public partial class UploadCandidate : ComponentBase, IDisposable
{
    private bool _isCancelled;

    private CandidateDetails CandidateDetails { get; } = new();

    [Parameter]
    public EventCallback<CloseEventArgs> Close { get; set; }

    private EditContext Context { get; set; }

    private SfDialog Dialog { get; set; }

    private SfDataForm EditCandidateForm { get; set; }

    // Removed: Unused Model and validator properties

    private ParsedCandidate Model { get; } = new();

    [Parameter]
    public int RequisitionID { get; set; }

    [Parameter]
    public bool ShowSubmissions { get; set; }

    [Parameter]
    public string User { get; set; } = "";

    private bool VisibleSpinner { get; set; }

    /// <summary>
    ///     Memory optimization: Clean disposal pattern
    /// </summary>
    public void Dispose()
    {
        // No resources to dispose - RecyclableMemoryStreams are handled locally in methods
        GC.SuppressFinalize(this);
    }

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
        // Removed: Commented Context.NotifyFieldChanged call
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

        // Removed: Commented Context.NotifyFieldChanged call
    }

    protected override void OnParametersSet()
    {
        // Memory optimization: Only create new EditContext if Model reference changed
        if (Context?.Model != Model)
        {
            Context = null; // Immediate reference cleanup for GC
            Context = new(Model);
        }

        base.OnParametersSet();
    }

    public async Task ShowDialog() => await Dialog.ShowAsync();

    private async Task UploadCandidateResume()
    {
        if (CandidateDetails.TextResume.NotNullOrWhiteSpace())
        {
            try
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
            }
            catch (Exception ex)
            {
                // Exception handling: Log and continue - non-critical AI parsing failure
                Log.Error(ex, "Failed to parse resume using Azure OpenAI for candidate upload");
            }

            // Memory optimization: Dictionary with exact capacity (1 key-value pair)
            Dictionary<string, string> _parameters = new(1)
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

    // Removed: Class-level AddedDocument property - using local variables for better memory management

    private async Task UploadDocument(UploadChangeEventArgs file)
    {
        string _resumeText = "";

        foreach (UploadFiles _file in file.Files)
        {
            try
            {
                // Memory optimization: Get ReusableMemoryStream from pool for each file
                RecyclableMemoryStream _addedDocument = ReusableMemoryStream.Get("candidate-upload");

                await using (Stream _str = _file.File.OpenReadStream(60 * 1024 * 1024)) //60MB maximum
                {
                    await _str.CopyToAsync(_addedDocument);
                }

                Model.FileName = _file.FileInfo.Name;
                Model.Mime = _file.FileInfo.MimeContentType;
                Model.DocumentBytes = _addedDocument.ToArray();

                string _extension = Path.GetExtension(_file.FileInfo.Name)?.ToLowerInvariant();
                _resumeText = _extension switch
                              {
                                  ".doc" or ".docx" or ".rtf" => _addedDocument.ReadFromWord(),
                                  ".pdf" => _addedDocument.ReadFromPdf(),
                                  ".txt" => await _addedDocument.ReadFromText(),
                                  _ => ""
                              };
                await _addedDocument.DisposeAsync();
            }
            catch (Exception ex)
            {
                // Exception handling: Log and continue - non-critical file processing failure
                Log.Error(ex, "Failed to process uploaded file {FileName} for candidate upload", _file.FileInfo.Name);
            }
        }

        CandidateDetails.TextResume = _resumeText;
    }
}