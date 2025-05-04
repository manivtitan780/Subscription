#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Templates.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          05-03-2025 16:05
// Last Updated On:     05-03-2025 19:59
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Admin;

public partial class Templates : ComponentBase
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private bool AdminScreens { get; set; }

    private List<AppTemplate> DataSource { get; set; } = [];

    [Inject]
    private SfDialogService DialogService { get; set; }

    private SfGrid<AppTemplate> Grid { get; set; }

    [Inject]
    private ILocalStorageService LocalStorage { get; set; }

    [Inject]
    private NavigationManager NavManager { get; set; }

    private string RoleID { get; set; }

    private string RoleName { get; set; }

    [Inject]
    private ISessionStorageService SessionStorage { get; set; }

    private string TemplateAuto { get; set; }

    private AppTemplate TemplateRecord { get; set; } = new();

    private AppTemplate TemplateRecordClone { get; set; } = new();

    private string Title { get; set; } = "Edit";

    private string User { get; set; }

    private bool VisibleSpinner { get; set; }

    private TemplateDialog TemplateDialog { get; set; }

    private async Task DataBound(object arg)
    {
        if (Grid.TotalItemCount > 0)
        {
            await Grid.SelectRowAsync(0);
        }
    }

    private Task EditTemplateAsync(int id = 0) => ExecuteMethod(async () =>
                                                                {
                                                                    VisibleSpinner = true;
                                                                    if (id != 0)
                                                                    {
                                                                        List<AppTemplate> _selectedList = await Grid.GetSelectedRecordsAsync();
                                                                        if (_selectedList.Count == 0 || _selectedList.First().ID != id)
                                                                        {
                                                                            int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
                                                                            await Grid.SelectRowAsync(_index);
                                                                        }
                                                                    }

                                                                    if (id == 0)
                                                                    {
                                                                        Title = "Add";
                                                                        if (TemplateRecordClone == null)
                                                                        {
                                                                            TemplateRecordClone = new();
                                                                        }
                                                                        else
                                                                        {
                                                                            TemplateRecordClone.Clear();
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        Title = "Edit";
                                                                        TemplateRecordClone = TemplateRecord.Copy();
                                                                    }

                                                                    VisibleSpinner = false;
                                                                    //TemplateRecordClone.Entity = "Template";
                                                                    await TemplateDialog.ShowDialog();
                                                                });

    private Task ExecuteMethod(Func<Task> task) => General.ExecuteMethod(_semaphore, task);

    private Task FilterGrid(ChangeEventArgs<string, KeyValues> template) => ExecuteMethod(async () =>
                                                                                          {
                                                                                              await FilterSet(template.Value.NullOrWhiteSpace() ? "" : template.Value);
                                                                                              await SetDataSource();
                                                                                          });

    private async Task FilterSet(string value)
    {
        TemplateAuto = value;
        await LocalStorage.SetItemAsStringAsync("autoTemplate", value);
    }

    private static string GetActionText(byte actionValue)
    {
        if (!Enum.IsDefined(typeof(TemplateAction), (int)actionValue))
        {
            return "Unknown";
        }

        string actionName = ((TemplateAction)actionValue).ToString();
        return SplitPascalCase(actionName);
    }

    [GeneratedRegex("(?<!^)([A-Z])")]
    private static partial Regex MyRegex();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            string _result = await LocalStorage.GetItemAsStringAsync("autoTemplate");

            TemplateAuto = _result.NotNullOrWhiteSpace() && _result != "null" ? _result : "";

            try
            {
                await SetDataSource();
            }
            catch
            {
                //
            }
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await ExecuteMethod(async () =>
                            {
                                IEnumerable<Claim> _claims = await General.GetClaimsToken(LocalStorage, SessionStorage);

                                if (_claims == null)
                                {
                                    NavManager.NavigateTo($"{NavManager.BaseUri}login", true);
                                }
                                else
                                {
                                    IEnumerable<Claim> _enumerable = _claims as Claim[] ?? _claims.ToArray();
                                    User = _enumerable.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value.ToUpperInvariant();
                                    RoleName = _enumerable.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value.ToUpperInvariant();
                                    if (User.NullOrWhiteSpace())
                                    {
                                        NavManager.NavigateTo($"{NavManager.BaseUri}login", true);
                                    }

                                    // Set user permissions
                                    AdminScreens = _enumerable.Any(claim => claim.Type == "Permission" && claim.Value == "AdminScreens");
                                }
                            });

        await base.OnInitializedAsync();
    }

    private async Task RefreshGrid() => await SetDataSource();

    private void RowSelected(RowSelectingEventArgs<AppTemplate> template) => TemplateRecord = template.Data;

    private Task SaveTemplate() => ExecuteMethod(async () =>
                                                 {
                                                     DataSource = await General.SaveEntityAndRefreshAsync<AppTemplate, AppTemplate>("Admin/SaveTemplate", "Admin_SaveTemplate", "Template",
                                                                                                                                    nameof(CacheObjects.Templates), TemplateRecordClone,
                                                                                                                                    record => TemplateRecord = record.Copy(), RefreshGrid); //()
                                                 });

    private async Task SetDataSource()
    {
        DataSource = await General.LoadDataAsync<AppTemplate>("Admin_GetTemplates", TemplateAuto ?? "");
        foreach (AppTemplate item in DataSource)
        {
            item.ActionText = GetActionText(item.Action);
        }

        await Grid.Refresh();
    }

    private static string SplitPascalCase(string input) => MyRegex().Replace(input, " $1").Trim();

    private Task ToggleMethod(int id, bool enabled) => ExecuteMethod(async () =>
                                                                     {
                                                                         List<AppTemplate> _selectedList = await Grid.GetSelectedRecordsAsync();
                                                                         if (_selectedList.Count == 0 || _selectedList.First().ID != id)
                                                                         {
                                                                             int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
                                                                             await Grid.SelectRowAsync(_index);
                                                                         }

                                                                         if (await DialogService.ConfirmAsync(null, enabled ? "Disable Template?" : "Enable Template?",
                                                                                                              General.DialogOptions($"Are you sure you want to <strong>{(enabled ? "disable" : "enable")
                                                                                                              }</strong> this <i>Template</i>?")))
                                                                         {
                                                                             Dictionary<string, string> _parameters = new()
                                                                                                                      {
                                                                                                                          {"methodName", "Admin_ToggleTemplateStatus"},
                                                                                                                          {"id", id.ToString()}
                                                                                                                      };
                                                                             string _response = await General.ExecuteRest<string>("Admin/ToggleAdminList", _parameters);

                                                                             if (_response.NotNullOrWhiteSpace() && _response != "[]")
                                                                             {
                                                                                 await FilterSet("");
                                                                                 DataSource = General.DeserializeObject<List<AppTemplate>>(_response);
                                                                             }
                                                                         }
                                                                     });
}