namespace Subscription.Model;

public class RecentActivityItem
{
    public string ActivityNotes { get; set; }

    public string CandidateName { get; set; }

    public string Company { get; set; }

    public string CurrentStatus { get; set; }

    public DateTime DateFirstSubmitted { get; set; }

    public DateTime LastActivityDate { get; set; }

    public string User { get; set; }
}