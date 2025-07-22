#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CompanyDetails.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-06-2025 16:02
// Last Updated On:     04-24-2025 20:43
// *****************************************/

#endregion

namespace Subscription.Model;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global"), SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class CompanyDetails
{
    public CompanyDetails()
    {
        Clear();
    }

    public string City { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string DUNS { get; set; }

    public string EIN { get; set; }

    public string EmailAddress { get; set; }

    public string Extension { get; set; }

    public string Fax { get; set; }

    public int ID { get; set; }

    public bool IsAdd { get; set; }

    public string LocationNotes { get; set; }

    public string NAICS { get; set; }

    //[System.Text.Json.Serialization.JsonConverter(typeof(StringToIntConverter))]
    public int NAICSCode { get; set; }

    public string Name { get; set; }

    public string Notes { get; set; }

    public string Phone { get; set; }

    public string State { get; set; }

    public int StateID { get; set; }

    public bool Status { get; set; }

    public string StreetName { get; set; }

    public string UpdatedBy { get; set; }

    public DateTime UpdatedDate { get; set; }

    public string Website { get; set; }

    public string ZipCode { get; set; }

    public void Clear()
    {
        City = "";
        CreatedBy = "ADMIN";
        CreatedDate = DateTime.Today;
        DUNS = "";
        EmailAddress = "";
        ID = 0;
        NAICS = "";
        NAICSCode = 0;
        Name = "";
        Notes = "";
        Phone = "";
        State = "";
        StateID = 1;
        Status = false;
        StreetName = "";
        UpdatedBy = "";
        UpdatedDate = DateTime.Today;
        Website = "";
        ZipCode = "";
        IsAdd = false;
    }

    public CompanyDetails Copy() => MemberwiseClone() as CompanyDetails;
}