#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CandidateDetails.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-06-2025 16:02
// Last Updated On:     05-02-2025 19:05
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents the details of a candidate.
/// </summary>
/// <remarks>
///     This class is used to manage and manipulate candidate data, including personal information, contact details, and
///     preferences.
/// </remarks>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global"), SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global"), SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class CandidateDetails
{
	/// <summary>
	///     Initializes a new instance of the <see cref="CandidateDetails" /> class.
	/// </summary>
	/// <remarks>
	///     This constructor is used to create a new candidate with default data.
	/// </remarks>
	public CandidateDetails()
    {
        Clear();
    }

	/// <summary>
	///     Gets or sets the first line of the candidate's address.
	/// </summary>
	/// <value>
	///     The first line of the candidate's address.
	/// </value>
	/// <remarks>
	///     This property is required and its length should be between 1 and 255 characters.
	/// </remarks>
	public string Address1 { get; set; }

	/// <summary>
	///     Gets or sets the secondary address line for the candidate.
	/// </summary>
	/// <value>
	///     A string representing the secondary address line of the candidate.
	/// </value>
	/// <remarks>
	///     This property is used when the candidate's address requires an additional line for more detailed information.
	///     The maximum length of this property is 255 characters.
	/// </remarks>
	public string Address2 { get; set; }

	/// <summary>
	///     Gets or sets a value indicating whether the background check has been completed for the candidate.
	/// </summary>
	/// <value>
	///     true if the background check is completed; otherwise, false.
	/// </value>
	/// <remarks>
	///     Use this property to track the status of the candidate's background check process.
	/// </remarks>
	public bool Background { get; set; }

	/// <summary>
	///     Gets or sets the unique identifier for the candidate.
	/// </summary>
	/// <value>
	///     The unique identifier for the candidate.
	/// </value>
	/// <remarks>
	///     This property is used to uniquely identify a candidate in the system.
	/// </remarks>
	public int CandidateID { get; set; }

	/// <summary>
	///     Gets or sets the city of the candidate.
	/// </summary>
	/// <value>
	///     The city of the candidate.
	/// </value>
	/// <remarks>
	///     This property is required and should be between 1 and 50 characters.
	/// </remarks>
	public string City { get; set; }

	/// <summary>
	///     Gets or sets the communication rating of the candidate.
	/// </summary>
	/// <value>
	///     The communication rating, represented as a string value of the CommunicationEnum.
	/// </value>
	/// <remarks>
	///     This property is used to store the candidate's communication rating. The rating is represented as a string value of
	///     the CommunicationEnum:
	///     - "G" => "Good"
	///     - "A" => "Average"
	///     - "X" => "Excellent"
	///     - Any other value => "Fair"
	/// </remarks>
	public string Communication { get; set; }

	/// <summary>
	///     Gets or sets the creation date of the candidate details.
	/// </summary>
	/// <value>
	///     The creation date of the candidate details.
	/// </value>
	/// <remarks>
	///     This property is used to track when the candidate details were first created.
	/// </remarks>
	public string Created { get; set; }

	/// <summary>
	///     Gets or sets a value indicating whether the candidate has Equal Employment Opportunity (EEO) status.
	/// </summary>
	/// <value>
	///     <c>true</c> if the candidate has EEO status; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>
	///     This property is used to track the EEO status of the candidate. EEO status is a legal framework that prohibits
	///     employment discrimination.
	/// </remarks>
	public bool EEO { get; set; }

	/// <summary>
	///     Gets or sets the Equal Employment Opportunity (EEO) file associated with the candidate.
	/// </summary>
	/// <value>
	///     The EEO file as a string.
	/// </value>
	/// <remarks>
	///     This property is used to store and retrieve the EEO file of the candidate.
	/// </remarks>
	public string EEOFile { get; set; }

	/// <summary>
	///     Gets or sets the eligibility ID of the candidate.
	/// </summary>
	/// <value>
	///     The eligibility ID is an integer value that represents the eligibility status of the candidate.
	///     This ID is used to link the candidate with their corresponding eligibility status in the system.
	/// </value>
	/// <remarks>
	///     The eligibility ID is used in various parts of the application, such as the Candidate page and the
	///     EditCandidateDialog component,
	///     to determine and display the eligibility status of the candidate.
	/// </remarks>
	public int EligibilityID { get; set; }

	/// <summary>
	///     Gets or sets the email address of the candidate.
	/// </summary>
	/// <value>
	///     The email address of the candidate.
	/// </value>
	/// <remarks>
	///     This property is required and should be in proper email format.
	///     The length of the email address should be between 5 and 255 characters.
	///     The email address is also validated to check if the candidate already exists.
	/// </remarks>
	public string Email { get; set; }

	/// <summary>
	///     Gets or sets the experience ID of the candidate.
	/// </summary>
	/// <value>
	///     The experience ID is an integer that represents the candidate's experience level.
	///     It is used to match the candidate's experience with the requirements of the job position.
	/// </value>
	public int ExperienceID { get; set; }

	/// <summary>
	///     Gets or sets the Facebook profile ID of the candidate.
	/// </summary>
	/// <value>
	///     The Facebook profile ID of the candidate. This value should be less than 255 characters.
	/// </value>
	/// <remarks>
	///     This property is used when saving candidate details in the `CandidatesController.SaveCandidate()` method.
	/// </remarks>
	public string Facebook { get; set; }

	/// <summary>
	///     Gets or sets the first name of the candidate.
	/// </summary>
	/// <value>
	///     The first name of the candidate. This value is required and should be between 1 and 50 characters.
	/// </value>
	/// <remarks>
	///     This property is validated by the CheckCandidateExists method in the Validations class.
	/// </remarks>
	public string FirstName { get; set; } = "";

	/// <summary>
	///     Gets or sets the formatted resume of the candidate.
	/// </summary>
	/// <value>
	///     The formatted resume of the candidate as a string.
	/// </value>
	/// <remarks>
	///     This property is used when saving candidate details to the database via the 'SaveCandidate' stored procedure.
	/// </remarks>
	public string FormattedResume { get; set; }

	/// <summary>
	///     Gets or sets the Google+ profile ID of the candidate.
	/// </summary>
	/// <value>
	///     The Google+ profile ID of the candidate. This value can be up to 255 characters long.
	/// </value>
	/// <remarks>
	///     This property is used to store the Google+ profile ID of the candidate. It can be used to access the candidate's
	///     Google+ profile.
	/// </remarks>
	public string GooglePlus { get; set; }

	/// <summary>
	///     Gets or sets the hourly rate for the candidate.
	/// </summary>
	/// <value>
	///     The hourly rate is a decimal value ranging from $0.0 to $2000.0.
	/// </value>
	/// <remarks>
	///     This property is used to represent the candidate's expected hourly compensation.
	///     An error message is displayed if the value is not within the specified range.
	/// </remarks>
	public decimal HourlyRate { get; set; }

	/// <summary>
	///     Gets or sets the upper limit of the hourly rate for the candidate.
	/// </summary>
	/// <value>
	///     A decimal representing the high end of the candidate's hourly rate.
	/// </value>
	/// <remarks>
	///     This property is validated to be between $0 and $2,000.
	/// </remarks>
	public decimal HourlyRateHigh { get; set; }

	/// <summary>
	///     Gets or sets a value indicating whether a new candidate is being added.
	/// </summary>
	/// <value>
	///     <c>true</c> if a new candidate is being added; otherwise, <c>false</c> if an existing candidate is being edited.
	/// </value>
	/// <remarks>
	///     This property is used in the Candidate page in the ProfSvc_AppTrack application to distinguish between adding a new
	///     candidate and editing an existing one.
	/// </remarks>
	public bool IsAdd { get; set; }

	/// <summary>
	///     Gets or sets the job options for the candidate.
	/// </summary>
	/// <value>
	///     A string representing the job options preferred by the candidate.
	/// </value>
	/// <remarks>
	///     This property can be used to store and retrieve the job options that a candidate is interested in.
	/// </remarks>
	public string JobOptions { get; set; }

	public List<string> JobOptionsList { get => JobOptions.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList(); set => JobOptions = string.Join(",", value); }

	/// <summary>
	///     Gets or sets the keywords associated with the candidate.
	/// </summary>
	/// <value>
	///     A string containing the keywords. This value is required and its length should be between 3 and 500 characters.
	/// </value>
	/// <remarks>
	///     The keywords are used to categorize the candidate and improve searching.
	/// </remarks>
	public string Keywords { get; set; }

	/// <summary>
	///     Gets or sets the last name of the candidate.
	/// </summary>
	/// <remarks>
	///     This property is required and must be between 1 and 50 characters.
	///     It also uses custom validation to check if the candidate exists.
	/// </remarks>
	public string LastName { get; set; }

	/// <summary>
	///     Gets or sets the LinkedIn profile ID of the candidate.
	/// </summary>
	/// <value>
	///     The LinkedIn profile ID of the candidate.
	/// </value>
	/// <remarks>
	///     The LinkedIn profile ID should be less than 255 characters.
	/// </remarks>
	public string LinkedIn { get; set; }

	/// <summary>
	///     Gets or sets the middle name of the candidate.
	/// </summary>
	/// <value>
	///     The middle name of the candidate.
	/// </value>
	/// <remarks>
	///     This property is validated to ensure that the middle name is no more than 50 characters in length.
	/// </remarks>
	public string MiddleName { get; set; }

	/// <summary>
	///     Gets or sets a value indicating whether the MPC condition is met.
	/// </summary>
	/// <value>
	///     <c>true</c> if MPC is met; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>
	///     Use this property to check or define the MPC status of the candidate.
	/// </remarks>
	public bool MPC { get; set; }

	/// <summary>
	///     Gets or sets the notes associated with the candidate in the MPC context.
	/// </summary>
	/// <value>
	///     The notes associated with the candidate in the MPC context.
	/// </value>
	/// <remarks>
	///     This property is used in the Candidate page of the ProfSvc_AppTrack application for retrieving the most recent note
	///     from the CandidateMPC object list and for retrieving the most recent date from the CandidateMPC list.
	/// </remarks>
	public string MPCNotes { get; set; }

	/// <summary>
	///     Gets or sets the original resume of the candidate.
	/// </summary>
	/// <value>
	///     The original resume of the candidate as a string.
	/// </value>
	/// <remarks>
	///     This property is used to store the original format of the candidate's resume.
	///     It is used in the `SaveCandidate` method of the `CandidatesController` class.
	/// </remarks>
	public string OriginalResume { get; set; }

	/// <summary>
	///     Gets or sets the primary phone number of the candidate.
	/// </summary>
	/// <value>
	///     The primary phone number of the candidate.
	/// </value>
	/// <remarks>
	///     This property is required and should be in proper phone number format. The phone number should be 10 digits long,
	///     including the area code.
	/// </remarks>
	public string Phone1 { get; set; }

	/// <summary>
	///     Gets or sets the secondary phone number of the candidate.
	/// </summary>
	/// <value>
	///     The secondary phone number of the candidate.
	/// </value>
	/// <remarks>
	///     This property is used when the candidate has more than one phone number.
	/// </remarks>
	public string Phone2 { get; set; }

	/// <summary>
	///     Gets or sets the third phone number of the candidate.
	/// </summary>
	/// <value>
	///     The third phone number of the candidate.
	/// </value>
	/// <remarks>
	///     This property is used when the candidate has more than two phone numbers.
	/// </remarks>
	public string Phone3 { get; set; }

	/// <summary>
	///     Gets or sets the phone extension for the candidate.
	/// </summary>
	/// <value>
	///     The phone extension as a string.
	/// </value>
	/// <remarks>
	///     This property is used when more than one phone line is associated with the same phone number.
	/// </remarks>
	public string PhoneExt { get; set; }

	/// <summary>
	///     Gets or sets the rating of the candidate.
	/// </summary>
	/// <value>
	///     The rating of the candidate.
	/// </value>
	/// <remarks>
	///     This property is used to store the rating of the candidate. The value is an integer where a higher value indicates
	///     a better rating.
	/// </remarks>
	public int RateCandidate { get; set; }

	/// <summary>
	///     Gets or sets the rate notes for the candidate.
	/// </summary>
	/// <value>
	///     The rate notes for the candidate.
	/// </value>
	/// <remarks>
	///     This property is used to store any notes related to the candidate's rating.
	///     It is used in the Candidate page of the ProfSvc_AppTrack application to retrieve and display the rating note for a
	///     candidate.
	/// </remarks>
	public string RateNotes { get; set; }

	/// <summary>
	///     Gets or sets a value indicating whether the candidate is referred or not.
	/// </summary>
	/// <value>
	///     <c>true</c> if the candidate is referred; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>
	///     This property is used to track the referral status of a candidate. If the candidate is referred by someone, this
	///     property will be set to <c>true</c>; otherwise, it will be set to <c>false</c>.
	/// </remarks>
	public bool Refer { get; set; }

	/// <summary>
	///     Gets or sets the account manager reference for the candidate.
	/// </summary>
	/// <value>
	///     The account manager reference.
	/// </value>
	/// <remarks>
	///     This property is used when the candidate is referred by an account manager.
	///     The value corresponds to the account manager's identifier.
	/// </remarks>
	public string ReferAccountManager { get; set; }

	/// <summary>
	///     Gets or sets a value indicating whether the candidate is willing to relocate.
	/// </summary>
	/// <value>
	///     <c>true</c> if the candidate is willing to relocate; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>
	///     This property is used to understand the candidate's flexibility in terms of job location.
	/// </remarks>
	public bool Relocate { get; set; }

	/// <summary>
	///     Gets or sets the relocation notes for the candidate.
	/// </summary>
	/// <value>
	///     The relocation notes, which should be less than 2000 characters.
	/// </value>
	/// <remarks>
	///     This property is used to store any additional notes or details about the candidate's relocation preferences or
	///     requirements.
	/// </remarks>
	public string RelocationNotes { get; set; }

	/// <summary>
	///     Gets or sets the maximum expected salary for the candidate.
	/// </summary>
	/// <value>
	///     The maximum salary that the candidate expects, expressed as a decimal.
	/// </value>
	/// <remarks>
	///     This property is validated to be within the range of $0 to $1,000,000.
	/// </remarks>
	public decimal SalaryHigh { get; set; }

	/// <summary>
	///     Gets or sets the lower limit of the salary range for the candidate.
	/// </summary>
	/// <value>
	///     A decimal representing the lower limit of the salary range in dollars.
	/// </value>
	/// <remarks>
	///     The value should be between $0 and $1,000,000. An error message will be displayed if the value is outside this
	///     range.
	/// </remarks>
	public decimal SalaryLow { get; set; }

	/// <summary>
	///     Gets or sets the security notes for the candidate.
	/// </summary>
	/// <value>
	///     The security notes for the candidate.
	/// </value>
	/// <remarks>
	///     This property is used to store any security-related notes or comments about the candidate.
	///     The length of the notes should be less than 2000 characters.
	/// </remarks>
	public string SecurityNotes { get; set; }

	/// <summary>
	///     Gets or sets the identifier for the state of the candidate.
	/// </summary>
	/// <value>
	///     The identifier for the state.
	/// </value>
	/// <remarks>
	///     This property is used to link the candidate to a specific state.
	///     The value corresponds to the key of the state in the state's dictionary.
	/// </remarks>
	public int StateID { get; set; }

	/// <summary>
	///     Gets or sets the status of the candidate.
	/// </summary>
	/// <value>
	///     A string representing the current status of the candidate.
	/// </value>
	/// <remarks>
	///     The status of the candidate can be used to track the candidate's progress in the recruitment process.
	/// </remarks>
	public string Status { get; set; }

	/// <summary>
	///     Gets or sets the summary of the candidate's profile.
	/// </summary>
	/// <value>
	///     A string representing the summary of the candidate's profile.
	/// </value>
	/// <remarks>
	///     This property is bound to a text box control in the user interface, allowing the user to view and edit the summary
	///     of the candidate's profile.
	///     The summary should be less than 32767 characters.
	/// </remarks>
	public string Summary { get; set; }

	/// <summary>
	///     Gets or sets the tax term for the candidate.
	/// </summary>
	/// <value>
	///     A string representing the tax term. The tax term is a comma-separated list of tax-related terms applicable to the
	///     candidate.
	/// </value>
	/// <remarks>
	///     This property is used in the `Candidate.SetTaxTerm()` method in the `Candidate` class and the
	///     `EditCandidateDialog.BuildRenderTree()` method in the `EditCandidateDialog` class.
	/// </remarks>
	public string TaxTerm { get; set; } = "";

    public List<string> TaxTerms { get => TaxTerm.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList(); set => TaxTerm = string.Join(",", value); }

	/// <summary>
	///     Gets or sets the text resume of the candidate.
	/// </summary>
	/// <value>
	///     The text resume of the candidate.
	/// </value>
	/// <remarks>
	///     This property is validated to ensure that the text resume is less than 262,136 characters.
	/// </remarks>
	public string TextResume { get; set; }

	/// <summary>
	///     Gets or sets the title of the candidate.
	/// </summary>
	/// <value>
	///     The title of the candidate.
	/// </value>
	/// <remarks>
	///     This property is required and its length should be between 1 and 200 characters.
	/// </remarks>
	public string Title { get; set; }

	/// <summary>
	///     Gets or sets the Twitter profile ID of the candidate.
	/// </summary>
	/// <value>
	///     The Twitter profile ID of the candidate.
	/// </value>
	/// <remarks>
	///     This property is validated to ensure that the Twitter profile ID is less than 255 characters.
	/// </remarks>
	public string Twitter { get; set; }

	/// <summary>
	///     Gets or sets the timestamp of the last update made to the candidate details.
	/// </summary>
	/// <value>
	///     A string representing the last update timestamp.
	/// </value>
	/// <remarks>
	///     The value is typically set automatically when changes are made to the candidate details.
	/// </remarks>
	public string Updated { get; set; }

	/// <summary>
	///     Gets or sets the Zip Code of the candidate.
	/// </summary>
	/// <value>
	///     The Zip Code of the candidate.
	/// </value>
	/// <remarks>
	///     This property is required and must be a minimum of 5 digits and a maximum of 10 digits.
	/// </remarks>
	public string ZipCode { get; set; }

	/// <summary>
	///     Clears all the properties of the CandidateDetails instance.
	/// </summary>
	/// <remarks>
	///     This method is used to reset all the properties of the CandidateDetails instance to their default values.
	///     It is typically used when we need to clear the existing data of a candidate.
	/// </remarks>
	public void Clear()
    {
        FirstName = "";
        MiddleName = "";
        LastName = "";
        Address1 = "";
        Address2 = "";
        City = "";
        StateID = 0;
        ZipCode = "";
        Email = "";
        Phone1 = "";
        Phone2 = "";
        Phone3 = "";
        PhoneExt = "";
        LinkedIn = "";
        Facebook = "";
        Twitter = "";
        Title = "";
        EligibilityID = 0;
        Relocate = false;
        Background = false;
        JobOptions = "";
        TaxTerm = "";
        OriginalResume = "";
        FormattedResume = "";
        TextResume = "";
        Keywords = "";
        Communication = "A";
        RateCandidate = 3;
        RateNotes = "";
        MPC = false;
        MPCNotes = "";
        ExperienceID = 0;
        HourlyRate = decimal.Zero;
        HourlyRateHigh = decimal.Zero;
        SalaryHigh = decimal.Zero;
        SalaryLow = decimal.Zero;
        RelocationNotes = "";
        SecurityNotes = "";
        Refer = false;
        ReferAccountManager = "";
        EEO = false;
        EEOFile = "";
        Summary = "";
        GooglePlus = "";
        Created = "";
        Updated = "";
        CandidateID = 0;
        Status = "AVL";
    }

	/// <summary>
	///     Creates a copy of the current instance of the <see cref="CandidateDetails" /> class.
	/// </summary>
	/// <returns>
	///     A new instance of the <see cref="CandidateDetails" /> class with the same values as the current instance.
	/// </returns>
	/// <remarks>
	///     This method uses the MemberwiseClone method to create a shallow copy of the current object.
	/// </remarks>
	public CandidateDetails Copy() => MemberwiseClone() as CandidateDetails;

    /*/// <summary>
    ///     Enumerates the possible communication ratings for a candidate.
    /// </summary>
    /// <remarks>
    ///     This enum is used to represent the candidate's communication rating in the CandidateDetails class.
    ///     The values are:
    ///     - A (1) => Average
    ///     - X (2) => Excellent
    ///     - G (3) => Good
    ///     - F (4) => Fair
    /// </remarks>*/
    /*private enum CommunicationEnum
    {
        A = 1, X = 2, G = 3, F = 4
    }*/
}