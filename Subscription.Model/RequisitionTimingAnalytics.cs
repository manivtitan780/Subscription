namespace Subscription.Model;

/// <summary>
///     Model for timing analytics at the requisition level (Query 5).
///     Tracks time spent in various candidate submission stages for each requisition.
/// </summary>
public class RequisitionTimingAnalytics
{
    /// <summary>
    ///     Company name
    /// </summary>
    public string CompanyName { get; set; }

    /// <summary>
    ///     User who created/owns the requisition
    /// </summary>
    public string CreatedBy { get; set; }

    /// <summary>
    ///     Days spent in Decision status
    /// </summary>
    public decimal DECDays { get; set; }

    /// <summary>
    ///     Days spent in Hired status
    /// </summary>
    public decimal HIRDays { get; set; }

    /// <summary>
    ///     Days spent in Hold status
    /// </summary>
    public decimal HLDDays { get; set; }

    /// <summary>
    ///     Days spent in Interview status
    /// </summary>
    public decimal INTDays { get; set; }

    /// <summary>
    ///     Days spent in Notice of Award status
    /// </summary>
    public decimal NOADays { get; set; }

    /// <summary>
    ///     Days spent in Offer Declined status
    /// </summary>
    public decimal ODCDays { get; set; }

    /// <summary>
    ///     Days spent in Offer Extended status
    /// </summary>
    public decimal OEXDays { get; set; }

    /// <summary>
    ///     Days spent in Pending status
    /// </summary>
    public decimal PENDays { get; set; }

    /// <summary>
    ///     Days spent in Presented to Hiring Manager status
    /// </summary>
    public decimal PHNDays { get; set; }

    /// <summary>
    ///     Days spent in Rejected status
    /// </summary>
    public decimal REJDays { get; set; }

    /// <summary>
    ///     Requisition code (e.g., "DS20241220-01")
    /// </summary>
    public string RequisitionCode { get; set; }

    /// <summary>
    ///     Days spent in Rejected by Hiring Manager status
    /// </summary>
    public decimal RHMDays { get; set; }

    /// <summary>
    ///     Time to fill the position in days
    /// </summary>
    public decimal TimeToFillDays { get; set; }

    /// <summary>
    ///     Time to hire from first submission in days
    /// </summary>
    public decimal TimeToHireDays { get; set; }

    /// <summary>
    ///     Position title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    ///     Total number of candidates submitted for this requisition
    /// </summary>
    public int TotalCandidates { get; set; }

    /// <summary>
    ///     Days spent in Under Review status
    /// </summary>
    public decimal URWDays { get; set; }

    /// <summary>
    ///     Days spent in Withdrawn status
    /// </summary>
    public decimal WDRDays { get; set; }
}