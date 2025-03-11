#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           AdminListDefault.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily
// Created On:          09-17-2022 20:01
// Last Updated On:     11-28-2022 16:10
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
/// </summary>
public class AdminListDefault
{
    /// <summary>
    ///     Initializes the AdminListDefault class.
    /// </summary>
    public AdminListDefault()
    {
        ClearData();
    }

    /// <summary>
    ///     Initializes the AdminListDefault class.
    /// </summary>
    /// <param name="type">String to be displayed in case of error.</param>
    /// <param name="methodName">Name of the Stored Procedure to execute.</param>
    /// <param name="isAdd">Is the Record Add or Edit?</param>
    /// <param name="isString">Is the Primary key String or Integer.</param>
    public AdminListDefault(string type, string methodName, bool isAdd, bool isString)
    {
        Type = type;
        MethodName = methodName;
        IsAdd = isAdd;
        IsString = isString;
    }

    /// <summary>
    ///     Is the Record Add or Edit?
    /// </summary>
    [Display(Name = "Is Add")]
    public static bool IsAdd
    {
        get;
        set;
    }

    /// <summary>
    ///     Is the Primary key String or Integer.
    /// </summary>
    [Display(Name = "Is String")]
    public static bool IsString
    {
        get;
        set;
    }

    /// <summary>
    ///     Name of the Stored Procedure to execute.
    /// </summary>
    [Display(Name = "Method Name")]
    public static string MethodName
    {
        get;
        set;
    }

    /// <summary>
    ///     String to be displayed in case of error.
    /// </summary>
    public static string Type
    {
        get;
        set;
    }

    /// <summary>
    ///     Clears this object data.
    /// </summary>
    public void ClearData()
    {
        Type = "";
        MethodName = "";
        IsAdd = false;
        IsString = false;
    }

    /// <summary>
    ///     Creates a shallow copy of the current object.
    /// </summary>
    /// <returns></returns>
    public AdminListDefault Copy() => MemberwiseClone() as AdminListDefault;
}