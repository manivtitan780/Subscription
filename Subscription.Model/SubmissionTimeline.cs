#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           SubmissionTimeline.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-26-2025 20:07
// Last Updated On:     07-27-2025 19:09
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a lightweight submission timeline entry for display purposes.
/// </summary>
/// <remarks>
///     This struct is optimized for memory efficiency when displaying submission timelines
///     in the TimelineDialog component. It contains only essential fields required for timeline visualization.
/// </remarks>
public struct SubmissionTimeline
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SubmissionTimeline" /> struct.
    /// </summary>
    /// <param name="id">The unique identifier for the submission.</param>
    /// <param name="requisitionId">The requisition identifier.</param>
    /// <param name="candidateId">The candidate identifier.</param>
    /// <param name="status">The status code (e.g., PHN, INT, SUB).</param>
    /// <param name="statusName">The full status name from StatusCode lookup.</param>
    /// <param name="notes">Notes associated with the submission.</param>
    /// <param name="createdBy">The user who created the submission.</param>
    /// <param name="createdDate">The date when the submission was created.</param>
    /// <param name="candidateName">The candidate's name.</param>
    /// <param name="requisitionName">The requisition title/name.</param>
    /// <param name="interviewDateTime">The interview date/time (only for INT status).</param>
    /// <param name="phoneNumber">The interview phone number (only for INT status).</param>
    /// <param name="interviewDetails">The interview details (only for INT status).</param>
    public SubmissionTimeline(int id, int requisitionId, int candidateId, string status, string statusName, string notes, string createdBy, DateTime createdDate,
                              string candidateName, string requisitionName, DateTime? interviewDateTime = null, string phoneNumber = null, string interviewDetails = null)
    {
        Id = id;
        RequisitionId = requisitionId;
        CandidateId = candidateId;
        Status = status;
        StatusName = statusName;
        Notes = notes ?? "";
        CreatedBy = createdBy ?? "";
        CreatedDate = createdDate;
        CandidateName = candidateName ?? "";
        RequisitionName = requisitionName ?? "";
        InterviewDateTime = interviewDateTime;
        PhoneNumber = phoneNumber ?? "";
        InterviewDetails = interviewDetails ?? "";
    }

    /// <summary>
    ///     Gets the unique identifier for the submission.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Gets the requisition identifier.
    /// </summary>
    public int RequisitionId { get; set; }

    /// <summary>
    ///     Gets the candidate identifier.
    /// </summary>
    public int CandidateId { get; set; }

    /// <summary>
    ///     Gets the status code (e.g., PHN, INT, SUB).
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    ///     Gets the full status name from StatusCode lookup.
    /// </summary>
    public string StatusName { get; set; }

    /// <summary>
    ///     Gets the notes associated with the submission.
    /// </summary>
    public string Notes { get; set; }

    /// <summary>
    ///     Gets the user who created the submission.
    /// </summary>
    public string CreatedBy { get; set; }

    /// <summary>
    ///     Gets the date when the submission was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    ///     Gets the candidate's name.
    /// </summary>
    public string CandidateName { get; set; }

    /// <summary>
    ///     Gets the requisition title/name.
    /// </summary>
    public string RequisitionName { get; set; }

    /// <summary>
    ///     Gets the interview date/time (only relevant for INT status).
    /// </summary>
    public DateTime? InterviewDateTime { get; set; }

    /// <summary>
    ///     Gets the interview phone number (only relevant for INT status).
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    ///     Gets the interview details (only relevant for INT status).
    /// </summary>
    public string InterviewDetails { get; set; }

    /// <summary>
    ///     Gets a value indicating whether this submission has interview details.
    /// </summary>
    public bool IsInterview => Status == "INT";

    /// <summary>
    ///     Gets the formatted date string for display.
    /// </summary>
    public string FormattedDate => CreatedDate.CultureDate();

    /// <summary>
    ///     Gets the formatted interview date string for display.
    /// </summary>
    public string FormattedInterviewDate => InterviewDateTime?.CultureDate() ?? "";
}