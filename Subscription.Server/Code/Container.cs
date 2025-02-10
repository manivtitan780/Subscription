#region Header
// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Container.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-08-2025 15:02
// Last Updated On:     02-08-2025 15:02
// *****************************************/
#endregion

using System.Security.Cryptography.X509Certificates;

namespace Subscription.Server.Code;

public class Container
{
    public Guid InstanceID
    {
        get;
        set;
    } = Guid.NewGuid();
    
    /*
    public List<Company> Companies
    {
        get; 
        set;
    } = [];

    public List<CompanyContacts> CompanyContacts
    {
        get;
        set;
    } = [];

    public List<KeyValues> StatusList
    {
        get;
        set;
    } = [];
*/
}