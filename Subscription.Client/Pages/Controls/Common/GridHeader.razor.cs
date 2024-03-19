#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Client
// File Name:           GridHeader.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          2-9-2024 20:2
// Last Updated On:     2-10-2024 20:19
// *****************************************/

#endregion

#region Using

using Syncfusion.Blazor.Navigations;

#endregion

namespace Subscription.Client.Pages.Controls.Common;

public partial class GridHeader
{
    [Parameter]
    public int CurrentPage
    {
        get;
        set;
    } = 1;

    [Parameter]
    public RenderFragment GridContent
    {
        get;
        set;
    }

    [Parameter]
    public EventCallback<PagerItemClickEventArgs> ItemClick
    {
        get;
        set;
    }

    [Parameter]
    public int NumericCount
    {
        get;
        set;
    } = 7;

    private SfPager Pager
    {
        get;
        set;
    }

    [Parameter]
    public int PageSize
    {
        get;
        set;
    } = 25;

    [Parameter]
    public EventCallback<PageSizeChangedArgs> PageSizeChanged
    {
        get;
        set;
    }

    [Parameter]
    public List<int> PageSizes
    {
        get;
        set;
    } = [5, 10, 15, 20, 25, 50, 75, 100];

    [Parameter]
    public int TotalCount
    {
        get;
        set;
    } = 100;
}