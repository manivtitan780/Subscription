#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           JsonContext.cs
// Created By:          Gemini
// Created On:          07-28-2025
// *****************************************/

#endregion

using System.Text.Json.Serialization;
using Subscription.Model;
using Subscription.Model.Return;

namespace Subscription.Model;

[JsonSerializable(typeof(List<CandidateMPC>))]
[JsonSerializable(typeof(List<CandidateRating>))]
[JsonSerializable(typeof(Role))]
[JsonSerializable(typeof(List<Role>))]
[JsonSerializable(typeof(AdminList))]
[JsonSerializable(typeof(DocumentTypes))]
[JsonSerializable(typeof(JobOptions))]
[JsonSerializable(typeof(NAICS))]
[JsonSerializable(typeof(State))]
[JsonSerializable(typeof(AppTemplate))]
[JsonSerializable(typeof(List<AppTemplate>))]
[JsonSerializable(typeof(List<IntValues>))]
[JsonSerializable(typeof(List<StateCache>))]
[JsonSerializable(typeof(List<Requisition>))]
[JsonSerializable(typeof(List<JobOptions>))]
[JsonSerializable(typeof(List<Workflow>))]
[JsonSerializable(typeof(List<CandidateActivity>))]
[JsonSerializable(typeof(List<RequisitionDocuments>))]
[JsonSerializable(typeof(List<CandidateNotes>))]
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(Workflow))]
[JsonSerializable(typeof(CandidateSearch))]
[JsonSerializable(typeof(CandidateActivity[]))]
[JsonSerializable(typeof(StateCache[]))]
[JsonSerializable(typeof(IntValues[]))]
[JsonSerializable(typeof(KeyValues[]))]
[JsonSerializable(typeof(CandidateMPC[]))]
[JsonSerializable(typeof(CandidateRating[]))]
[JsonSerializable(typeof(CandidateRatingMPC))]
[JsonSerializable(typeof(ReturnGrid))]
[JsonSerializable(typeof(CandidateDetails))]
[JsonSerializable(typeof(CandidateActivity))]
[JsonSerializable(typeof(List<EmailTemplates>))]
[JsonSerializable(typeof(EmailTemplates))]
[JsonSerializable(typeof(List<KeyValues>))]
[JsonSerializable(typeof(KeyValues))]
[JsonSerializable(typeof(CandidateDetailsResume))]
[JsonSerializable(typeof(CandidateEducation))]
[JsonSerializable(typeof(CandidateExperience))]
[JsonSerializable(typeof(CandidateNotes))]
[JsonSerializable(typeof(CandidateSkills))]
[JsonSerializable(typeof(RequisitionSearch))]
[JsonSerializable(typeof(ReturnGridRequisition))]
[JsonSerializable(typeof(ReturnRequisitionDetails))]
[JsonSerializable(typeof(RequisitionDetails))]
[JsonSerializable(typeof(ReturnDashboard))]
[JsonSerializable(typeof(List<Dictionary<string, string>>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(ConsolidatedMetrics))]
[JsonSerializable(typeof(List<ConsolidatedMetrics>))]
[JsonSerializable(typeof(RecentActivityItem))]
[JsonSerializable(typeof(List<RecentActivityItem>))]
[JsonSerializable(typeof(HiredPlacement))]
[JsonSerializable(typeof(List<HiredPlacement>))]
[JsonSerializable(typeof(RequisitionTimingAnalytics))]
[JsonSerializable(typeof(List<RequisitionTimingAnalytics>))]
[JsonSerializable(typeof(CompanyTimingAnalytics))]
[JsonSerializable(typeof(List<CompanyTimingAnalytics>))]
[JsonSerializable(typeof(UserItem))]
[JsonSerializable(typeof(CompanySearch))]
[JsonSerializable(typeof(CompanyDetails))]
[JsonSerializable(typeof(CompanyLocations))]
[JsonSerializable(typeof(List<CompanyLocations>))]
[JsonSerializable(typeof(CompanyContacts))]
[JsonSerializable(typeof(List<CompanyContacts>))]
[JsonSerializable(typeof(CandidateNotes[]))]
[JsonSerializable(typeof(CompanyDocuments))]
[JsonSerializable(typeof(List<CompanyDocuments>))]
[JsonSerializable(typeof(List<Company>))]
[JsonSerializable(typeof(List<RequisitionDetails>))]
[JsonSerializable(typeof(ReturnCompanyDetails))]
[JsonSerializable(typeof(SubmissionTimeline))]
[JsonSerializable(typeof(SubmissionTimeline[]))]
[JsonSerializable(typeof(UserList))]
[JsonSerializable(typeof(List<UserList>))]
[JsonSerializable(typeof(StatusCode))]
[JsonSerializable(typeof(List<StatusCode>))]
[JsonSerializable(typeof(Preferences))]
[JsonSerializable(typeof(CompaniesList))]
[JsonSerializable(typeof(List<CompaniesList>))]
[JsonSerializable(typeof(CandidateDocument))]
[JsonSerializable(typeof(CandidateDocument[]))]
[JsonSerializable(typeof(CandidateEducation))]
[JsonSerializable(typeof(CandidateEducation[]))]
[JsonSerializable(typeof(CandidateExperience))]
[JsonSerializable(typeof(CandidateExperience[]))]
[JsonSerializable(typeof(CandidateSkills[]))]
[JsonSerializable(typeof(DocumentDetails))]
[JsonSerializable(typeof(Candidate))]
[JsonSerializable(typeof(List<Candidate>))]
public partial class JsonContext : JsonSerializerContext
{
}
