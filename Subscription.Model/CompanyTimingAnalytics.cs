namespace Subscription.Model;

/// <summary>
///     Model for timing analytics at the company level (Query 6).
///     Aggregated timing data grouped by company and user.
/// </summary>
public class CompanyTimingAnalytics
{
    /// <summary>
    ///     Company name
    /// </summary>
    public string CompanyName { get; set; }

    /// <summary>
    ///     User who created/owns the requisitions
    /// </summary>
    public string CreatedBy { get; set; }

    /// <summary>
    ///     Average days spent in Decision status
    /// </summary>
    public decimal DECDays { get; set; }

    /// <summary>
    ///     Average days spent in Hired status
    /// </summary>
    public decimal HIRDays { get; set; }

    /// <summary>
    ///     Average days spent in Hold status
    /// </summary>
    public decimal HLDDays { get; set; }

    /// <summary>
    ///     Average days spent in Interview status
    /// </summary>
    public decimal INTDays { get; set; }

    /// <summary>
    ///     Average days spent in Notice of Award status
    /// </summary>
    public decimal NOADays { get; set; }

    /// <summary>
    ///     Average days spent in Offer Declined status
    /// </summary>
    public decimal ODCDays { get; set; }

    /// <summary>
    ///     Average days spent in Offer Extended status
    /// </summary>
    public decimal OEXDays { get; set; }

    /// <summary>
    ///     Average days spent in Pending status
    /// </summary>
    public decimal PENDays { get; set; }

    /// <summary>
    ///     Average days spent in Phone Screen status
    /// </summary>
    public decimal PHNDays { get; set; }

    /// <summary>
    ///     Average days spent in Rejected status
    /// </summary>
    public decimal REJDays { get; set; }

    /// <summary>
    ///     Average days spent in Hiring Manager Review status
    /// </summary>
    public decimal RHMDays { get; set; }

    /// <summary>
    ///     Average time to fill positions in days
    /// </summary>
    public decimal TimeToFillDays { get; set; }

    /// <summary>
    ///     Average time to hire from first submission in days
    /// </summary>
    public decimal TimeToHireDays { get; set; }

    /// <summary>
    ///     Total number of candidates for this company
    /// </summary>
    public int TotalCandidates { get; set; }

    /// <summary>
    ///     Total number of recruiters working on this company
    /// </summary>
    public int TotalRecruiters { get; set; }

    /// <summary>
    ///     Total number of requisitions for this company
    /// </summary>
    public int TotalRequisitions { get; set; }

    /// <summary>
    ///     Average days spent in Under Review status
    /// </summary>
    public decimal URWDays { get; set; }

    /// <summary>
    ///     Average days spent in Withdrawn status
    /// </summary>
    public decimal WDRDays { get; set; }
}