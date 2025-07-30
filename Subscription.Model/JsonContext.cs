#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           JsonContext.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-29-2025 19:07
// Last Updated On:     07-29-2025 20:02
// *****************************************/

#endregion

#region Using

using System.Text.Json.Serialization;

using Subscription.Model.Return;

#endregion

namespace Subscription.Model;

// Comprehensive JsonSerializable attributes for all classes in Subscription.Model
// Generated with Class, List<Class>, and Class[] variations for each type

// Root Model Classes - Class Types
[JsonSerializable(typeof(AdminList)), JsonSerializable(typeof(AppTemplate)), JsonSerializable(typeof(CacheObjects)), JsonSerializable(typeof(Candidate)), JsonSerializable(typeof(CandidateActivity)),
 JsonSerializable(typeof(CandidateDetails)), JsonSerializable(typeof(CandidateDetailsResume)), JsonSerializable(typeof(CandidateDocument)), JsonSerializable(typeof(CandidateEducation)),
 JsonSerializable(typeof(CandidateExperience)), JsonSerializable(typeof(CandidateMPC)), JsonSerializable(typeof(CandidateNotes)), JsonSerializable(typeof(CandidateRating)), JsonSerializable(typeof(CandidateRatingMPC)),
 JsonSerializable(typeof(CandidateResume)), JsonSerializable(typeof(CandidateSearch)), JsonSerializable(typeof(CandidateSkills)), JsonSerializable(typeof(ChartDataPoint)), JsonSerializable(typeof(CityZip)),
 JsonSerializable(typeof(Company)), JsonSerializable(typeof(CompanyContacts)), JsonSerializable(typeof(CompanyContactList)), JsonSerializable(typeof(CompanyDetails)), JsonSerializable(typeof(CompanyDocuments)),
 JsonSerializable(typeof(CompanyLocations)), JsonSerializable(typeof(CompanySearch)), JsonSerializable(typeof(CompanyTimingAnalytics)), JsonSerializable(typeof(CompaniesList)), JsonSerializable(typeof(ConsolidatedMetrics)),
 JsonSerializable(typeof(DashboardData)), JsonSerializable(typeof(DateCount)), JsonSerializable(typeof(DateCounts)), JsonSerializable(typeof(DocumentDetails)), JsonSerializable(typeof(DocumentTypes)),
 JsonSerializable(typeof(EmailTemplates)), JsonSerializable(typeof(HiredPlacement)), JsonSerializable(typeof(IntValues)), JsonSerializable(typeof(JobOptions)), JsonSerializable(typeof(KeyValues)),
 JsonSerializable(typeof(LocationDrop)), JsonSerializable(typeof(LoginCooky)), JsonSerializable(typeof(LoginModel)), JsonSerializable(typeof(NAICS)), JsonSerializable(typeof(ParsedCandidate)), JsonSerializable(typeof(PeriodItem)),
 JsonSerializable(typeof(Preferences)), JsonSerializable(typeof(RatioCount)), JsonSerializable(typeof(RatioCounts)), JsonSerializable(typeof(RecentActivity)), JsonSerializable(typeof(RecentActivityItem)),
 JsonSerializable(typeof(RecruiterConsolidatedMetrics)), JsonSerializable(typeof(Requisition)), JsonSerializable(typeof(RequisitionDetails)), JsonSerializable(typeof(RequisitionDocuments)),
 JsonSerializable(typeof(RequisitionSearch)), JsonSerializable(typeof(RequisitionStatus)), JsonSerializable(typeof(RequisitionTimingAnalytics)), JsonSerializable(typeof(Role)), JsonSerializable(typeof(State)),
 JsonSerializable(typeof(StateCache)), JsonSerializable(typeof(StatusCode)), JsonSerializable(typeof(SubmitCandidateRequisition)), JsonSerializable(typeof(SubmissionTimeline)), JsonSerializable(typeof(SummaryGridItem)),
 JsonSerializable(typeof(TemplateAction)), JsonSerializable(typeof(TimeMetric)), JsonSerializable(typeof(User)), JsonSerializable(typeof(UserItem)), JsonSerializable(typeof(UserList)), JsonSerializable(typeof(Workflow)),
 JsonSerializable(typeof(Zip)), JsonSerializable(typeof(ReturnCandidateDetails)), JsonSerializable(typeof(ReturnCompanyDetails)), JsonSerializable(typeof(ReturnDashboard)), JsonSerializable(typeof(ReturnGrid)),
 JsonSerializable(typeof(ReturnGridRequisition)), JsonSerializable(typeof(ReturnRequisitionDetails)), JsonSerializable(typeof(List<AdminList>)), JsonSerializable(typeof(List<AppTemplate>)),
 JsonSerializable(typeof(List<CacheObjects>)), JsonSerializable(typeof(List<Candidate>)), JsonSerializable(typeof(List<CandidateActivity>)), JsonSerializable(typeof(List<CandidateDetails>)),
 JsonSerializable(typeof(List<CandidateDetailsResume>)), JsonSerializable(typeof(List<CandidateDocument>)), JsonSerializable(typeof(List<CandidateEducation>)), JsonSerializable(typeof(List<CandidateExperience>)),
 JsonSerializable(typeof(List<CandidateMPC>)), JsonSerializable(typeof(List<CandidateNotes>)), JsonSerializable(typeof(List<CandidateRating>)), JsonSerializable(typeof(List<CandidateRatingMPC>)),
 JsonSerializable(typeof(List<CandidateResume>)), JsonSerializable(typeof(List<CandidateSearch>)), JsonSerializable(typeof(List<CandidateSkills>)), JsonSerializable(typeof(List<ChartDataPoint>)),
 JsonSerializable(typeof(List<CityZip>)), JsonSerializable(typeof(List<Company>)), JsonSerializable(typeof(List<CompanyContacts>)), JsonSerializable(typeof(List<CompanyContactList>)),
 JsonSerializable(typeof(List<CompanyDetails>)), JsonSerializable(typeof(List<CompanyDocuments>)), JsonSerializable(typeof(List<CompanyLocations>)), JsonSerializable(typeof(List<CompanySearch>)),
 JsonSerializable(typeof(List<CompanyTimingAnalytics>)), JsonSerializable(typeof(List<CompaniesList>)), JsonSerializable(typeof(List<ConsolidatedMetrics>)), JsonSerializable(typeof(List<DashboardData>)),
 JsonSerializable(typeof(List<DateCount>)), JsonSerializable(typeof(List<DateCounts>)), JsonSerializable(typeof(List<DocumentDetails>)), JsonSerializable(typeof(List<DocumentTypes>)),
 JsonSerializable(typeof(List<EmailTemplates>)), JsonSerializable(typeof(List<HiredPlacement>)), JsonSerializable(typeof(List<IntValues>)), JsonSerializable(typeof(List<JobOptions>)), JsonSerializable(typeof(List<KeyValues>)),
 JsonSerializable(typeof(List<LocationDrop>)), JsonSerializable(typeof(List<LoginCooky>)), JsonSerializable(typeof(List<LoginModel>)), JsonSerializable(typeof(List<NAICS>)), JsonSerializable(typeof(List<ParsedCandidate>)),
 JsonSerializable(typeof(List<PeriodItem>)), JsonSerializable(typeof(List<Preferences>)), JsonSerializable(typeof(List<RatioCount>)), JsonSerializable(typeof(List<RatioCounts>)), JsonSerializable(typeof(List<RecentActivity>)),
 JsonSerializable(typeof(List<RecentActivityItem>)), JsonSerializable(typeof(List<RecruiterConsolidatedMetrics>)), JsonSerializable(typeof(List<Requisition>)), JsonSerializable(typeof(List<RequisitionDetails>)),
 JsonSerializable(typeof(List<RequisitionDocuments>)), JsonSerializable(typeof(List<RequisitionSearch>)), JsonSerializable(typeof(List<RequisitionStatus>)), JsonSerializable(typeof(List<RequisitionTimingAnalytics>)),
 JsonSerializable(typeof(List<Role>)), JsonSerializable(typeof(List<State>)), JsonSerializable(typeof(List<StateCache>)), JsonSerializable(typeof(List<StatusCode>)), JsonSerializable(typeof(List<SubmitCandidateRequisition>)),
 JsonSerializable(typeof(List<SubmissionTimeline>)), JsonSerializable(typeof(List<SummaryGridItem>)), JsonSerializable(typeof(List<TemplateAction>)), JsonSerializable(typeof(List<TimeMetric>)),
 JsonSerializable(typeof(List<User>)), JsonSerializable(typeof(List<UserItem>)), JsonSerializable(typeof(List<UserList>)), JsonSerializable(typeof(List<Workflow>)), JsonSerializable(typeof(List<Zip>)),
 JsonSerializable(typeof(List<ReturnCandidateDetails>)), JsonSerializable(typeof(List<ReturnCompanyDetails>)), JsonSerializable(typeof(List<ReturnDashboard>)), JsonSerializable(typeof(List<ReturnGrid>)),
 JsonSerializable(typeof(List<ReturnGridRequisition>)), JsonSerializable(typeof(List<ReturnRequisitionDetails>)), JsonSerializable(typeof(AdminList[])), JsonSerializable(typeof(AppTemplate[])),
 JsonSerializable(typeof(CacheObjects[])), JsonSerializable(typeof(Candidate[])), JsonSerializable(typeof(CandidateActivity[])), JsonSerializable(typeof(CandidateDetails[])), JsonSerializable(typeof(CandidateDetailsResume[])),
 JsonSerializable(typeof(CandidateDocument[])), JsonSerializable(typeof(CandidateEducation[])), JsonSerializable(typeof(CandidateExperience[])), JsonSerializable(typeof(CandidateMPC[])), JsonSerializable(typeof(CandidateNotes[])),
 JsonSerializable(typeof(CandidateRating[])), JsonSerializable(typeof(CandidateRatingMPC[])), JsonSerializable(typeof(CandidateResume[])), JsonSerializable(typeof(CandidateSearch[])), JsonSerializable(typeof(CandidateSkills[])),
 JsonSerializable(typeof(ChartDataPoint[])), JsonSerializable(typeof(CityZip[])), JsonSerializable(typeof(Company[])), JsonSerializable(typeof(CompanyContacts[])), JsonSerializable(typeof(CompanyContactList[])),
 JsonSerializable(typeof(CompanyDetails[])), JsonSerializable(typeof(CompanyDocuments[])), JsonSerializable(typeof(CompanyLocations[])), JsonSerializable(typeof(CompanySearch[])),
 JsonSerializable(typeof(CompanyTimingAnalytics[])), JsonSerializable(typeof(CompaniesList[])), JsonSerializable(typeof(ConsolidatedMetrics[])), JsonSerializable(typeof(DashboardData[])), JsonSerializable(typeof(DateCount[])),
 JsonSerializable(typeof(DateCounts[])), JsonSerializable(typeof(DocumentDetails[])), JsonSerializable(typeof(DocumentTypes[])), JsonSerializable(typeof(EmailTemplates[])), JsonSerializable(typeof(HiredPlacement[])),
 JsonSerializable(typeof(IntValues[])), JsonSerializable(typeof(JobOptions[])), JsonSerializable(typeof(KeyValues[])), JsonSerializable(typeof(LocationDrop[])), JsonSerializable(typeof(LoginCooky[])),
 JsonSerializable(typeof(LoginModel[])), JsonSerializable(typeof(NAICS[])), JsonSerializable(typeof(ParsedCandidate[])), JsonSerializable(typeof(PeriodItem[])), JsonSerializable(typeof(Preferences[])),
 JsonSerializable(typeof(RatioCount[])), JsonSerializable(typeof(RatioCounts[])), JsonSerializable(typeof(RecentActivity[])), JsonSerializable(typeof(RecentActivityItem[])),
 JsonSerializable(typeof(RecruiterConsolidatedMetrics[])), JsonSerializable(typeof(Requisition[])), JsonSerializable(typeof(RequisitionDetails[])), JsonSerializable(typeof(RequisitionDocuments[])),
 JsonSerializable(typeof(RequisitionSearch[])), JsonSerializable(typeof(RequisitionStatus[])), JsonSerializable(typeof(RequisitionTimingAnalytics[])), JsonSerializable(typeof(Role[])), JsonSerializable(typeof(State[])),
 JsonSerializable(typeof(StateCache[])), JsonSerializable(typeof(StatusCode[])), JsonSerializable(typeof(SubmitCandidateRequisition[])), JsonSerializable(typeof(SubmissionTimeline[])), JsonSerializable(typeof(SummaryGridItem[])),
 JsonSerializable(typeof(TemplateAction[])), JsonSerializable(typeof(TimeMetric[])), JsonSerializable(typeof(User[])), JsonSerializable(typeof(UserItem[])), JsonSerializable(typeof(UserList[])),
 JsonSerializable(typeof(Workflow[])), JsonSerializable(typeof(Zip[])), JsonSerializable(typeof(ReturnCandidateDetails[])), JsonSerializable(typeof(ReturnCompanyDetails[])), JsonSerializable(typeof(ReturnDashboard[])),
 JsonSerializable(typeof(ReturnGrid[])), JsonSerializable(typeof(ReturnGridRequisition[])), JsonSerializable(typeof(ReturnRequisitionDetails[])), JsonSerializable(typeof(Dictionary<string, string>)),
 JsonSerializable(typeof(List<Dictionary<string, string>>))]
public partial class JsonContext : JsonSerializerContext
{
    /// <summary>
    ///     Provides a case-insensitive JSON serializer context for the application.
    ///     This context is configured to ignore case when matching JSON property names to .NET object properties.
    ///     It is initialized with a JsonSerializerOptions object that has PropertyNameCaseInsensitive set to true.
    /// </summary>
    public static JsonContext CaseInsensitive { get; } = new(new()
                                                             {
                                                                 PropertyNameCaseInsensitive = true
                                                             });
}