CREATE OR ALTER PROCEDURE [dbo].[DashboardAccountsManager_Refactor]
    @User VARCHAR(10) = 'DAVE'
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    SET ANSI_NULLS ON;
    SET QUOTED_IDENTIFIER ON;
    
    -- Set default user if not provided
    IF @User IS NULL
        SET @User = SYSTEM_USER;
    
    DECLARE @Today DATE = GETDATE();
    
    -- Generic Query for Dropdown
    SELECT 
        CreatedBy as KeyValue, FullName as Text
    FROM 
        dbo.ActiveUsersView 
    WHERE 
        CreatedBy = @User
    FOR JSON AUTO;
    
    /***************************************************************************
    * RESULT SET 1: TIME-BOUND METRICS (Refactored)
    * Uses pre-calculated SubmissionMetricsView instead of CTEs/temp tables
    ***************************************************************************/
    
    -- Single query replaces entire #RequisitionMetrics and #SubmissionMetrics logic
    SELECT ISNULL((
        SELECT 
            [CreatedBy] as [User], [Period], [Total_Submissions], [Active_Requisitions_Updated] as Active_Requisitions, [INT_Count] as INT_Submissions,
            [OEX_Count] as OEX_Submissions, [HIR_Count] as HIR_Submissions, [OEX_HIR_Ratio], [Requisitions_Created]
        FROM 
            [dbo].[SubmissionMetricsView]
        WHERE 
            [CreatedBy] = @User
        ORDER BY 
            CASE [Period] 
                WHEN '7D' THEN 1 
                WHEN 'MTD' THEN 2 
                WHEN 'QTD' THEN 3 
                WHEN 'HYTD' THEN 4 
                WHEN 'YTD' THEN 5 
            END
        FOR JSON PATH
    ), '[]');

        /***************************************************************************
    * RESULT SET 2: RECENT ACTIVITY REPORT
    * Query 7: 30-Day Submission Activity
    ***************************************************************************/

    DECLARE @return varchar(max);
    SELECT @return =
        (SELECT 
            RAV.CompanyName + ' - [' + RAV.RequisitionCode + '] - ' + CAST(RAV.Positions as varchar(5)) + ' Positions - ' + RAV.RequisitionTitle as Company,
            RAV.CandidateName, RAV.CurrentStatus, RAV.DateFirstSubmitted, RAV.LastActivityDate, RAV.ActivityNotes, @User [User]
        FROM 
            [dbo].[RecentActivityView] RAV
        WHERE 
            RAV.CreatedBy = @User
        ORDER BY 
            RAV.SubmissionCount DESC, RAV.CompanyName ASC, RAV.RequisitionCode ASC, RAV.DateFirstSubmitted ASC
        FOR JSON PATH);

    SELECT ISNULL(@return, '[]');

    /***************************************************************************
    * RESULT SET 3: PLACEMENT REPORT
    * Query 8: 3-Month Hire Report
    ***************************************************************************/

    SELECT @return =
        (SELECT 
            PRV.CompanyName as Company, PRV.RequisitionNumber, PRV.NumPosition, PRV.RequisitionTitle as Title, PRV.CandidateName, PRV.DateHired,
            PRV.SalaryOffered, PRV.StartDate, PRV.DateInvoiced, PRV.DatePaid, PRV.PlacementFee as [Placementfee], PRV.CommissionPercent,
            PRV.CommissionEarned, @User [User]
        FROM [dbo].[PlacementReportView] PRV
        WHERE PRV.CreatedBy = @User
        ORDER BY 
            PRV.CompanyName ASC, PRV.RequisitionNumber ASC, PRV.DateHired DESC
        FOR JSON PATH);

    SELECT ISNULL(@return, '[]');

    /***************************************************************************
    * RESULT SET 2: ACCOUNTS MANAGER TIMING ANALYTICS (Keep Original Logic)
    * Query 1: Time to Fill, Time to Hire, and Time in Stage metrics (By Requisition) 
    * Query 2: Time to Fill, Time to Hire, and Time in Stage metrics (By Company)
    * Shows data for SPECIFIC USER requisitions only (AccountsManager perspective)
    ***************************************************************************/
    
    SELECT @return = 
        (SELECT 
            'DAVE' as [User], TAV.RequisitionCode, TAV.CompanyName, TAV.Title, ISNULL(CEILING(AVG(CAST(TAV.TimeToFill_Days as FLOAT))), 0) as TimeToFill_Days,
            ISNULL(CEILING(AVG(CAST(TAV.TimeToHire_Days as FLOAT))), 0) as TimeToHire_Days, ISNULL(CEILING(AVG(CAST(TAV.PEN_Days as FLOAT))), 0) as PEN_Days,
            ISNULL(CEILING(AVG(CAST(TAV.REJ_Days as FLOAT))), 0) as REJ_Days, ISNULL(CEILING(AVG(CAST(TAV.HLD_Days as FLOAT))), 0) as HLD_Days,
            ISNULL(CEILING(AVG(CAST(TAV.PHN_Days as FLOAT))), 0) as PHN_Days, ISNULL(CEILING(AVG(CAST(TAV.URW_Days as FLOAT))), 0) as URW_Days,
            ISNULL(CEILING(AVG(CAST(TAV.INT_Days as FLOAT))), 0) as INT_Days, ISNULL(CEILING(AVG(CAST(TAV.RHM_Days as FLOAT))), 0) as RHM_Days,
            ISNULL(CEILING(AVG(CAST(TAV.DEC_Days as FLOAT))), 0) as DEC_Days, ISNULL(CEILING(AVG(CAST(TAV.NOA_Days as FLOAT))), 0) as NOA_Days,
            ISNULL(CEILING(AVG(CAST(TAV.OEX_Days as FLOAT))), 0) as OEX_Days, ISNULL(CEILING(AVG(CAST(TAV.ODC_Days as FLOAT))), 0) as ODC_Days,
            ISNULL(CEILING(AVG(CAST(TAV.HIR_Days as FLOAT))), 0) as HIR_Days, ISNULL(CEILING(AVG(CAST(TAV.WDR_Days as FLOAT))), 0) as WDR_Days,
            COUNT(DISTINCT CONCAT(TAV.RequisitionId, '-', TAV.CandidateId)) as TotalCandidates
        FROM 
            [dbo].[TimingAnalyticsView] TAV
        WHERE 
            TAV.CreatedBy = 'DAVE' AND TAV.Context = 'AM'  -- AM context only
        GROUP BY 
            TAV.RequisitionCode, TAV.CompanyName, TAV.Title
        ORDER BY 
            TAV.RequisitionCode
        FOR JSON PATH);
    
        SELECT ISNULL(@return, '[]');

    SELECT @return = 
        (SELECT 
            @User as [User], TAV.CompanyName, ISNULL(CEILING(AVG(CAST(TAV.TimeToFill_Days as FLOAT))), 0) as TimeToFill_Days,
            ISNULL(CEILING(AVG(CAST(TAV.TimeToHire_Days as FLOAT))), 0) as TimeToHire_Days, ISNULL(CEILING(AVG(CAST(TAV.PEN_Days as FLOAT))), 0) as PEN_Days,
            ISNULL(CEILING(AVG(CAST(TAV.REJ_Days as FLOAT))), 0) as REJ_Days, ISNULL(CEILING(AVG(CAST(TAV.HLD_Days as FLOAT))), 0) as HLD_Days,
            ISNULL(CEILING(AVG(CAST(TAV.PHN_Days as FLOAT))), 0) as PHN_Days, ISNULL(CEILING(AVG(CAST(TAV.URW_Days as FLOAT))), 0) as URW_Days,
            ISNULL(CEILING(AVG(CAST(TAV.INT_Days as FLOAT))), 0) as INT_Days, ISNULL(CEILING(AVG(CAST(TAV.RHM_Days as FLOAT))), 0) as RHM_Days,
            ISNULL(CEILING(AVG(CAST(TAV.DEC_Days as FLOAT))), 0) as DEC_Days, ISNULL(CEILING(AVG(CAST(TAV.NOA_Days as FLOAT))), 0) as NOA_Days,
            ISNULL(CEILING(AVG(CAST(TAV.OEX_Days as FLOAT))), 0) as OEX_Days, ISNULL(CEILING(AVG(CAST(TAV.ODC_Days as FLOAT))), 0) as ODC_Days,
            ISNULL(CEILING(AVG(CAST(TAV.HIR_Days as FLOAT))), 0) as HIR_Days, ISNULL(CEILING(AVG(CAST(TAV.WDR_Days as FLOAT))), 0) as WDR_Days,
            COUNT(DISTINCT CONCAT(TAV.RequisitionId, '-', TAV.CandidateId)) as TotalCandidates, COUNT(DISTINCT TAV.RequisitionId) as TotalRequisitions
        FROM 
            [dbo].[TimingAnalyticsView] TAV
        WHERE 
            TAV.CreatedBy = @User AND TAV.Context = 'AM'  -- AM context only
        GROUP BY 
            TAV.CompanyName
        ORDER BY 
            TAV.CompanyName
        --FOR JSON PATH);

    SELECT ISNULL(@return, '[]');

END;

CREATE OR ALTER PROCEDURE [dbo].[DashboardAdmin_Refactor]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    SET ANSI_NULLS ON;
    SET QUOTED_IDENTIFIER ON;
    
    DECLARE @Today DATE = GETDATE();
    
    -- Generic Query for Dropdown (Admin sees all users)
    SELECT 
        CreatedBy as KeyValue, FullName as Text
    FROM 
        dbo.ActiveUsersView
    WHERE 
        Status = 1
    FOR JSON AUTO;
    
    /***************************************************************************
    * RESULT SET 1: TIME-BOUND METRICS - ADMIN AGGREGATED (Refactored)
    * Uses pre-calculated SubmissionMetricsView - SUM across ALL USERS
    ***************************************************************************/
    
    -- Single query replaces entire #RequisitionMetrics and #SubmissionMetrics logic
    -- Admin perspective: Show individual data for ALL active users (no aggregation)
    SELECT ISNULL((
        SELECT 
            [CreatedBy] as [User], [Period], [Total_Submissions], [Active_Requisitions_Updated] as Active_Requisitions, [INT_Count] as INT_Submissions,
            [OEX_Count] as OEX_Submissions, [HIR_Count] as HIR_Submissions, [OEX_HIR_Ratio], [Requisitions_Created]
        FROM 
            [dbo].[SubmissionMetricsView]
        WHERE 
            [CreatedBy] IN (SELECT UserName FROM dbo.Users WHERE Status = 1 AND Role IN (4, 5, 6))
        ORDER BY 
            [CreatedBy], CASE [Period] 
                            WHEN '7D' THEN 1 
                            WHEN 'MTD' THEN 2 
                            WHEN 'QTD' THEN 3 
                            WHEN 'HYTD' THEN 4 
                            WHEN 'YTD' THEN 5 
                        END
        FOR JSON PATH
    ), '[]');

    /***************************************************************************
    * RESULT SET 2: ADMIN STATUS REPORT (Keep Original Logic)
    * Query 1: Current submission status across all users
    ***************************************************************************/
    
    DECLARE @return varchar(max) = '[]';

    SELECT @return =
        (SELECT 
            RAV.CompanyName + ' - [' + RAV.RequisitionCode + '] - ' + CAST(RAV.Positions as varchar(5)) + ' Positions - ' + RAV.RequisitionTitle as Company,
            RAV.CandidateName, RAV.CurrentStatus, RAV.DateFirstSubmitted, RAV.LastActivityDate, RAV.ActivityNotes, RAV.CreatedBy [User]
        FROM 
            [dbo].[RecentActivityView] RAV
        ORDER BY 
            RAV.SubmissionCount DESC, RAV.CompanyName ASC, RAV.RequisitionCode ASC, RAV.DateFirstSubmitted ASC
        FOR JSON PATH);

    SELECT ISNULL(@return, '[]');

    /***************************************************************************
    * RESULT SET 3: HIRED CANDIDATES REPORT (Keep Original Logic)
    * Query 1: Recent hires across all users
    ***************************************************************************/
    
    SELECT @return =
        (SELECT 
            PRV.CompanyName as Company, PRV.RequisitionNumber, PRV.NumPosition, PRV.RequisitionTitle as Title, PRV.CandidateName, PRV.DateHired,
            PRV.SalaryOffered, PRV.StartDate, PRV.DateInvoiced, PRV.DatePaid, PRV.PlacementFee as [Placementfee], PRV.CommissionPercent,
            PRV.CommissionEarned, PRV.CreatedBy [User]
        FROM 
            [dbo].[PlacementReportView] PRV
        ORDER BY 
            PRV.CreatedBy ASC, PRV.CompanyName ASC, PRV.RequisitionNumber ASC, PRV.DateHired DESC
        FOR JSON PATH);

    SELECT ISNULL(@return, '[]');

    /***************************************************************************
    * RESULT SET 4: ADMIN TIMING ANALYTICS (Keep Original Logic)
    * Query 1: Time to Fill, Time to Hire, and Time in Stage metrics (By Requisition) 
    * Query 2: Time to Fill, Time to Hire, and Time in Stage metrics (By Company)
    * Shows data for ALL USERS (Admin perspective)
    ***************************************************************************/
    
    SELECT @return = 
        (SELECT 
            TAV.CreatedBy, TAV.RequisitionCode, TAV.CompanyName, TAV.Title, ISNULL(CEILING(AVG(CAST(TAV.TimeToFill_Days as FLOAT))), 0) as TimeToFill_Days,
            ISNULL(CEILING(AVG(CAST(TAV.TimeToHire_Days as FLOAT))), 0) as TimeToHire_Days, ISNULL(CEILING(AVG(CAST(TAV.PEN_Days as FLOAT))), 0) as PEN_Days,
            ISNULL(CEILING(AVG(CAST(TAV.REJ_Days as FLOAT))), 0) as REJ_Days, ISNULL(CEILING(AVG(CAST(TAV.HLD_Days as FLOAT))), 0) as HLD_Days,
            ISNULL(CEILING(AVG(CAST(TAV.PHN_Days as FLOAT))), 0) as PHN_Days, ISNULL(CEILING(AVG(CAST(TAV.URW_Days as FLOAT))), 0) as URW_Days,
            ISNULL(CEILING(AVG(CAST(TAV.INT_Days as FLOAT))), 0) as INT_Days, ISNULL(CEILING(AVG(CAST(TAV.RHM_Days as FLOAT))), 0) as RHM_Days,
            ISNULL(CEILING(AVG(CAST(TAV.DEC_Days as FLOAT))), 0) as DEC_Days, ISNULL(CEILING(AVG(CAST(TAV.NOA_Days as FLOAT))), 0) as NOA_Days,
            ISNULL(CEILING(AVG(CAST(TAV.OEX_Days as FLOAT))), 0) as OEX_Days, ISNULL(CEILING(AVG(CAST(TAV.ODC_Days as FLOAT))), 0) as ODC_Days,
            ISNULL(CEILING(AVG(CAST(TAV.HIR_Days as FLOAT))), 0) as HIR_Days, ISNULL(CEILING(AVG(CAST(TAV.WDR_Days as FLOAT))), 0) as WDR_Days,
            COUNT(DISTINCT CONCAT(TAV.RequisitionId, '-', TAV.CandidateId)) as TotalCandidates
        FROM 
            [dbo].[TimingAnalyticsView] TAV
        GROUP BY  
            TAV.CreatedBy, TAV.RequisitionCode, TAV.CompanyName, TAV.Title
        ORDER BY 
            TAV.RequisitionCode
        FOR JSON PATH);
    
    SELECT ISNULL(@return, '[]');

    -- Query 2: Timing Analytics by Company (Admin view - all users)
    SELECT @return = 
        (SELECT 
            TAV.CreatedBy, TAV.CompanyName, ISNULL(CEILING(AVG(CAST(TAV.TimeToFill_Days as FLOAT))), 0) as TimeToFill_Days,
            ISNULL(CEILING(AVG(CAST(TAV.TimeToHire_Days as FLOAT))), 0) as TimeToHire_Days, ISNULL(CEILING(AVG(CAST(TAV.PEN_Days as FLOAT))), 0) as PEN_Days,
            ISNULL(CEILING(AVG(CAST(TAV.REJ_Days as FLOAT))), 0) as REJ_Days, ISNULL(CEILING(AVG(CAST(TAV.HLD_Days as FLOAT))), 0) as HLD_Days,
            ISNULL(CEILING(AVG(CAST(TAV.PHN_Days as FLOAT))), 0) as PHN_Days, ISNULL(CEILING(AVG(CAST(TAV.URW_Days as FLOAT))), 0) as URW_Days,
            ISNULL(CEILING(AVG(CAST(TAV.INT_Days as FLOAT))), 0) as INT_Days, ISNULL(CEILING(AVG(CAST(TAV.RHM_Days as FLOAT))), 0) as RHM_Days,
            ISNULL(CEILING(AVG(CAST(TAV.DEC_Days as FLOAT))), 0) as DEC_Days, ISNULL(CEILING(AVG(CAST(TAV.NOA_Days as FLOAT))), 0) as NOA_Days,
            ISNULL(CEILING(AVG(CAST(TAV.OEX_Days as FLOAT))), 0) as OEX_Days, ISNULL(CEILING(AVG(CAST(TAV.ODC_Days as FLOAT))), 0) as ODC_Days,
            ISNULL(CEILING(AVG(CAST(TAV.HIR_Days as FLOAT))), 0) as HIR_Days, ISNULL(CEILING(AVG(CAST(TAV.WDR_Days as FLOAT))), 0) as WDR_Days,
            COUNT(DISTINCT CONCAT(TAV.RequisitionId, '-', TAV.CandidateId)) as TotalCandidates, COUNT(DISTINCT TAV.RequisitionId) as TotalRequisitions,
            COUNT(DISTINCT TAV.CreatedBy) as TotalRecruiters
        FROM 
            [dbo].[TimingAnalyticsView] TAV
        GROUP BY 
            TAV.CreatedBy, TAV.CompanyName
        ORDER BY 
            TAV.CompanyName
        FOR JSON PATH);

    SELECT ISNULL(@return, '[]');

END;

CREATE OR ALTER PROCEDURE [dbo].[DashboardRecruiter_Refactor]
    @User varchar(10) = 'KEVIN'
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
    DECLARE @Today DATE = CAST(GETDATE() as date);

    SELECT 
        CreatedBy as KeyValue, FullName as Text
    FROM 
        dbo.ActiveUsersView 
    WHERE 
        CreatedBy = @User
    FOR JSON AUTO;
    
    
    /***************************************************************************
    * RESULT SET 1: SUBMISSION METRICS BY PERIOD (Refactored)
    * Uses pre-calculated SubmissionMetricsView instead of CTEs/temp tables
    ***************************************************************************/
    
    -- Single query replaces entire #BaseMetrics CTE and complex date logic
    SELECT ISNULL((
        SELECT 
            [CreatedBy] as [User], [Period], [Total_Submissions], [Active_Requisitions_Updated], [INT_Count] as INT_Submissions, [OEX_Count] as OEX_Submissions, 
            [HIR_Count] as HIR_Submissions, [OEX_HIR_Ratio]
        FROM 
            [dbo].[SubmissionMetricsView]
        WHERE 
            [CreatedBy] = @User
        ORDER BY 
            CASE [Period] 
                WHEN '7D' THEN 1 
                WHEN 'MTD' THEN 2 
                WHEN 'QTD' THEN 3 
                WHEN 'HYTD' THEN 4 
                WHEN 'YTD' THEN 5 
            END
        FOR JSON PATH
    ), '[]');

    /***************************************************************************
    * RESULT SET 2: CURRENT STATUS REPORT (Keep Original Logic)
    * Query 1: Current submission status for recruiter's candidates
    ***************************************************************************/
    
    -- Current Status Report for this recruiter's submissions
    DECLARE @return varchar(max) = '[]';

    SELECT @return =
        (SELECT 
            RAV.CompanyName + ' - [' + RAV.RequisitionCode + '] - ' + CAST(RAV.Positions as varchar(5)) + ' Positions - ' + RAV.RequisitionTitle as Company,
            RAV.CandidateName, RAV.CurrentStatus, RAV.DateFirstSubmitted, RAV.LastActivityDate, RAV.ActivityNotes, @User [User]
        FROM 
            [dbo].[RecentActivityView] RAV
        WHERE 
            RAV.CreatedBy = @User
        ORDER BY 
            RAV.SubmissionCount DESC, RAV.CompanyName ASC, RAV.RequisitionCode ASC, RAV.DateFirstSubmitted ASC
        FOR JSON PATH);

    SELECT ISNULL(@return, '[]');

    /***************************************************************************
    * RESULT SET 3: HIRED CANDIDATES REPORT (Keep Original Logic)
    * Query 1: Recent hires for this recruiter
    ***************************************************************************/
    
        SELECT @return =
            (SELECT 
                PRV.CompanyName as Company, PRV.RequisitionNumber, PRV.NumPosition, PRV.RequisitionTitle as Title, PRV.CandidateName, PRV.DateHired,
                PRV.SalaryOffered, PRV.StartDate, PRV.DateInvoiced, PRV.DatePaid, PRV.PlacementFee as [Placementfee], PRV.CommissionPercent,
                PRV.CommissionEarned, @User [User]
            FROM 
                [dbo].[PlacementReportView] PRV
            WHERE 
                PRV.CreatedBy = @User
            ORDER BY 
                PRV.CompanyName ASC, PRV.RequisitionNumber ASC, PRV.DateHired DESC
            FOR JSON PATH);

    SELECT ISNULL(@return, '[]');

    /***************************************************************************
    * RESULT SET 4: RECRUITER TIMING ANALYTICS (Keep Original Logic)
    * Query 1: Time to Fill, Time to Hire, and Time in Stage metrics (By Requisition) 
    * Query 2: Time to Fill, Time to Hire, and Time in Stage metrics (By Company)
    * Shows data for SPECIFIC USER submissions only (Recruiter perspective)
    ***************************************************************************/
    

    -- Query 1: Timing Analytics by Requisition (Recruiter view - user filtered)
    SELECT @return = 
        (SELECT 
            @User as [User], TAV.RequisitionCode, TAV.CompanyName, TAV.Title, ISNULL(CEILING(AVG(CAST(TAV.TimeToFill_Days as FLOAT))), 0) as TimeToFill_Days,
            ISNULL(CEILING(AVG(CAST(TAV.TimeToHire_Days as FLOAT))), 0) as TimeToHire_Days, ISNULL(CEILING(AVG(CAST(TAV.PEN_Days as FLOAT))), 0) as PEN_Days,
            ISNULL(CEILING(AVG(CAST(TAV.REJ_Days as FLOAT))), 0) as REJ_Days, ISNULL(CEILING(AVG(CAST(TAV.HLD_Days as FLOAT))), 0) as HLD_Days,
            ISNULL(CEILING(AVG(CAST(TAV.PHN_Days as FLOAT))), 0) as PHN_Days, ISNULL(CEILING(AVG(CAST(TAV.URW_Days as FLOAT))), 0) as URW_Days,
            ISNULL(CEILING(AVG(CAST(TAV.INT_Days as FLOAT))), 0) as INT_Days, ISNULL(CEILING(AVG(CAST(TAV.RHM_Days as FLOAT))), 0) as RHM_Days,
            ISNULL(CEILING(AVG(CAST(TAV.DEC_Days as FLOAT))), 0) as DEC_Days, ISNULL(CEILING(AVG(CAST(TAV.NOA_Days as FLOAT))), 0) as NOA_Days,
            ISNULL(CEILING(AVG(CAST(TAV.OEX_Days as FLOAT))), 0) as OEX_Days, ISNULL(CEILING(AVG(CAST(TAV.ODC_Days as FLOAT))), 0) as ODC_Days,
            ISNULL(CEILING(AVG(CAST(TAV.HIR_Days as FLOAT))), 0) as HIR_Days, ISNULL(CEILING(AVG(CAST(TAV.WDR_Days as FLOAT))), 0) as WDR_Days,
            COUNT(DISTINCT CONCAT(TAV.RequisitionId, '-', TAV.CandidateId)) as TotalCandidates
        FROM 
            [dbo].[TimingAnalyticsView] TAV
        WHERE 
            TAV.CreatedBy = @User AND TAV.Context = 'RECRUITER'
        GROUP BY 
            TAV.RequisitionCode, TAV.CompanyName, TAV.Title
        ORDER BY 
            TAV.RequisitionCode
        FOR JSON PATH);

    SELECT ISNULL(@return, '[]');
    
    SELECT @return = 
        (SELECT 
            @User as [User], TAV.CompanyName, ISNULL(CEILING(AVG(CAST(TAV.TimeToFill_Days as FLOAT))), 0) as TimeToFill_Days,
            ISNULL(CEILING(AVG(CAST(TAV.TimeToHire_Days as FLOAT))), 0) as TimeToHire_Days, ISNULL(CEILING(AVG(CAST(TAV.PEN_Days as FLOAT))), 0) as PEN_Days,
            ISNULL(CEILING(AVG(CAST(TAV.REJ_Days as FLOAT))), 0) as REJ_Days, ISNULL(CEILING(AVG(CAST(TAV.HLD_Days as FLOAT))), 0) as HLD_Days,
            ISNULL(CEILING(AVG(CAST(TAV.PHN_Days as FLOAT))), 0) as PHN_Days, ISNULL(CEILING(AVG(CAST(TAV.URW_Days as FLOAT))), 0) as URW_Days,
            ISNULL(CEILING(AVG(CAST(TAV.INT_Days as FLOAT))), 0) as INT_Days, ISNULL(CEILING(AVG(CAST(TAV.RHM_Days as FLOAT))), 0) as RHM_Days,
            ISNULL(CEILING(AVG(CAST(TAV.DEC_Days as FLOAT))), 0) as DEC_Days, ISNULL(CEILING(AVG(CAST(TAV.NOA_Days as FLOAT))), 0) as NOA_Days,
            ISNULL(CEILING(AVG(CAST(TAV.OEX_Days as FLOAT))), 0) as OEX_Days, ISNULL(CEILING(AVG(CAST(TAV.ODC_Days as FLOAT))), 0) as ODC_Days,
            ISNULL(CEILING(AVG(CAST(TAV.HIR_Days as FLOAT))), 0) as HIR_Days, ISNULL(CEILING(AVG(CAST(TAV.WDR_Days as FLOAT))), 0) as WDR_Days,
            COUNT(DISTINCT CONCAT(TAV.RequisitionId, '-', TAV.CandidateId)) as TotalCandidates, COUNT(DISTINCT TAV.RequisitionId) as TotalRequisitions
        FROM 
            [dbo].[TimingAnalyticsView] TAV
        WHERE 
            TAV.CreatedBy = @User AND TAV.Context = 'RECRUITER'  -- Recruiter context only
        GROUP BY 
            TAV.CompanyName
        ORDER BY 
            TAV.CompanyName
        FOR JSON PATH);

    SELECT ISNULL(@return, '[]');
    
END;