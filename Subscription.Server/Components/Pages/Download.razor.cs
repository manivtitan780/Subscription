#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Profsvc_AppTrack
// Project:             Profsvc_AppTrack.Client
// File Name:           Download.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          1-3-2024 20:12
// Last Updated On:     1-3-2024 20:15
// *****************************************/

#endregion


namespace Subscription.Server.Components.Pages;

public partial class Download
{
    [Parameter]
    public string File
    {
        get;
        set;
    }

    [Inject]
    private HttpClient HttpClient
    {
        get;
        set;
    }

    [Inject]
    private IJSRuntime JsRuntime
    {
        get;
        set;
    }

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(File))
        {
            return;
        }

        string[] _decodedStringArray = File.FromBase64String().Split('^');
        if (_decodedStringArray.Length != 4)
        {
            return;
        }

        string _type = _decodedStringArray[3] switch
                      {
                          "1" => "Requisition",
                          "2" => "Company",
                          "3" => "Lead",
                          _ => "Candidate"
                      };
        string _blobPath = $"{Start.AzureBlobContainer}/{_type}/{_decodedStringArray[1]}/{_decodedStringArray[0]}";
        byte[] _fileBytes = await General.ReadFromBlob(_blobPath); // System.IO.File.ReadAllBytes(_filePath);

        await JsRuntime.InvokeVoidAsync("downloadFileFromBytes", _decodedStringArray[2], Convert.ToBase64String(_fileBytes));
    }
}