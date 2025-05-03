#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           AppWorkflow.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          04-17-2024 20:04
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a workflow in the application.
/// </summary>
/// <remarks>
///     This class is used to manage the workflow in the application. It includes properties for the Workflow's unique
///     identifier, the current step, the next step, whether it's the last step, the associated role IDs, whether the
///     workflow step can be scheduled, whether the workflow step can be executed at any stage, and the full descriptions
///     of the next step, the roles, and the step.
/// </remarks>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class Workflow
{
	/// <summary>
	///     Initializes a new instance of the <see cref="Workflow" /> class.
	/// </summary>
	/// <remarks>
	///     This constructor is used to reset all properties of the AppWorkflow instance to their default values.
	/// </remarks>
	public Workflow()
    {
        Clear();
    }

	/// <summary>
	///     Initializes a new instance of the <see cref="Workflow" /> class.
	/// </summary>
	/// <param name="id">The unique identifier for the AppWorkflow instance.</param>
	/// <param name="step">The step in the workflow.</param>
	/// <param name="next">The next step in the workflow.</param>
	/// <param name="isLast">A value indicating whether this is the last step in the workflow.</param>
	/// <param name="roleIDs">The Role IDs associated with the AppWorkflow instance.</param>
	/// <param name="schedule">A value indicating whether the workflow step can be scheduled.</param>
	/// <param name="anyStage">A value indicating whether the workflow step can be executed at any stage.</param>
	/// <param name="nextFull">The full description of the next step in the workflow.</param>
	/// <param name="roleFull">The full description of the roles associated with the AppWorkflow instance.</param>
	/// <param name="stepFull">The full description of the step in the workflow. This parameter is optional.</param>
	public Workflow(int id, string step, string next, bool isLast, string roleIDs, bool schedule, bool anyStage, string nextFull,
					   string roleFull, string stepFull = "")
    {
        ID = id;
        Step = step;
        Next = next;
        IsLast = isLast;
        RoleIDs = roleIDs;
        Schedule = schedule;
        AnyStage = anyStage;
        NextFull = nextFull;
        RoleFull = roleFull;
        StepFull = stepFull;
    }

	/// <summary>
	///     Gets or sets a value indicating whether the AppWorkflow instance can be executed at any stage.
	/// </summary>
	/// <value>
	///     A boolean value. If true, the AppWorkflow instance can be executed at any stage; otherwise, it can't.
	/// </value>
	/// <remarks>
	///     This property is used to determine if the AppWorkflow instance can be executed at any stage in the workflow.
	///     It is displayed as 'Yes' or 'No' in the 'Any Stage' column of the Workflow grid in the Admin section of the
	///     application.
	/// </remarks>
	public bool AnyStage
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the unique identifier for the AppWorkflow instance.
	/// </summary>
	/// <value>
	///     The unique identifier for the AppWorkflow instance.
	/// </value>
	/// <remarks>
	///     This property is used as the primary key in the database and is also used to link workflows in the system.
	/// </remarks>
	public int ID
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets a value indicating whether this is the last step in the AppWorkflow instance.
	/// </summary>
	/// <value>
	///     A boolean value. If true, this is the last step in the workflow; otherwise, it is not.
	/// </value>
	/// <remarks>
	///     This property is used to determine if the current step is the last one in the workflow.
	///     It is displayed as 'Yes' or 'No' in the 'Is Last Step' column of the Workflow grid in the Admin section of the
	///     application.
	/// </remarks>
	public bool IsLast
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the next step in the AppWorkflow instance.
	/// </summary>
	/// <value>
	///     A string representing the next step. This property is not required and should not exceed 100 characters in length.
	/// </value>
	/// <remarks>
	///     The next step is used to determine the subsequent step in the workflow.
	///     It is displayed in the 'Next Step' column of the Workflow grid in the Admin section of the application.
	/// </remarks>
	public string Next
    {
        get;
        set;
    }
	
	public List<string> NextList
	{
		get => Next.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
		set => Next = string.Join(",", value);
	}

	/// <summary>
	///     Gets or sets the full name of the next step in the AppWorkflow instance.
	/// </summary>
	/// <value>
	///     A string representing the full name of the next step. This property is not required and there is no limit on its
	///     length.
	/// </value>
	/// <remarks>
	///     The full name of the next step provides a more detailed description of the next step in the workflow.
	///     It is displayed in the 'Next Step' column of the Workflow grid in the Admin section of the application.
	/// </remarks>
	public string NextFull
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the full role associated with the AppWorkflow instance.
	/// </summary>
	/// <value>
	///     A string representing the full role. This property is not required and there is no limit on its length.
	/// </value>
	/// <remarks>
	///     The full role is used to provide a more detailed description of the roles associated with a particular workflow
	///     step.
	///     It is displayed in the 'Roles' column of the Workflow grid in the Admin section of the application.
	/// </remarks>
	public string RoleFull
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the Role IDs associated with the AppWorkflow instance.
	/// </summary>
	/// <value>
	///     A string representing the Role IDs. This property is required and should not exceed 50 characters in length.
	/// </value>
	/// <remarks>
	///     The Role IDs are used to determine the roles associated with a particular workflow step.
	///     Each Role ID should be unique. At least one Role ID is required for each AppWorkflow instance.
	/// </remarks>
	public string RoleIDs
    {
        get;
        set;
    }
	
	public List<string> RoleIDList
	{
		get => RoleIDs.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
		set => RoleIDs = string.Join(",", value);
	}

	/// <summary>
	///     Gets or sets a value indicating whether the workflow step is schedulable.
	/// </summary>
	/// <value>
	///     A boolean value. If true, the workflow step can be scheduled; otherwise, it cannot be scheduled.
	/// </value>
	/// <remarks>
	///     This property is used in the `EditActivityDialog` class to determine if the workflow step can be shown in the
	///     dialog.
	///     It is also used in the `Workflow` and `WorkflowDialog` classes for rendering purposes.
	/// </remarks>
	public bool Schedule
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the step in the application workflow.
	/// </summary>
	/// <value>
	///     A string representing the step in the workflow. This value should not be more than 3 characters.
	/// </value>
	/// <remarks>
	///     This property is used in various parts of the application such as the `Candidate`, `CompanyRequisitions`, and other
	///     classes to control the flow of the application.
	/// </remarks>
	public string Step
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the full description of the step in the workflow.
	/// </summary>
	/// <value>
	///     A string representing the full description of the step.
	/// </value>
	/// <remarks>
	///     This property is used to provide a more detailed description of the step in the workflow. It can be used in user
	///     interfaces to give users more context about the step.
	/// </remarks>
	public string StepFull
    {
        get;
        set;
    }

	/// <summary>
	///     Resets all properties of the AppWorkflow instance to their default values.
	/// </summary>
	/// <remarks>
	///     This method is typically used to prepare an AppWorkflow instance for reuse or to clear its data before disposal.
	/// </remarks>
	// ReSharper disable once MemberCanBePrivate.Global
	public void Clear()
    {
        ID = 0;
        Step = "";
        Next = "";
        IsLast = false;
        RoleIDs = "";
        Schedule = false;
        AnyStage = false;
        NextFull = "";
        RoleFull = "";
        StepFull = "";
    }

	/// <summary>
	///     Creates a deep copy of the current AppWorkflow object.
	/// </summary>
	/// <returns>A new AppWorkflow object that is a deep copy of this instance.</returns>
	public Workflow Copy() => MemberwiseClone() as Workflow;
}