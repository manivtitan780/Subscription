#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           Role.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-06-2025 16:02
// Last Updated On:     02-26-2025 15:02
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a role in the Professional Services application.
/// </summary>
/// <remarks>
///     A role defines a set of permissions that determine what actions a user can perform.
///     Each user is assigned one or more roles, and the user's permissions are the union of the permissions of all their
///     roles.
/// </remarks>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global"), SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class Role
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Role" /> class and resets its properties to their default values.
    /// </summary>
    public Role()
    {
        Clear();
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the role has permission to change requisition status.
    /// </summary>
    /// <value>
    ///     true if the role can change requisition status; otherwise, false.
    /// </value>
    public bool CreateOrEditCandidate
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the role has permission to change candidate status.
    /// </summary>
    /// <value>
    ///     true if the role can change candidate status; otherwise, false.
    /// </value>
    public bool CreateOrEditCompany
    {
        get;
        set;
    }

    public bool AdminScreens
    {
        get;
        set;
    }
    /// <summary>
    ///     Gets or sets a value indicating whether the role has permission to edit requisitions.
    /// </summary>
    /// <value>
    ///     true if the role can edit requisitions; otherwise, false.
    /// </value>
    public bool CreateOrEditRequisitions
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the description of the role.
    /// </summary>
    /// <value>
    ///     The description of the role.
    /// </value>
    public string Description
    {
        get;
        set;
    }

    public bool DownloadFormatted
    {
        get;
        set;
    }

    public bool DownloadOriginal
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the role has permission to edit clients.
    /// </summary>
    /// <value>
    ///     true if the role can edit clients; otherwise, false.
    /// </value>
    public bool EditMyCompanyProfile
    {
        get;
        set;
    }

    public bool EditRequisitions
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the identifier for the Role.
    /// </summary>
    /// <value>
    ///     The string value representing the unique identifier of the Role.
    /// </value>
    /// <remarks>
    ///     This property is used to uniquely identify a Role. It is used in various operations such as fetching user rights
    ///     based on roles.
    /// </remarks>
    public int ID
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the role is being added.
    /// </summary>
    /// <value>
    ///     true if the role is being added; otherwise, false.
    /// </value>
    public bool ManageSubmittedCandidates
    {
        get;
        set;
    }

    public string RoleName
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the role has permission to forward resumes.
    /// </summary>
    /// <value>
    ///     true if the role can forward resumes; otherwise, false.
    /// </value>
    public bool ViewAllCandidates
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the role has permission to download resumes.
    /// </summary>
    /// <value>
    ///     true if the role can download resumes; otherwise, false.
    /// </value>
    public bool ViewAllCompanies
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the role has permission to edit candidate information.
    /// </summary>
    /// <value>
    ///     true if the role can edit candidate information; otherwise, false.
    /// </value>
    public bool ViewMyCompanyProfile
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the role has permission to forward resumes.
    /// </summary>
    /// <value>
    ///     true if the role can forward resumes; otherwise, false.
    /// </value>
    public bool ViewOnlyMyCandidates
    {
        get;
        set;
    }

    public bool ViewRequisitions
    {
        get;
        set;
    }

    public string CreatedDate
    {
        get;
        set;
    }
    
    public string UpdatedDate
    {
        get;
        set;
    }
    /// <summary>
    ///     Resets all properties of the Role instance to their default values.
    /// </summary>
    /// <remarks>
    ///     This method is used to clear all the properties of the Role instance. It sets all string properties to an empty
    ///     string and all boolean properties to false.
    ///     It is typically used when a new Role needs to be created or an existing Role needs to be reset.
    /// </remarks>
    public void Clear()
    {
        ID = 0;
        Description = "";
        RoleName = "";
        CreateOrEditCandidate = false;
        CreateOrEditCompany = false;
        CreateOrEditRequisitions = false;
        DownloadFormatted = false;
        DownloadOriginal = false;
        EditMyCompanyProfile = false;
        EditRequisitions = false;
        ManageSubmittedCandidates = false;
        ViewAllCandidates = false;
        ViewAllCompanies = false;
        ViewMyCompanyProfile = false;
        ViewOnlyMyCandidates = false;
        ViewRequisitions = false;
        CreatedDate = DateTime.Today.CultureDate();
        UpdatedDate = DateTime.Today.CultureDate();
    }

    /// <summary>
    ///     Creates a copy of the current Role instance.
    /// </summary>
    /// <returns>
    ///     A new Role object that is a copy of the current instance.
    /// </returns>
    /// <remarks>
    ///     This method uses the MemberwiseClone method to create a shallow copy of the current object.
    ///     Shallow copy means the method creates a new object and then copies the nonstatic fields of the current object to
    ///     the new object.
    ///     If a field is a value type, a bit-by-bit copy of the field is performed.
    ///     If a field is a reference type, the reference is copied but the referred object is not; therefore, the original
    ///     object and its clone refer to the same object.
    /// </remarks>
    public Role Copy() => MemberwiseClone() as Role;
}