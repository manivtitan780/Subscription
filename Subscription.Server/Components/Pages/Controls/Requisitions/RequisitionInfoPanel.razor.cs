#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_AppTrack
// File Name:           BasicInfoRequisitionPanel.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          12-09-2022 15:57
// Last Updated On:     10-02-2023 18:52
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Requisitions;

/// <summary>
///     Represents a panel for displaying and managing basic information about a requisition.
/// </summary>
/// <remarks>
///     This class is a part of the ProfSvc_AppTrack.Pages.Controls.Requisitions namespace and is used in the context of
///     managing requisitions.
///     It provides properties for managing the height of the panel, details of the requisition, skills text in a markup
///     format, and a list of integer values representing different states.
/// </remarks>
public partial class RequisitionInfoPanel
{
    /// <summary>
    ///     Gets or sets the height of the BasicInfoRequisitionPanel.
    /// </summary>
    /// <value>
    ///     A string representing the height of the panel. The default value is "450px".
    /// </value>
    [Parameter]
    public string Height
    {
        get;
        set;
    } = "450px";

    /// <summary>
    ///     Gets or sets the details of the requisition.
    /// </summary>
    /// <value>
    ///     The details of the requisition.
    /// </value>
    [Parameter]
    public RequisitionDetails Model
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Gets or sets the skills text in a markup format for the BasicInfoRequisitionPanel.
    /// </summary>
    /// <value>
    ///     The skills text in a markup format.
    /// </value>
    [Parameter]
    public MarkupString SkillsText
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets a list of integer values representing different states.
    /// </summary>
    /// <value>
    ///     The list of states, each represented by an instance of the IntValues class.
    /// </value>
    [Parameter]
    public List<IntValues> States
    {
        get;
        set;
    }

    /// <summary>
    ///     Converts a short form duration code into its full form.
    /// </summary>
    /// <param name="durationCode">
    ///     The short form duration code. Can be "m" for months, "w" for weeks, "d" for days. Any other
    ///     value will be considered as years.
    /// </param>
    /// <returns>The full form of the duration code.</returns>
    private string GetDurationCode(string durationCode)
    {
        return durationCode.ToLower() switch
               {
                   "m" => "months",
                   "w" => "weeks",
                   "d" => "days",
                   _ => "years"
               };
    }

    /// <summary>
    ///     Gets the location name based on the provided location key.
    /// </summary>
    /// <param name="location">The location key as a string.</param>
    /// <returns>The location name if found, otherwise returns the provided location key.</returns>
    private string GetLocation(string location)
    {
        if (States == null || location.ToInt32() == 0)
        {
            return location;
        }

        foreach (IntValues _intValues in States.Where(intValues => location.ToInt32() == intValues.KeyValue))
        {
            return _intValues.Text;
        }

        return location;
    }

    /// <summary>
    ///     Asynchronously sets the parameters for the BasicInfoRequisitionPanel.
    /// </summary>
    /// <remarks>
    ///     This method ensures that the Model property is not null before calling the base implementation of
    ///     OnParametersSetAsync.
    ///     If the Model property is null, a new instance of RequisitionDetails is assigned to it.
    /// </remarks>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    protected override async Task OnParametersSetAsync()
    {
        while (Model == null)
        {
            Model = new();
        }

        await base.OnParametersSetAsync();
    }
}