#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Download.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          04-30-2025 16:04
// Last Updated On:     04-30-2025 20:43
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages;

public partial class Download
{
    [Parameter]
    public string File { get; set; }

    [Inject]
    private HttpClient HttpClient { get; set; }

    [Inject]
    private IJSRuntime JsRuntime { get; set; }

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
        byte[] _fileBytes = await General.ReadFromBlob(_blobPath);
        // Memory optimization: Use ArrayPool-based Base64 conversion for file downloads
        await JsRuntime.InvokeVoidAsync("downloadFileFromBytes", _decodedStringArray[2], Extensions.Memory.Base64Helper.ConvertToBase64Efficiently(_fileBytes));
    }
}