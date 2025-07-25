#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           CandidateInfoPanel.razor.cs
// Created By:          Claude Code (Anthropic)
// Created On:          07-25-2025
// Last Updated On:     07-25-2025
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Candidates;

/// <summary>
///     The CandidateInfoPanel component displays detailed basic information for a candidate,
///     including personal details, contact information, rates, preferences, and ratings.
///     This component is optimized for memory efficiency and follows established architectural patterns.
/// </summary>
/// <remarks>
///     This component was extracted from the Candidates.razor DetailTemplate Tab 1 to improve
///     modularity and reusability. It follows the same patterns as RequisitionInfoPanel and CompanyInfoPanel.
/// </remarks>
public partial class CandidateInfoPanel
{
    /// <summary>
    ///     Gets or sets the candidate details model containing all the basic information to display.
    /// </summary>
    [Parameter]
    public CandidateDetails Model { get; set; } = new();

    /// <summary>
    ///     Gets or sets the formatted address markup to display in the address section.
    ///     This is pre-formatted HTML content showing the candidate's full address.
    /// </summary>
    [Parameter]
    public MarkupString Address { get; set; } = "".ToMarkupString();

    /// <summary>
    ///     Gets or sets the candidate eligibility/authorization markup to display.
    ///     This shows the candidate's work authorization status.
    /// </summary>
    [Parameter]
    public MarkupString CandidateEligibility { get; set; } = "".ToMarkupString();

    /// <summary>
    ///     Gets or sets the candidate experience level markup to display.
    ///     This shows the candidate's experience level in formatted text.
    /// </summary>
    [Parameter]
    public MarkupString CandidateExperience { get; set; } = "".ToMarkupString();

    /// <summary>
    ///     Gets or sets the candidate tax terms markup to display.
    ///     This shows the candidate's preferred tax terms (W2, 1099, etc.).
    /// </summary>
    [Parameter]
    public MarkupString CandidateTaxTerms { get; set; } = "".ToMarkupString();

    /// <summary>
    ///     Gets or sets the candidate job options markup to display.
    ///     This shows the candidate's job preferences and options.
    /// </summary>
    [Parameter]
    public MarkupString CandidateJobOptions { get; set; } = "".ToMarkupString();

    /// <summary>
    ///     Gets or sets the candidate communication skills markup to display.
    ///     This shows the candidate's communication skill level.
    /// </summary>
    [Parameter]
    public MarkupString CandidateCommunication { get; set; } = "".ToMarkupString();

    /// <summary>
    ///     Gets or sets the rating date markup to display.
    ///     This shows when the candidate was last rated and by whom.
    /// </summary>
    [Parameter]
    public MarkupString RatingDate { get; set; } = "".ToMarkupString();

    /// <summary>
    ///     Gets or sets the rating notes markup to display.
    ///     This shows any notes associated with the candidate's rating.
    /// </summary>
    [Parameter]
    public MarkupString RatingNote { get; set; } = "".ToMarkupString();

    /// <summary>
    ///     Gets or sets the MPC date markup to display.
    ///     This shows when the candidate's MPC (Most Placeable Candidate) status was last updated.
    /// </summary>
    [Parameter]
    public MarkupString MPCDate { get; set; } = "".ToMarkupString();

    /// <summary>
    ///     Gets or sets the MPC notes markup to display.
    ///     This shows any notes associated with the candidate's MPC status.
    /// </summary>
    [Parameter]
    public MarkupString MPCNote { get; set; } = "".ToMarkupString();
}