#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             ExtendedComponents
// File Name:           Upload.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-06-2025 16:02
// Last Updated On:     06-11-2025 19:42
// *****************************************/

#endregion

#region Using

using ActionCompleteEventArgs = Syncfusion.Blazor.Inputs.ActionCompleteEventArgs;

#endregion

namespace ExtendedComponents;

public partial class Upload : ComponentBase
{
    /// <summary>
    ///     Gets or sets the callback event that is triggered after a file upload operation is completed.
    ///     This event receives an argument of type <see cref="Syncfusion.Blazor.Inputs.ActionCompleteEventArgs" />,
    ///     which contains information about the completed upload action.
    /// </summary>
    [Parameter]
    public EventCallback<ActionCompleteEventArgs> AfterUpload { get; set; }

    /// <summary>
    ///     Gets or sets the string of allowed file extensions for the UploaderControl.
    ///     The extensions should be prefixed with a dot and separated by commas.
    ///     By default, the allowed extensions are ".pdf,.docx,.doc,.rtf,.xps,.txt,.xlsx,.xls".
    /// </summary>
    [Parameter]
    public string AllowedExtensions { get; set; } = ".pdf,.docx,.doc,.rtf,.xps,.txt,.xlsx,.xls";

    /// <summary>
    ///     Gets or sets a value indicating whether the file upload should start automatically after a file is selected.
    ///     If set to true, the upload starts immediately after file selection. If set to false, the user must manually start
    ///     the upload.
    ///     The default value is true.
    /// </summary>
    [Parameter]
    public bool AutoUpload { get; set; } = true;

    /// <summary>
    ///     Gets or sets the callback event that is triggered before a file upload operation starts.
    ///     This event receives an argument of type <see cref="Syncfusion.Blazor.Inputs.BeforeUploadEventArgs" />,
    ///     which contains information about the upload action that is about to start.
    /// </summary>
    [Parameter]
    public EventCallback<BeforeUploadEventArgs> BeforeUpload { get; set; }

    /// <summary>
    ///     Gets or sets the child content of the UploaderControl. This is a RenderFragment that can be used to
    ///     render additional HTML content or Razor components within the UploaderControl.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    ///     Gets or sets the callback event that is triggered when a file is selected in the UploaderControl.
    ///     This event receives an argument of type <see cref="Syncfusion.Blazor.Inputs.SelectedEventArgs" />,
    ///     which contains information about the selected file.
    /// </summary>
    [Parameter]
    public EventCallback<SelectedEventArgs> FileSelected { get; set; }

    /// <summary>
    ///     Gets or sets the ID of the UpdaterControl. This ID is used as the 'for' attribute value for the label and the
    ///     'Target' attribute value for the tooltip in the HTML markup.
    /// </summary>
    [Parameter]
    public string ID { get; set; } = "";

    /// <summary>
    ///     Gets or sets a value indicating whether multiple file selection is allowed in the UploaderControl.
    ///     If set to true, the user can select multiple files at once for upload. If set to false, the user can only select
    ///     one file at a time.
    ///     The default value is false.
    /// </summary>
    [Parameter]
    public bool Multiple { get; set; }

    /// <summary>
    ///     Gets or sets the callback event that is triggered when the value of the file input changes in the UploaderControl.
    ///     This event receives an argument of type <see cref="Syncfusion.Blazor.Inputs.UploadChangeEventArgs" />,
    ///     which contains information about the file input change event.
    /// </summary>
    [Parameter]
    public EventCallback<UploadChangeEventArgs> OnFileUpload { get; set; }

    /// <summary>
    ///     Gets or sets the callback event that is triggered when a file is removed from the UploaderControl.
    ///     This event receives an argument of type <see cref="Syncfusion.Blazor.Inputs.RemovingEventArgs" />,
    ///     which contains information about the file removal event.
    /// </summary>
    [Parameter]
    public EventCallback<RemovingEventArgs> OnRemove { get; set; }

    /// <summary>
    ///     Gets or sets the placeholder text for the UpdaterControl component.
    ///     This property allows for setting a display text in the date input field when it is empty.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the file upload should be sequential.
    ///     If set to true, the files are uploaded one after the other when multiple files are selected.
    ///     If set to false, the files are uploaded simultaneously.
    ///     The default value is false.
    /// </summary>
    [Parameter]
    public bool SequentialUpload { get; set; }
}