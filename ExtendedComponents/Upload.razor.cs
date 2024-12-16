#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             ExtendedComponents
// File Name:           Upload.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          12-16-2024 20:12
// Last Updated On:     12-16-2024 20:12
// *****************************************/

#endregion

namespace ExtendedComponents;

public partial class Upload : ComponentBase
{
    [Parameter]
    public string ID
    {
        get;
        set;
    }

    public string Placeholder
    {
        get;
        set;
    }
}