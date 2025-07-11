#region Header
// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Extensions
// File Name:           Extensions.Streams.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          05-07-2025 19:05
// Last Updated On:     05-07-2025 19:46
// *****************************************/
#endregion

using System.Text;

using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;

namespace Extensions;

public static partial class Extensions
{
    public static string ReadFromWord(this Stream stream)
    {
        using WordDocument _document = new(stream, FormatType.Automatic);
        string _resumeText = _document.GetText();
        _document.Close();
        return _resumeText;
    }
    
    public static string ReadFromPdf(this Stream stream)
    {
        using PdfLoadedDocument _document = new(stream);
        // Pre-allocate StringBuilder with estimated capacity based on page count
        StringBuilder _resumeText = new(_document.Pages.Count * 1024);
        
        foreach (PdfLoadedPage page in _document.Pages)
        {
            _resumeText.Append(page.ExtractText());
        }

        _document.Close();
        return _resumeText.ToString();
    }
    
    public static async Task<string> ReadFromText(this Stream stream)
    {
        using StreamReader reader = new(stream);
        string text = await reader.ReadToEndAsync();
        return text;
    }
}