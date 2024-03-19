#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CompanyLocationsValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          3-19-2024 16:27
// Last Updated On:     3-19-2024 18:52
// *****************************************/

#endregion

namespace Subscription.Model.Validators;

public class CompanyLocationsValidator : AbstractValidator<CompanyLocations>
{
    public CompanyLocationsValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

    }
}