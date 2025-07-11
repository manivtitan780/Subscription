/*
    Code Added by Gemini.
    Reason: This code-behind file supports the new CompanyInfoPanel component.
    It encapsulates the parameters and display helper methods that were previously in the main Companies.razor.cs file.
    This improves modularity and makes the parent component cleaner.
*/
namespace Subscription.Server.Components.Pages.Controls.Companies;

public partial class CompanyInfoPanel
{
    [Parameter]
    public CompanyDetails Model
    {
        get;
        set;
    }

    [Parameter]
    public MarkupString Address
    {
        get;
        set;
    }

    public static string FormatDUNS(string input)
    {
        if (input.NullOrWhiteSpace() || input.Length != 9)
        {
            return input;
        }

        return $"{input[..2]}-{input.Substring(2, 3)}-{input[5..]}";
    }

    public static string FormatEIN(string input)
    {
        if (input.NullOrWhiteSpace() || input.Length != 9)
        {
            return input;
        }

        return $"{input[..2]}-{input[2..]}";
    }
}
