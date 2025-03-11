#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Profsvc_AppTrack
// Project:             Profsvc_AppTrack.Client
// File Name:           Header.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          1-5-2024 16:13
// Last Updated On:     1-27-2024 16:19
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Admin;

/// <summary>
///     Represents the header component of the Admin page in the ProfSvc_AppTrack application.
/// </summary>
/// <remarks>
///     The Header class is a partial class that contains various parameters and methods that are used to manage the header
///     component of the Admin page.
///     It includes parameters for the current page, the user's role ID, and various view permissions.
///     It also includes methods for initializing the component and handling user interactions, such as menu selections and
///     clicks on the variable commission dialog.
/// </remarks>
public partial class Header
{
    private string _baseURL = "";

    /// <summary>
    ///     Gets or sets an instance of the ILocalStorageService.
    ///     This service is used for managing local storage in the application.
    ///     It is used to remove the storage cookie on user logout.
    /// </summary>
    [Inject]
    private ILocalStorageService LocalStorage
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the instance of the NavigationManager service used in the Companies page.
    ///     This service provides methods and properties to manage and interact with the URI of the application.
    ///     It is used for tasks such as navigating to different pages and constructing URIs for use within the application.
    ///     For example, it is used to navigate to home page after logout of the user.
    /// </summary>
    [Inject]
    private NavigationManager NavManager
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the current page of the Header component.
    /// </summary>
    /// <value>
    ///     The Page is a string that represents the current page of the Header component.
    /// </value>
    [Parameter]
    public string Page
    {
        get;
        set;
    }

    /*/// <summary>
    ///     Gets or sets the PreferencesDialog instance associated with the Header component.
    /// </summary>
    /// <value>
    ///     The PreferencesDialog instance used to manage and display the preferences dialog in the Header
    ///     component.
    /// </value>
    /// <remarks>
    ///     This property is used to handle interactions with the preferences dialog in the Header component, such as
    ///     showing the dialog when a user clicks on the associated UI element.
    /// </remarks>
    private PreferencesDialog PreferenceDialog
    {
        get;
        set;
    }*/

    /// <summary>
    ///     Gets or sets the RoleID associated with the user.
    /// </summary>
    /// <value>
    ///     The RoleID is a string that represents the role identifier of the user.
    /// </value>
    [Parameter]
    public string RoleID
    {
        get;
        set;
    }

    /*/// <summary>
    ///     Gets or sets the VariableCommissionDialog instance associated with the Header component.
    /// </summary>
    /// <value>
    ///     The VariableCommissionDialog instance used to manage and display the variable commission dialog in the Header
    ///     component.
    /// </value>
    /// <remarks>
    ///     This property is used to handle interactions with the variable commission dialog in the Header component, such as
    ///     showing the dialog when a user clicks on the associated UI element.
    /// </remarks>
    private VariableCommissionDialog VariableDialog
    {
        get;
        set;
    }*/

    /// <summary>
    ///     Gets or sets a value indicating whether the Candidate menu item is visible.
    /// </summary>
    /// <value>
    ///     true if the Candidate menu item is visible; otherwise, false. The default is true.
    /// </value>
    /// <remarks>
    ///     This property is used to control the visibility of the Candidate menu item in the header component of the Admin
    ///     page.
    ///     If the value is true, the Candidate menu item is visible; if the value is false, the Candidate menu item is hidden.
    /// </remarks>
    [Parameter]
    public bool ViewCandidate
    {
        get;
        set;
    } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether the Company menu item is visible.
    /// </summary>
    /// <value>
    ///     true if the Company menu item is visible; otherwise, false. The default is true.
    /// </value>
    /// <remarks>
    ///     This property is used to control the visibility of the Company menu item in the header component of the Admin page.
    ///     If the value is true, the Company menu item is visible; if the value is false, the Company menu item is hidden.
    /// </remarks>
    [Parameter]
    public bool ViewCompany
    {
        get;
        set;
    } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether the Leads menu item is visible.
    /// </summary>
    /// <value>
    ///     true if the Leads menu item is visible; otherwise, false. The default is true.
    /// </value>
    /// <remarks>
    ///     This property is used to control the visibility of the Leads menu item in the header component of the Admin page.
    ///     If the value is true, the Leads menu item is visible; if the value is false, the Leads menu item is hidden.
    /// </remarks>
    [Parameter]
    public bool ViewLeads
    {
        get;
        set;
    } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether the Requisition menu item is visible.
    /// </summary>
    /// <value>
    ///     true if the Requisition menu item is visible; otherwise, false. The default is true.
    /// </value>
    /// <remarks>
    ///     This property is used to control the visibility of the Requisition menu item in the header component of the Admin
    ///     page.
    ///     If the value is true, the Requisition menu item is visible; if the value is false, the Requisition menu item is
    ///     hidden.
    /// </remarks>
    [Parameter]
    public bool ViewRequisition
    {
        get;
        set;
    } = true;

    /// <summary>
    ///     Handles the selection of a menu item.
    /// </summary>
    /// <param name="args">The arguments of the menu item that was selected.</param>
    /// <remarks>
    ///     This method is invoked when a menu item is selected. It performs different actions based on the text of the
    ///     selected item:
    ///     - "Variable Commission": Opens the Variable Commission dialog.
    ///     - "Preferences": Opens the Preferences dialog.
    ///     - "Sign Out": Removes the "DeliciousCookie" from the local storage and navigates to the base URL.
    /// </remarks>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    private async Task MenuSelected(MenuEventArgs<MenuItem> args)
    {
        await Task.Yield();
        switch (args.Item.Text)
        {
            case "Variable Commission":
                await Task.CompletedTask; //VariableDialog.ShowDialog();
                break;
            case "Preferences":
                await Task.CompletedTask; //PreferenceDialog.ShowDialog();
                break;
            case "Sign Out":
                await LocalStorage.RemoveItemAsync("DeliciousCookie");
                NavManager.NavigateTo($"{_baseURL}", true);
                break;
        }
    }

    /// <summary>
    ///     Asynchronously initializes the component after receiving parameters from its parent in the render tree.
    /// </summary>
    /// <remarks>
    ///     This method sets the _baseURL field to the BaseUri of the NavigationManager.
    ///     It's an override of the base method in the ComponentBase class, which is called when the component is initialized.
    /// </remarks>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    protected override void OnInitialized()
    {
        _baseURL = NavManager.BaseUri;
        base.OnInitialized();
    }
}