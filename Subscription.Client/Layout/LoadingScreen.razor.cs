#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Client
// File Name:           LoadingScreen.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          2-7-2024 15:51
// Last Updated On:     2-7-2024 15:58
// *****************************************/

#endregion

namespace Subscription.Client.Layout;

/// <summary>
///     Represents a loading screen component in the ProfSvc_AppTrack application.
///     This component displays a spinner while the page content is loading.
/// </summary>
/// <remarks>
///     This class is used in the App.razor file as a wrapper around the main Router component.
///     It uses a RenderFragment to display child content once the content has been loaded.
/// </remarks>
public partial class LoadingScreen
{
    private bool _contentLoaded;

    /// <summary>
    ///     Gets or sets the child content of the LoadingScreen component.
    /// </summary>
    /// <value>
    ///     The child content to be displayed once the content has been loaded.
    /// </value>
    /// <remarks>
    ///     This property is of type RenderFragment, which represents a segment of UI content.
    ///     It is used in the LoadingScreen.razor file to conditionally display the child content when the content has been
    ///     loaded.
    /// </remarks>
    [Parameter]
    public RenderFragment ChildContent
    {
        get;
        set;
    }

    /// <summary>
    ///     Asynchronously initializes the LoadingScreen component.
    /// </summary>
    /// <remarks>
    ///     This method is called after the component has been initialized, but before it has received its initial parameters.
    ///     It sets the _contentLoaded field to true after a delay, indicating that the content has been loaded.
    /// </remarks>
    /// <returns>
    ///     A Task that represents the asynchronous operation.
    /// </returns>
    protected override async Task OnInitializedAsync()
    {
        await Task.Delay(100);
        _contentLoaded = true;
        await base.OnInitializedAsync();
    }
}