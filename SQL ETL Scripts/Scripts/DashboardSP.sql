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

ALTER PROCEDURE [dbo].[RefreshTimingAnalyticsView]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
    DECLARE @RefreshStart DATETIME2 = SYSDATETIME();
    DECLARE @Today DATE = GETDATE();
    
    -- Simple year-1 date calculation
    DECLARE @StartDate365 DATE = DATEADD(YEAR, -1, @Today);
    
    BEGIN TRY
        -- Clear existing data (nuclear refresh approach)
        DELETE FROM [dbo].[TimingAnalyticsView];
        
        -- =====================================================================================
        -- CONTEXT 1: ACCOUNTS MANAGER PERSPECTIVE (R.CreatedBy - Requisition Owner)
        -- =====================================================================================
        
        WITH AM_RequisitionBase AS (
            -- Get requisitions created by AMs/Full Desk users in last year
            SELECT 
                R.Id as RequisitionId, R.Code as RequisitionCode, ISNULL(C.CompanyName, 'Unknown Company') as CompanyName, R.PosTitle as Title,
                R.CreatedDate as RequisitionCreatedDate, R.UpdatedDate as RequisitionUpdatedDate, R.Status as RequisitionStatus, R.CreatedBy,
                CASE 
                    WHEN R.Status = 'FUL' THEN DATEDIFF(DAY, R.CreatedDate, R.UpdatedDate)
                    ELSE NULL 
                END as TimeToFill
            FROM 
                Requisitions R LEFT JOIN Companies C ON R.CompanyId = C.ID
                INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName 
            WHERE 
                U.Status = 1  AND U.Role IN (5, 6) AND CAST(R.CreatedDate AS DATE) >= @StartDate365
                AND R.CreatedBy IS NOT NULL
        ),
        AM_FirstSubmissions AS (
            -- Get first submission date for each candidate+requisition combo
            SELECT 
                S.RequisitionId, S.CandidateId, MIN(S.CreatedDate) as FirstSubmissionDate
            FROM 
                Submissions S INNER JOIN AM_RequisitionBase ARB ON S.RequisitionId = ARB.RequisitionId
            GROUP BY 
                S.RequisitionId, S.CandidateId
        ),
        AM_TimeToHireCalc AS (
            -- Calculate Time to Hire for hired candidates
            SELECT 
                AFS.RequisitionId, AFS.CandidateId, AFS.FirstSubmissionDate, MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HireDate,
                MAX(CASE WHEN S.Status = 'HIR' THEN DATEDIFF(DAY, AFS.FirstSubmissionDate, S.CreatedDate) END) as TimeToHire
            FROM 
                AM_FirstSubmissions AFS INNER JOIN Submissions S ON AFS.RequisitionId = S.RequisitionId AND AFS.CandidateId = S.CandidateId
            GROUP BY 
                AFS.RequisitionId, AFS.CandidateId, AFS.FirstSubmissionDate
        ),
        AM_SubmissionHistory AS (
            -- Get all submission history with next submission date for gap calculation
            SELECT 
                S.RequisitionId, S.CandidateId, S.Status, S.CreatedDate,
                LEAD(S.CreatedDate) OVER (PARTITION BY S.RequisitionId, S.CandidateId ORDER BY S.CreatedDate) as NextSubmissionDate,
                CASE WHEN ROW_NUMBER() OVER (PARTITION BY S.RequisitionId, S.CandidateId ORDER BY S.CreatedDate DESC) = 1 THEN 1 ELSE 0 END as IsCurrentStatus
            FROM 
                Submissions S INNER JOIN AM_FirstSubmissions AFS ON S.RequisitionId = AFS.RequisitionId AND S.CandidateId = AFS.CandidateId
        ),
        AM_StageTimeCalculations AS (
            -- Calculate CUMULATIVE days spent in each stage (sum all periods per status)
            SELECT 
                AFS.RequisitionId, AFS.CandidateId,
                -- PEN_Days: Sum all time spent in PEN status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'PEN' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as PEN_Days,
                -- REJ_Days: Sum all time spent in REJ status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'REJ' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as REJ_Days,
                -- HLD_Days: Sum all time spent in HLD status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'HLD' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as HLD_Days,
                -- PHN_Days: Sum all time spent in PHN status (THIS WILL HANDLE MULTIPLE PHN PERIODS!)
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'PHN' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as PHN_Days,
                -- URW_Days: Sum all time spent in URW status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'URW' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as URW_Days,
                -- INT_Days: Sum all time spent in INT status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'INT' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as INT_Days,
                -- RHM_Days: Sum all time spent in RHM status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'RHM' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as RHM_Days,
                -- DEC_Days: Sum all time spent in DEC status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'DEC' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as DEC_Days,
                -- NOA_Days: Sum all time spent in NOA status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'NOA' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as NOA_Days,
                
                -- OEX_Days: Sum all time spent in OEX status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'OEX' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as OEX_Days,
                -- ODC_Days: Sum all time spent in ODC status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'ODC' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as ODC_Days,
                
                -- HIR_Days: Sum all time spent in HIR status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'HIR' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as HIR_Days,
                -- WDR_Days: Sum all time spent in WDR status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'WDR' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as WDR_Days
            FROM 
                AM_FirstSubmissions AFS INNER JOIN AM_SubmissionHistory ASH ON AFS.RequisitionId = ASH.RequisitionId AND AFS.CandidateId = ASH.CandidateId
            GROUP BY 
                AFS.RequisitionId, AFS.CandidateId
        )
        
        -- Insert AM context data
        INSERT INTO [dbo].[TimingAnalyticsView] 
            ([Context], [CreatedBy], [RequisitionId], [RequisitionCode], [CompanyName], [Title], [CandidateId], [RequisitionCreatedDate], [RequisitionUpdatedDate],
            [RequisitionStatus], [TimeToFill_Days], [TimeToHire_Days], [FirstSubmissionDate], [HireDate], [PEN_Days], [REJ_Days], [HLD_Days], [PHN_Days], [URW_Days], 
            [INT_Days], [RHM_Days], [DEC_Days], [NOA_Days], [OEX_Days], [ODC_Days], [HIR_Days], [WDR_Days], [RefreshDate])
        SELECT 
            'AM' as Context, ARB.CreatedBy, ARB.RequisitionId, ARB.RequisitionCode, ARB.CompanyName, ARB.Title, AFS.CandidateId, ARB.RequisitionCreatedDate,
            ARB.RequisitionUpdatedDate, ARB.RequisitionStatus, ARB.TimeToFill, ATTH.TimeToHire, AFS.FirstSubmissionDate, ATTH.HireDate, ASTC.PEN_Days, ASTC.REJ_Days,
            ASTC.HLD_Days, ASTC.PHN_Days, ASTC.URW_Days, ASTC.INT_Days, ASTC.RHM_Days, ASTC.DEC_Days, ASTC.NOA_Days, ASTC.OEX_Days, ASTC.ODC_Days, ASTC.HIR_Days,
            ASTC.WDR_Days, @RefreshStart
        FROM 
            AM_RequisitionBase ARB INNER JOIN AM_FirstSubmissions AFS ON ARB.RequisitionId = AFS.RequisitionId
            LEFT JOIN AM_TimeToHireCalc ATTH ON AFS.RequisitionId = ATTH.RequisitionId AND AFS.CandidateId = ATTH.CandidateId
            LEFT JOIN AM_StageTimeCalculations ASTC ON AFS.RequisitionId = ASTC.RequisitionId AND AFS.CandidateId = ASTC.CandidateId;
        
        -- =====================================================================================
        -- CONTEXT 2: RECRUITER PERSPECTIVE (S.CreatedBy - Submission Owner)
        -- =====================================================================================
        
        WITH REC_SubmissionBase AS (
            -- Get submissions created by Recruiters/Full Desk users in last year
            SELECT DISTINCT
                S.RequisitionId, S.CandidateId, S.CreatedBy as SubmissionOwner, MIN(S.CreatedDate) as FirstSubmissionDate
            FROM 
                Submissions S INNER JOIN dbo.Users U ON S.CreatedBy = U.UserName 
            WHERE 
                U.Status = 1  AND U.Role IN (4, 6) AND CAST(S.CreatedDate AS DATE) >= @StartDate365 AND S.CreatedBy IS NOT NULL
            GROUP BY 
                S.RequisitionId, S.CandidateId, S.CreatedBy
        ),
        REC_RequisitionDetails AS (
            -- Get requisition details for recruiter submissions
            SELECT 
                RSB.RequisitionId, RSB.CandidateId, RSB.SubmissionOwner, RSB.FirstSubmissionDate, R.Code as RequisitionCode,
                ISNULL(C.CompanyName, 'Unknown Company') as CompanyName, R.PosTitle as Title, R.CreatedDate as RequisitionCreatedDate, R.UpdatedDate as RequisitionUpdatedDate,
                R.Status as RequisitionStatus,
                -- Time to Fill calculation (only for FUL status)
                CASE 
                    WHEN R.Status = 'FUL' THEN DATEDIFF(DAY, R.CreatedDate, R.UpdatedDate)
                    ELSE NULL 
                END as TimeToFill
            FROM 
                REC_SubmissionBase RSB INNER JOIN Requisitions R ON RSB.RequisitionId = R.Id
                LEFT JOIN Companies C ON R.CompanyId = C.ID
        ),
        REC_TimeToHireCalc AS (
            -- Calculate Time to Hire for hired candidates (recruiter context)
            SELECT 
                RSB.RequisitionId,
                RSB.CandidateId,
                RSB.SubmissionOwner,
                RSB.FirstSubmissionDate,
                MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HireDate,
                MAX(CASE WHEN S.Status = 'HIR' THEN DATEDIFF(DAY, RSB.FirstSubmissionDate, S.CreatedDate) END) as TimeToHire
            FROM REC_SubmissionBase RSB
            INNER JOIN Submissions S ON RSB.RequisitionId = S.RequisitionId AND RSB.CandidateId = S.CandidateId
            GROUP BY RSB.RequisitionId, RSB.CandidateId, RSB.SubmissionOwner, RSB.FirstSubmissionDate
        ),
        REC_SubmissionHistory AS (
            -- Get all submission history for recruiter context
            SELECT 
                S.RequisitionId, S.CandidateId, RSB.SubmissionOwner, S.Status, S.CreatedDate,
                LEAD(S.CreatedDate) OVER (PARTITION BY S.RequisitionId, S.CandidateId ORDER BY S.CreatedDate) as NextSubmissionDate,
                CASE WHEN ROW_NUMBER() OVER (PARTITION BY S.RequisitionId, S.CandidateId ORDER BY S.CreatedDate DESC) = 1 THEN 1 ELSE 0 END as IsCurrentStatus
            FROM 
                Submissions S INNER JOIN REC_SubmissionBase RSB ON S.RequisitionId = RSB.RequisitionId AND S.CandidateId = RSB.CandidateId
        ),
        REC_StageTimeCalculations AS (
            -- Calculate CUMULATIVE days spent in each stage (recruiter context)
            SELECT 
                RSB.RequisitionId, RSB.CandidateId, RSB.SubmissionOwner,
                -- Sum all time periods for each status (same logic as AM)
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'PEN' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as PEN_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'REJ' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as REJ_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'HLD' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as HLD_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'PHN' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as PHN_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'URW' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as URW_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'INT' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as INT_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'RHM' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as RHM_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'DEC' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as DEC_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'NOA' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as NOA_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'OEX' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as OEX_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'ODC' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as ODC_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'HIR' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as HIR_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'WDR' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as WDR_Days
            FROM 
                REC_SubmissionBase RSB INNER JOIN REC_SubmissionHistory RSH ON RSB.RequisitionId = RSH.RequisitionId AND RSB.CandidateId = RSH.CandidateId 
                AND RSB.SubmissionOwner = RSH.SubmissionOwner
            GROUP BY 
                RSB.RequisitionId, RSB.CandidateId, RSB.SubmissionOwner
        )
        
        -- Insert Recruiter context data
        INSERT INTO [dbo].[TimingAnalyticsView] 
            ([Context], [CreatedBy], [RequisitionId], [RequisitionCode], [CompanyName], [Title], [CandidateId], [RequisitionCreatedDate], [RequisitionUpdatedDate], 
            [RequisitionStatus], [TimeToFill_Days], [TimeToHire_Days], [FirstSubmissionDate], [HireDate], [PEN_Days], [REJ_Days], [HLD_Days], [PHN_Days], [URW_Days], 
            [INT_Days], [RHM_Days], [DEC_Days], [NOA_Days], [OEX_Days], [ODC_Days], [HIR_Days], [WDR_Days], [RefreshDate])
        SELECT 
            'RECRUITER' as Context, RRD.SubmissionOwner as CreatedBy, RRD.RequisitionId, RRD.RequisitionCode, RRD.CompanyName, RRD.Title, RRD.CandidateId,
            RRD.RequisitionCreatedDate, RRD.RequisitionUpdatedDate, RRD.RequisitionStatus, RRD.TimeToFill, RTTH.TimeToHire, RRD.FirstSubmissionDate, RTTH.HireDate, 
            RSTC.PEN_Days, RSTC.REJ_Days, RSTC.HLD_Days, RSTC.PHN_Days, RSTC.URW_Days, RSTC.INT_Days, RSTC.RHM_Days, RSTC.DEC_Days, RSTC.NOA_Days, RSTC.OEX_Days, 
            RSTC.ODC_Days, RSTC.HIR_Days, RSTC.WDR_Days, @RefreshStart
        FROM 
            REC_RequisitionDetails RRD LEFT JOIN REC_TimeToHireCalc RTTH ON RRD.RequisitionId = RTTH.RequisitionId  
            AND RRD.CandidateId = RTTH.CandidateId AND RRD.SubmissionOwner = RTTH.SubmissionOwner
            LEFT JOIN REC_StageTimeCalculations RSTC ON RRD.RequisitionId = RSTC.RequisitionId 
            AND RRD.CandidateId = RSTC.CandidateId AND RRD.SubmissionOwner = RSTC.SubmissionOwner;
        
        -- Log successful refresh
        DECLARE @RowCount INT = @@ROWCOUNT;
        DECLARE @RefreshEnd DATETIME2 = SYSDATETIME();
        DECLARE @Duration INT = DATEDIFF(MILLISECOND, @RefreshStart, @RefreshEnd);
        
        PRINT 'TimingAnalyticsView refresh completed successfully.';
        PRINT 'Rows processed: ' + CAST(@RowCount AS VARCHAR(10));
        PRINT 'Duration: ' + CAST(@Duration AS VARCHAR(10)) + ' ms';
        
    END TRY
    BEGIN CATCH
        -- Error handling
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorLine INT = ERROR_LINE();
        
        PRINT 'Error in TimingAnalyticsView refresh:';
        PRINT 'Line: ' + CAST(@ErrorLine AS VARCHAR(10));
        PRINT 'Message: ' + @ErrorMessage;
        
        -- Re-throw the error
        THROW;
    END CATCH
END;

GO

ALTER PROCEDURE [dbo].[RefreshRecentActivityView]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
    DECLARE @RefreshStart DATETIME2 = SYSDATETIME();
    DECLARE @Today DATE = GETDATE();
    
    -- Simple month-1 date calculation (no complex logic)
    DECLARE @StartDate1Month DATE = DATEADD(MONTH, -1, @Today);
    
    BEGIN TRY
        -- Clear existing data (nuclear refresh approach)
        DELETE FROM [dbo].[RecentActivityView];
        
        -- =====================================================================================
        -- RECENT ACTIVITY CALCULATION - REPLICATE CURRENT SET 2 LOGIC
        -- =====================================================================================
        
        WITH CompanySubmissionCounts AS (
            -- Get submission counts by company for sorting (across all active users)
            SELECT 
                C.CompanyName, COUNT(DISTINCT CAST(S.RequisitionId AS VARCHAR(10)) + '-' + CAST(S.CandidateId AS VARCHAR(10))) as SubmissionCount
            FROM 
                Submissions S INNER JOIN Requisitions R ON R.Id = S.RequisitionId
                INNER JOIN Companies C ON C.ID = R.CompanyId
                INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1  -- Only active users
            WHERE 
                S.CreatedDate >= @StartDate1Month  AND S.CreatedDate <= DATEADD(DAY, 1, @Today) AND R.CreatedBy IS NOT NULL
            GROUP BY 
                C.CompanyName
        ),
        SubmissionSummary AS (
            -- Get first and last activity for each Req+Cand combination (for all active users)
            SELECT 
                R.CreatedBy, S.RequisitionId, S.CandidateId, MIN(S.CreatedDate) as DateFirstSubmitted, MAX(S.CreatedDate) as LastActivityDate
            FROM 
                Submissions S INNER JOIN Requisitions R ON R.Id = S.RequisitionId
                INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1  -- Only active users
            WHERE 
                R.CreatedBy IS NOT NULL
            GROUP BY 
                R.CreatedBy, S.RequisitionId, S.CandidateId
            HAVING 
                MIN(S.CreatedDate) >= @StartDate1Month AND MIN(S.CreatedDate) <= DATEADD(DAY, 1, @Today)
        ),
        LastActivity AS (
            -- Get the status and notes from the most recent submission
            SELECT 
                SS.CreatedBy, SS.RequisitionId, SS.CandidateId, S.Status, S.Notes, S.CreatedDate,
                ROW_NUMBER() OVER (PARTITION BY SS.RequisitionId, SS.CandidateId ORDER BY S.CreatedDate DESC) as RN
            FROM 
                SubmissionSummary SS INNER JOIN Submissions S ON SS.RequisitionId = S.RequisitionId  AND SS.CandidateId = S.CandidateId
        ),
        RecentActivityData AS (
            -- Combine all data for final insert
            SELECT 
                SS.CreatedBy, SS.RequisitionId, SS.CandidateId, C.CompanyName, R.Code as RequisitionCode, R.PosTitle as RequisitionTitle, R.Positions,
                LTRIM(RTRIM(ISNULL(CAND.FirstName, '') + ' ' + ISNULL(CAND.LastName, ''))) as CandidateName, LA.Status as CurrentStatus, SS.DateFirstSubmitted,
                SS.LastActivityDate, ISNULL(LA.Notes, '') as ActivityNotes, CSC.SubmissionCount, @RefreshStart as RefreshDate
            FROM 
                SubmissionSummary SS INNER JOIN LastActivity LA ON LA.CreatedBy = SS.CreatedBy  AND LA.RequisitionId = SS.RequisitionId 
                AND LA.CandidateId = SS.CandidateId AND LA.RN = 1
                INNER JOIN Requisitions R ON R.Id = SS.RequisitionId
                INNER JOIN Companies C ON C.ID = R.CompanyId
                INNER JOIN Candidate CAND ON SS.CandidateId = CAND.ID
                INNER JOIN CompanySubmissionCounts CSC ON C.CompanyName = CSC.CompanyName
        )
        
        -- Insert all recent activity data
        INSERT INTO [dbo].[RecentActivityView] 
            ([CreatedBy], [RequisitionId], [CandidateId], [CompanyName], [RequisitionCode], [RequisitionTitle], [Positions], [CandidateName], [CurrentStatus], 
            [DateFirstSubmitted], [LastActivityDate], [ActivityNotes], [SubmissionCount], [RefreshDate])
        SELECT 
            CreatedBy, RequisitionId, CandidateId, CompanyName, RequisitionCode, RequisitionTitle, Positions, CandidateName, CurrentStatus, DateFirstSubmitted,
            LastActivityDate, ActivityNotes, SubmissionCount, RefreshDate
        FROM 
            RecentActivityData;
        
        -- Log successful refresh
        DECLARE @RowCount INT = @@ROWCOUNT;
        DECLARE @RefreshEnd DATETIME2 = SYSDATETIME();
        DECLARE @Duration INT = DATEDIFF(MILLISECOND, @RefreshStart, @RefreshEnd);
        
        PRINT 'RecentActivityView refresh completed successfully.';
        PRINT 'Date range: ' + CAST(@StartDate1Month AS VARCHAR(20)) + ' to ' + CAST(@Today AS VARCHAR(20));
        PRINT 'Rows processed: ' + CAST(@RowCount AS VARCHAR(10));
        PRINT 'Duration: ' + CAST(@Duration AS VARCHAR(10)) + ' ms';
        
    END TRY
    BEGIN CATCH
        -- Error handling
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorLine INT = ERROR_LINE();
        
        PRINT 'Error in RecentActivityView refresh:';
        PRINT 'Line: ' + CAST(@ErrorLine AS VARCHAR(10));
        PRINT 'Message: ' + @ErrorMessage;
        
        -- Re-throw the error
        THROW;
    END CATCH
END;

GO

ALTER PROCEDURE [dbo].[RefreshPlacementReportView]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
    DECLARE @RefreshStart DATETIME2 = SYSDATETIME();
    DECLARE @Today DATE = GETDATE();
    
    -- Simple 3-month back date calculation (no complex logic)
    DECLARE @StartDate3Month DATE = DATEADD(MONTH, -3, @Today);
    
    BEGIN TRY
        -- Clear existing data (nuclear refresh approach)
        DELETE FROM [dbo].[PlacementReportView];
        
        -- =====================================================================================
        -- PLACEMENT REPORT CALCULATION - REPLICATE CURRENT SET 3 LOGIC
        -- =====================================================================================
        
        WITH HiredCandidates AS (
            -- Get the latest HIR record for each Req+Cand combination (across all active users)
            SELECT 
                R.CreatedBy, S.RequisitionId, S.CandidateId, MAX(S.CreatedDate) as DateHired,
                ROW_NUMBER() OVER (PARTITION BY S.RequisitionId, S.CandidateId ORDER BY MAX(S.CreatedDate) DESC) as RN
            FROM 
                Submissions S INNER JOIN Requisitions R ON R.Id = S.RequisitionId
                INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1  -- Only active users
            WHERE 
                S.Status = 'HIR' AND R.CreatedBy IS NOT NULL
            GROUP BY 
                R.CreatedBy, S.RequisitionId, S.CandidateId
            HAVING 
                MAX(S.CreatedDate) >= @StartDate3Month AND MAX(S.CreatedDate) <= DATEADD(DAY, 1, @Today)
        ),
        PlacementData AS (
            -- Combine all placement data for final insert
            SELECT 
                HC.CreatedBy, HC.RequisitionId, HC.CandidateId, C.CompanyName, R.Code as RequisitionNumber, R.Positions as NumPosition,
                R.PosTitle as RequisitionTitle, LTRIM(RTRIM(ISNULL(CAND.FirstName, '') + ' ' + ISNULL(CAND.LastName, ''))) as CandidateName,
                CAST(HC.DateHired AS DATE) as DateHired, CAST(0.00 AS DECIMAL(9,2)) as SalaryOffered, CAST(@Today AS DATE) as StartDate, CAST(@Today AS DATE) as DateInvoiced,
                CAST(@Today AS DATE) as DatePaid, CAST(0.00 AS DECIMAL(9,2)) as PlacementFee, CAST(0.00 AS DECIMAL(5,2)) as CommissionPercent,
                CAST(0.00 AS DECIMAL(9,2)) as CommissionEarned, @RefreshStart as RefreshDate
            FROM 
                HiredCandidates HC INNER JOIN Requisitions R ON R.Id = HC.RequisitionId
                INNER JOIN Companies C ON C.ID = R.CompanyId
                INNER JOIN Candidate CAND ON CAND.ID = HC.CandidateId
            WHERE 
                HC.RN = 1  -- Only the latest hire record
        )
        
        -- Insert all placement data
        INSERT INTO [dbo].[PlacementReportView] 
            ([CreatedBy], [RequisitionId], [CandidateId], [CompanyName], [RequisitionNumber], [NumPosition], [RequisitionTitle], [CandidateName], [DateHired], 
            [SalaryOffered], [StartDate], [DateInvoiced], [DatePaid], [PlacementFee], [CommissionPercent], [CommissionEarned], [RefreshDate])
        SELECT 
            CreatedBy, RequisitionId, CandidateId, CompanyName, RequisitionNumber, NumPosition, RequisitionTitle, CandidateName, DateHired, SalaryOffered, StartDate,
            DateInvoiced, DatePaid, PlacementFee, CommissionPercent, CommissionEarned, RefreshDate
        FROM 
            PlacementData;
        
        -- Log successful refresh
        DECLARE @RowCount INT = @@ROWCOUNT;
        DECLARE @RefreshEnd DATETIME2 = SYSDATETIME();
        DECLARE @Duration INT = DATEDIFF(MILLISECOND, @RefreshStart, @RefreshEnd);
        
        PRINT 'PlacementReportView refresh completed successfully.';
        PRINT 'Date range: ' + CAST(@StartDate3Month AS VARCHAR(20)) + ' to ' + CAST(@Today AS VARCHAR(20));
        PRINT 'Hired candidates processed: ' + CAST(@RowCount AS VARCHAR(10));
        PRINT 'Duration: ' + CAST(@Duration AS VARCHAR(10)) + ' ms';
        
    END TRY
    BEGIN CATCH
        -- Error handling
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorLine INT = ERROR_LINE();
        
        PRINT 'Error in PlacementReportView refresh:';
        PRINT 'Line: ' + CAST(@ErrorLine AS VARCHAR(10));
        PRINT 'Message: ' + @ErrorMessage;
        
        -- Re-throw the error
        THROW;
    END CATCH
END;

GO

ALTER   PROCEDURE [dbo].[Dashboard_Refactor_SubmissionMetrics_AccountsManager]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
    DECLARE @RefreshStart DATETIME2 = SYSDATETIME();
    DECLARE @Today DATE = GETDATE();
    
    BEGIN TRY
        -- ========================================================================
        -- ACCOUNTS MANAGER LOGIC - COPY FROM DASHBOARDACCOUNTSMANAGER
        -- Key difference: Uses R.CreatedBy (requisition owner) vs S.CreatedBy (submitter)
        -- ========================================================================
        
        -- Date calculations (same as DashboardAccountsManager)
        DECLARE @YearStart DATE = DATEADD(YEAR, DATEDIFF(YEAR, 0, @Today), 0);
        DECLARE @StartDate7D DATE = DATEADD(DAY, -7, @Today);
        DECLARE @StartDateMTD DATE = DATEADD(MONTH, DATEDIFF(MONTH, 0, @Today), 0);
        DECLARE @StartDateQTD DATE = DATEADD(QUARTER, DATEDIFF(QUARTER, 0, @Today), 0);
        DECLARE @StartDateHYTD DATE = CASE WHEN MONTH(@Today) <= 6 THEN DATEFROMPARTS(YEAR(@Today), 1, 1) ELSE DATEFROMPARTS(YEAR(@Today), 7, 1) END;
        
        -- Single INSERT using JOINs for all accounts managers and all periods
        WITH AllAccountsManagers AS (
            SELECT 
                CreatedBy As UserName
            FROM 
                dbo.ActiveUsersView 
            WHERE 
                Role IN (5, 6) AND Status = 1  -- Role 5 = AM, Role 6 = Full Desk
        ),
        RequisitionMetrics AS (
            -- #RequisitionMetrics logic - requisitions they created
            SELECT 
                U.UserName as [User],
                -- Total requisitions created (by Updated date)
                ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDate7D AND @Today THEN 1 ELSE 0 END), 0) as TOTAL_LAST7D_COUNT,
                ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDateMTD AND @Today THEN 1 ELSE 0 END), 0) as TOTAL_MTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDateQTD AND @Today THEN 1 ELSE 0 END), 0) as TOTAL_QTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDateHYTD AND @Today THEN 1 ELSE 0 END), 0) as TOTAL_HYTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @YearStart AND @Today THEN 1 ELSE 0 END), 0) as TOTAL_YTD_COUNT,
                -- Active requisitions (OPN, NEW, PAR)
                ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDate7D AND @Today AND R.Status IN ('OPN','NEW','PAR') THEN 1 ELSE 0 END), 0) as ACTIVE_LAST7D_COUNT,
                ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDateMTD AND @Today AND R.Status IN ('OPN','NEW','PAR') THEN 1 ELSE 0 END), 0) as ACTIVE_MTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDateQTD AND @Today AND R.Status IN ('OPN','NEW','PAR') THEN 1 ELSE 0 END), 0) as ACTIVE_QTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDateHYTD AND @Today AND R.Status IN ('OPN','NEW','PAR') THEN 1 ELSE 0 END), 0) as ACTIVE_HYTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @YearStart AND @Today AND R.Status IN ('OPN','NEW','PAR') THEN 1 ELSE 0 END), 0) as ACTIVE_YTD_COUNT
            FROM 
                AllAccountsManagers U LEFT JOIN RequisitionView R ON R.CreatedBy = U.UserName
            GROUP BY 
                U.UserName
        ),
        SubmissionMetrics AS (
            -- #SubmissionMetrics logic - submissions TO their requisitions
            SELECT 
                U.UserName as [User],
                -- Interview status counts (submissions to their requisitions)
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND SS.Status = 'INT' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as INT_LAST7D_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND SS.Status = 'INT' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as INT_MTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND SS.Status = 'INT' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as INT_QTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND SS.Status = 'INT' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as INT_HYTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @YearStart AND @Today AND SS.Status = 'INT' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as INT_YTD_COUNT,
                -- Offer extended counts
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND SS.Status = 'OEX' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as OEX_LAST7D_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND SS.Status = 'OEX' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as OEX_MTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND SS.Status = 'OEX' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as OEX_QTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND SS.Status = 'OEX' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as OEX_HYTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @YearStart AND @Today AND SS.Status = 'OEX' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as OEX_YTD_COUNT,
                -- Hired counts
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND SS.Status = 'HIR' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as HIR_LAST7D_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND SS.Status = 'HIR' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as HIR_MTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND SS.Status = 'HIR' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as HIR_QTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND SS.Status = 'HIR' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as HIR_HYTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @YearStart AND @Today AND SS.Status = 'HIR' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as HIR_YTD_COUNT
            FROM AllAccountsManagers U
            LEFT JOIN (
                -- StatusSubmissions CTE logic
                SELECT 
                    S.RequisitionId, S.CandidateId, S.Status, MAX(S.CreatedDate) as LatestStatusDate, R.CreatedBy as RequisitionOwner
                FROM 
                    Submissions S INNER JOIN Requisitions R ON R.Id = S.RequisitionId
                WHERE 
                    S.Status IN ('INT', 'OEX', 'HIR')
                GROUP BY 
                    S.RequisitionId, S.CandidateId, S.Status, R.CreatedBy
            ) SS ON SS.RequisitionOwner = U.UserName
            GROUP BY 
                U.UserName
        )
        
        -- Generate all period rows for all accounts managers
        INSERT INTO 
            [dbo].[SubmissionMetricsView] 
            ([CreatedBy], [MetricDate], [Period], [INT_Count], [OEX_Count], [HIR_Count], [Total_Submissions], [OEX_HIR_Ratio], [Requisitions_Created], 
            [Active_Requisitions_Updated], [RefreshDate])
        SELECT 
            RM.[User] as CreatedBy, @Today as MetricDate, P.Period,
            CASE P.Period
                WHEN '7D' THEN SM.INT_LAST7D_COUNT
                WHEN 'MTD' THEN SM.INT_MTD_COUNT
                WHEN 'QTD' THEN SM.INT_QTD_COUNT
                WHEN 'HYTD' THEN SM.INT_HYTD_COUNT
                WHEN 'YTD' THEN SM.INT_YTD_COUNT
            END as INT_Count,
            CASE P.Period
                WHEN '7D' THEN SM.OEX_LAST7D_COUNT
                WHEN 'MTD' THEN SM.OEX_MTD_COUNT
                WHEN 'QTD' THEN SM.OEX_QTD_COUNT
                WHEN 'HYTD' THEN SM.OEX_HYTD_COUNT
                WHEN 'YTD' THEN SM.OEX_YTD_COUNT
            END as OEX_Count,
            CASE P.Period
                WHEN '7D' THEN SM.HIR_LAST7D_COUNT
                WHEN 'MTD' THEN SM.HIR_MTD_COUNT
                WHEN 'QTD' THEN SM.HIR_QTD_COUNT
                WHEN 'HYTD' THEN SM.HIR_HYTD_COUNT
                WHEN 'YTD' THEN SM.HIR_YTD_COUNT
            END as HIR_Count,
            0 as Total_Submissions,
            CASE 
                WHEN CASE P.Period
                    WHEN '7D' THEN SM.OEX_LAST7D_COUNT
                    WHEN 'MTD' THEN SM.OEX_MTD_COUNT
                    WHEN 'QTD' THEN SM.OEX_QTD_COUNT
                    WHEN 'HYTD' THEN SM.OEX_HYTD_COUNT
                    WHEN 'YTD' THEN SM.OEX_YTD_COUNT
                END = 0 THEN 0.00
                ELSE CAST(
                    CASE P.Period
                        WHEN '7D' THEN SM.HIR_LAST7D_COUNT
                        WHEN 'MTD' THEN SM.HIR_MTD_COUNT
                        WHEN 'QTD' THEN SM.HIR_QTD_COUNT
                        WHEN 'HYTD' THEN SM.HIR_HYTD_COUNT
                        WHEN 'YTD' THEN SM.HIR_YTD_COUNT
                    END AS DECIMAL(5,2)
                ) / CAST(
                    CASE P.Period
                        WHEN '7D' THEN SM.OEX_LAST7D_COUNT
                        WHEN 'MTD' THEN SM.OEX_MTD_COUNT
                        WHEN 'QTD' THEN SM.OEX_QTD_COUNT
                        WHEN 'HYTD' THEN SM.OEX_HYTD_COUNT
                        WHEN 'YTD' THEN SM.OEX_YTD_COUNT
                    END AS DECIMAL(5,2)
                )
            END as OEX_HIR_Ratio,
            CASE P.Period
                WHEN '7D' THEN RM.TOTAL_LAST7D_COUNT
                WHEN 'MTD' THEN RM.TOTAL_MTD_COUNT
                WHEN 'QTD' THEN RM.TOTAL_QTD_COUNT
                WHEN 'HYTD' THEN RM.TOTAL_HYTD_COUNT
                WHEN 'YTD' THEN RM.TOTAL_YTD_COUNT
            END as Requisitions_Created,
            CASE P.Period
                WHEN '7D' THEN RM.ACTIVE_LAST7D_COUNT
                WHEN 'MTD' THEN RM.ACTIVE_MTD_COUNT
                WHEN 'QTD' THEN RM.ACTIVE_QTD_COUNT
                WHEN 'HYTD' THEN RM.ACTIVE_HYTD_COUNT
                WHEN 'YTD' THEN RM.ACTIVE_YTD_COUNT
            END as Active_Requisitions_Updated, GETDATE() as RefreshDate
        FROM 
            RequisitionMetrics RM CROSS JOIN (VALUES ('7D'), ('MTD'), ('QTD'), ('HYTD'), ('YTD')) P(Period)
            LEFT JOIN SubmissionMetrics SM ON RM.[User] = SM.[User];
        
        -- Log success
        DECLARE @Duration_MS INT = DATEDIFF(MILLISECOND, @RefreshStart, SYSDATETIME());
        DECLARE @RowCount INT = @@ROWCOUNT;
        PRINT 'AccountsManager SubmissionMetricsView refreshed successfully - ' + CAST(@RowCount AS VARCHAR(10)) + ' rows in ' + CAST(@Duration_MS AS VARCHAR(10)) + 'ms';
        
    END TRY
    BEGIN CATCH
        -- Log error details
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        PRINT 'Error refreshing AccountsManager SubmissionMetricsView: ' + @ErrorMessage;
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;

GO

ALTER   PROCEDURE [dbo].[Dashboard_Refactor_SubmissionMetrics_Recruiter]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
    DECLARE @RefreshStart DATETIME2 = SYSDATETIME();
    DECLARE @Today DATE = CAST(GETDATE() as date);
    
    BEGIN TRY
        -- ========================================================================
        -- JOIN-BASED APPROACH - SINGLE QUERY FOR ALL RECRUITERS
        -- ========================================================================
        
        -- Date calculations (same for all users)
        DECLARE @YearStart DATE = DATEFROMPARTS(YEAR(@Today), 1, 1);
        DECLARE @StartDate7D DATE = DATEADD(DAY, -6, @Today);
        DECLARE @StartDateMTD DATE = DATEADD(DAY, 1, EOMONTH(@Today, -1));
        DECLARE @StartDateQTD DATE = DATEADD(QUARTER, DATEDIFF(QUARTER, 0, @Today), 0);
        DECLARE @StartDateHYTD DATE = CASE 
            WHEN MONTH(@Today) <= 6 THEN DATEFROMPARTS(YEAR(@Today), 1, 1)
            ELSE DATEFROMPARTS(YEAR(@Today), 7, 1)
        END;
        
        -- Single INSERT using JOINs for all recruiters and all periods
        WITH AllRecruiters AS (
            SELECT CreatedBy as UserName, 
                   -- Adjust YearStart per user based on their earliest submission
                   CASE 
                       WHEN (SELECT MIN(CAST(CreatedDate AS DATE)) FROM Submissions WHERE CreatedBy = A.CreatedBy) IS NULL 
                            OR (SELECT MIN(CAST(CreatedDate AS DATE)) FROM Submissions WHERE CreatedBy = A.CreatedBy) > @YearStart 
                       THEN ISNULL((SELECT MIN(CAST(CreatedDate AS DATE)) FROM Submissions WHERE CreatedBy = A.CreatedBy), @YearStart)
                       ELSE @YearStart
                   END as UserYearStart
            FROM dbo.ActiveUsersView A
            WHERE Role = 4 AND Status = 1
        ),
        BaseMetrics AS (
            -- Replicate #BaseMetrics temp table logic for all users
            SELECT 
                U.UserName as [User], U.UserYearStart,
                -- Submission counts
                ISNULL(SUM(CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDate7D AND @Today THEN 1 ELSE 0 END), 0) as SUB_LAST7D_COUNT,
                ISNULL(SUM(CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today THEN 1 ELSE 0 END), 0) as SUB_MTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today THEN 1 ELSE 0 END), 0) as SUB_QTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today THEN 1 ELSE 0 END), 0) as SUB_HYTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN U.UserYearStart AND @Today THEN 1 ELSE 0 END), 0) as SUB_YTD_COUNT,
                -- Active requisition counts
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDate7D AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN S.RequisitionId END), 0) as REQ_LAST7D_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN S.RequisitionId END), 0) as REQ_MTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN S.RequisitionId END), 0) as REQ_QTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN S.RequisitionId END), 0) as REQ_HYTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN U.UserYearStart AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN S.RequisitionId END), 0) as REQ_YTD_COUNT
            FROM 
                AllRecruiters U LEFT JOIN Submissions S ON S.CreatedBy = U.UserName
                LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
            GROUP BY 
                U.UserName, U.UserYearStart
        ),
        StatusMetrics AS (
            -- Replicate #StatusMetrics logic for all users
            SELECT 
                U.UserName as [User], U.UserYearStart,
                -- Interview status counts
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND RS.Status = 'INT' THEN 1 ELSE 0 END), 0) as INT_LAST7D_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND RS.Status = 'INT' THEN 1 ELSE 0 END), 0) as INT_MTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND RS.Status = 'INT' THEN 1 ELSE 0 END), 0) as INT_QTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND RS.Status = 'INT' THEN 1 ELSE 0 END), 0) as INT_HYTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN U.UserYearStart AND @Today AND RS.Status = 'INT' THEN 1 ELSE 0 END), 0) as INT_YTD_COUNT,
                -- Offer extended counts
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND RS.Status = 'OEX' THEN 1 ELSE 0 END), 0) as OEX_LAST7D_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND RS.Status = 'OEX' THEN 1 ELSE 0 END), 0) as OEX_MTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND RS.Status = 'OEX' THEN 1 ELSE 0 END), 0) as OEX_QTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND RS.Status = 'OEX' THEN 1 ELSE 0 END), 0) as OEX_HYTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN U.UserYearStart AND @Today AND RS.Status = 'OEX' THEN 1 ELSE 0 END), 0) as OEX_YTD_COUNT,
                -- Hired counts
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND RS.Status = 'HIR' THEN 1 ELSE 0 END), 0) as HIR_LAST7D_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND RS.Status = 'HIR' THEN 1 ELSE 0 END), 0) as HIR_MTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND RS.Status = 'HIR' THEN 1 ELSE 0 END), 0) as HIR_QTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND RS.Status = 'HIR' THEN 1 ELSE 0 END), 0) as HIR_HYTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN U.UserYearStart AND @Today AND RS.Status = 'HIR' THEN 1 ELSE 0 END), 0) as HIR_YTD_COUNT
            FROM AllRecruiters U
            LEFT JOIN (
                -- Replicate #RecruiterSubmissions logic for all users
                SELECT 
                    FS.RequisitionId, FS.CandidateId, FS.UserName, S.Status, MAX(S.CreatedDate) as LatestStatusDate
                FROM (
                    SELECT 
                        S.RequisitionId, S.CandidateId, U2.UserName, MIN(S.CreatedDate) as FirstSubmissionDate
                    FROM 
                        Submissions S CROSS JOIN AllRecruiters U2
                    WHERE 
                        S.CreatedBy = U2.UserName
                    GROUP BY 
                        S.RequisitionId, S.CandidateId, U2.UserName
                    HAVING 
                        MIN(CASE WHEN S.CreatedBy = U2.UserName THEN S.CreatedDate END) = MIN(S.CreatedDate)
                ) FS
                INNER JOIN Submissions S  ON FS.RequisitionId = S.RequisitionId AND FS.CandidateId = S.CandidateId AND S.Status IN ('INT', 'OEX', 'HIR')
                GROUP BY 
                    FS.RequisitionId, FS.CandidateId, FS.UserName, S.Status
            ) RS ON RS.UserName = U.UserName
            GROUP BY 
                U.UserName, U.UserYearStart
        )
        
        -- Generate all period rows for all users
         INSERT INTO 
            [dbo].[SubmissionMetricsView] 
            ([CreatedBy], [MetricDate], [Period], [INT_Count], [OEX_Count], [HIR_Count], [Total_Submissions], [OEX_HIR_Ratio], [Requisitions_Created], 
            [Active_Requisitions_Updated], [RefreshDate])
       SELECT 
            BM.[User] as CreatedBy, @Today as MetricDate, P.Period,
            CASE P.Period
                WHEN '7D' THEN SM.INT_LAST7D_COUNT
                WHEN 'MTD' THEN SM.INT_MTD_COUNT
                WHEN 'QTD' THEN SM.INT_QTD_COUNT
                WHEN 'HYTD' THEN SM.INT_HYTD_COUNT
                WHEN 'YTD' THEN SM.INT_YTD_COUNT
            END as INT_Count,
            CASE P.Period
                WHEN '7D' THEN SM.OEX_LAST7D_COUNT
                WHEN 'MTD' THEN SM.OEX_MTD_COUNT
                WHEN 'QTD' THEN SM.OEX_QTD_COUNT
                WHEN 'HYTD' THEN SM.OEX_HYTD_COUNT
                WHEN 'YTD' THEN SM.OEX_YTD_COUNT
            END as OEX_Count,
            CASE P.Period
                WHEN '7D' THEN SM.HIR_LAST7D_COUNT
                WHEN 'MTD' THEN SM.HIR_MTD_COUNT
                WHEN 'QTD' THEN SM.HIR_QTD_COUNT
                WHEN 'HYTD' THEN SM.HIR_HYTD_COUNT
                WHEN 'YTD' THEN SM.HIR_YTD_COUNT
            END as HIR_Count,
            CASE P.Period
                WHEN '7D' THEN BM.SUB_LAST7D_COUNT
                WHEN 'MTD' THEN BM.SUB_MTD_COUNT
                WHEN 'QTD' THEN BM.SUB_QTD_COUNT
                WHEN 'HYTD' THEN BM.SUB_HYTD_COUNT
                WHEN 'YTD' THEN BM.SUB_YTD_COUNT
            END as Total_Submissions,
            CASE 
                WHEN CASE P.Period
                    WHEN '7D' THEN SM.OEX_LAST7D_COUNT
                    WHEN 'MTD' THEN SM.OEX_MTD_COUNT
                    WHEN 'QTD' THEN SM.OEX_QTD_COUNT
                    WHEN 'HYTD' THEN SM.OEX_HYTD_COUNT
                    WHEN 'YTD' THEN SM.OEX_YTD_COUNT
                END = 0 THEN 0.00
                ELSE CAST(
                    CASE P.Period
                        WHEN '7D' THEN SM.HIR_LAST7D_COUNT
                        WHEN 'MTD' THEN SM.HIR_MTD_COUNT
                        WHEN 'QTD' THEN SM.HIR_QTD_COUNT
                        WHEN 'HYTD' THEN SM.HIR_HYTD_COUNT
                        WHEN 'YTD' THEN SM.HIR_YTD_COUNT
                    END AS DECIMAL(5,2)
                ) / CAST(
                    CASE P.Period
                        WHEN '7D' THEN SM.OEX_LAST7D_COUNT
                        WHEN 'MTD' THEN SM.OEX_MTD_COUNT
                        WHEN 'QTD' THEN SM.OEX_QTD_COUNT
                        WHEN 'HYTD' THEN SM.OEX_HYTD_COUNT
                        WHEN 'YTD' THEN SM.OEX_YTD_COUNT
                    END AS DECIMAL(5,2)
                )
            END as OEX_HIR_Ratio,
            0 as Requisitions_Created,
            CASE P.Period
                WHEN '7D' THEN BM.REQ_LAST7D_COUNT
                WHEN 'MTD' THEN BM.REQ_MTD_COUNT
                WHEN 'QTD' THEN BM.REQ_QTD_COUNT
                WHEN 'HYTD' THEN BM.REQ_HYTD_COUNT
                WHEN 'YTD' THEN BM.REQ_YTD_COUNT
            END as Active_Requisitions_Updated,
            GETDATE() as RefreshDate
        FROM 
            BaseMetrics BM CROSS JOIN (VALUES ('7D'), ('MTD'), ('QTD'), ('HYTD'), ('YTD')) P(Period)
            LEFT JOIN StatusMetrics SM ON BM.[User] = SM.[User];
        
        -- Log success
        DECLARE @Duration_MS INT = DATEDIFF(MILLISECOND, @RefreshStart, SYSDATETIME());
        DECLARE @RowCount INT = @@ROWCOUNT;
        PRINT 'Recruiter SubmissionMetricsView refreshed successfully - ' + CAST(@RowCount AS VARCHAR(10)) + ' rows in ' + CAST(@Duration_MS AS VARCHAR(10)) + 'ms';
        
    END TRY
    BEGIN CATCH
        -- Log error details
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        PRINT 'Error refreshing Recruiter SubmissionMetricsView: ' + @ErrorMessage;
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;

GO

ALTER   PROCEDURE [dbo].[Dashboard_Refactor_ActiveUsers]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
    DECLARE @RefreshStart DATETIME2 = SYSDATETIME();
    
    BEGIN TRY
        -- Clear existing data
        DELETE FROM [dbo].[ActiveUsersView];
        
        -- Populate ActiveUsersView with all active users who have created requisitions
        INSERT INTO 
            [dbo].[ActiveUsersView] 
            ([CreatedBy], [FullName], [Role], [Status], [LastRequisitionDate], [RequisitionCount], [RefreshDate])
        SELECT 
            U.UserName as CreatedBy, LTRIM(RTRIM(U.FirstName + ' ' + U.LastName)) as FullName, U.Role, U.Status, ISNULL(MAX(R.CreatedDate), '2000-01-01') as LastRequisitionDate,
            COUNT(DISTINCT R.Id) as RequisitionCount, GETDATE() as RefreshDate
        FROM 
            dbo.Users U LEFT JOIN dbo.Requisitions R ON U.UserName = R.CreatedBy AND R.CreatedDate >= DATEADD(YEAR, -1, GETDATE()) -- Only last year's activity
        WHERE 
            U.Status = 1 AND U.UserName IS NOT NULL
        GROUP BY 
            U.UserName, U.FirstName, U.LastName, U.Role, U.Status;
        
        -- Log success
        DECLARE @Duration_MS INT = DATEDIFF(MILLISECOND, @RefreshStart, SYSDATETIME());
        PRINT 'ActiveUsersView refreshed successfully in ' + CAST(@Duration_MS AS VARCHAR(10)) + 'ms';
        
    END TRY
    BEGIN CATCH
        -- Log error details
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        PRINT 'Error refreshing ActiveUsersView: ' + @ErrorMessage;
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;

GO

CREATE OR ALTER PROCEDURE [dbo].[Dashboard_Refactor_CandidateQualityMetrics]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
    DECLARE @RefreshStart DATETIME2 = SYSDATETIME();
    DECLARE @Today DATE = CAST(GETDATE() AS DATE);
    
    BEGIN TRY
        -- Clear existing data
        DELETE FROM [dbo].[CandidateQualityMetricsView];
        
        -- Date calculations
        DECLARE @YearStart DATE = DATEFROMPARTS(YEAR(@Today), 1, 1);
        DECLARE @StartDate7D DATE = DATEADD(DAY, -6, @Today);
        DECLARE @StartDateMTD DATE = DATEADD(DAY, 1, EOMONTH(@Today, -1));
        DECLARE @StartDateQTD DATE = DATEADD(QUARTER, DATEDIFF(QUARTER, 0, @Today), 0);
        DECLARE @StartDateHYTD DATE = CASE 
            WHEN MONTH(@Today) <= 6 THEN DATEFROMPARTS(YEAR(@Today), 1, 1)
            ELSE DATEFROMPARTS(YEAR(@Today), 7, 1)
        END;
        
        -- Get latest submission status per candidate+requisition combo
        WITH LatestSubmissions AS (
            SELECT 
                S.CreatedBy,
                S.RequisitionId,
                S.CandidateId,
                S.Status,
                CAST(S.CreatedDate AS DATE) as SubmissionDate,
                ROW_NUMBER() OVER (
                    PARTITION BY S.CreatedBy, S.RequisitionId, S.CandidateId 
                    ORDER BY S.CreatedDate DESC
                ) as rn
            FROM dbo.Submissions S
            INNER JOIN dbo.ActiveUsersView A ON S.CreatedBy = A.CreatedBy
            WHERE A.Status = 1
        ),
        CurrentStatus AS (
            SELECT 
                CreatedBy,
                RequisitionId,
                CandidateId,
                Status,
                SubmissionDate
            FROM LatestSubmissions 
            WHERE rn = 1
        ),
        QualityMetrics AS (
            SELECT 
                U.CreatedBy,
                -- 7D metrics
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDate7D AND @Today 
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as Total_7D,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDate7D AND @Today 
                    AND CS.Status IN ('INT', 'RHM', 'DEC', 'NOA', 'OEX', 'ODC', 'HIR', 'WDR')
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as INT_7D,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDate7D AND @Today 
                    AND CS.Status IN ('OEX', 'ODC', 'HIR', 'WDR')
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as OEX_7D,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDate7D AND @Today 
                    AND CS.Status = 'HIR'
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as HIR_7D,
                
                -- MTD metrics
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateMTD AND @Today 
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as Total_MTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateMTD AND @Today 
                    AND CS.Status IN ('INT', 'RHM', 'DEC', 'NOA', 'OEX', 'ODC', 'HIR', 'WDR')
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as INT_MTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateMTD AND @Today 
                    AND CS.Status IN ('OEX', 'ODC', 'HIR', 'WDR')
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as OEX_MTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateMTD AND @Today 
                    AND CS.Status = 'HIR'
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as HIR_MTD,
                
                -- QTD metrics
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateQTD AND @Today 
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as Total_QTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateQTD AND @Today 
                    AND CS.Status IN ('INT', 'RHM', 'DEC', 'NOA', 'OEX', 'ODC', 'HIR', 'WDR')
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as INT_QTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateQTD AND @Today 
                    AND CS.Status IN ('OEX', 'ODC', 'HIR', 'WDR')
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as OEX_QTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateQTD AND @Today 
                    AND CS.Status = 'HIR'
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as HIR_QTD,
                
                -- HYTD metrics
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateHYTD AND @Today 
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as Total_HYTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateHYTD AND @Today 
                    AND CS.Status IN ('INT', 'RHM', 'DEC', 'NOA', 'OEX', 'ODC', 'HIR', 'WDR')
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as INT_HYTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateHYTD AND @Today 
                    AND CS.Status IN ('OEX', 'ODC', 'HIR', 'WDR')
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as OEX_HYTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateHYTD AND @Today 
                    AND CS.Status = 'HIR'
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as HIR_HYTD,
                
                -- YTD metrics
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @YearStart AND @Today 
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as Total_YTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @YearStart AND @Today 
                    AND CS.Status IN ('INT', 'RHM', 'DEC', 'NOA', 'OEX', 'ODC', 'HIR', 'WDR')
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as INT_YTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @YearStart AND @Today 
                    AND CS.Status IN ('OEX', 'ODC', 'HIR', 'WDR')
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as OEX_YTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @YearStart AND @Today 
                    AND CS.Status = 'HIR'
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as HIR_YTD
            FROM dbo.ActiveUsersView U
            LEFT JOIN CurrentStatus CS ON U.CreatedBy = CS.CreatedBy
            WHERE U.Status = 1
            GROUP BY U.CreatedBy
        )
        
        -- Insert calculated metrics for all periods
        INSERT INTO [dbo].[CandidateQualityMetricsView]
        (CreatedBy, MetricDate, Period, Total_Submissions, Reached_INT, Reached_OEX, Reached_HIR, 
         PEN_to_INT_Ratio, INT_to_OEX_Ratio, OEX_to_HIR_Ratio, RefreshDate)
        SELECT 
            CreatedBy, @Today, Period, Total_Submissions, Reached_INT, Reached_OEX, Reached_HIR,
            CASE WHEN Total_Submissions > 0 THEN CAST((Reached_INT * 100.0 / Total_Submissions) AS DECIMAL(5,2)) ELSE 0.00 END,
            CASE WHEN Reached_INT > 0 THEN CAST((Reached_OEX * 100.0 / Reached_INT) AS DECIMAL(5,2)) ELSE 0.00 END,
            CASE WHEN Reached_OEX > 0 THEN CAST((Reached_HIR * 100.0 / Reached_OEX) AS DECIMAL(5,2)) ELSE 0.00 END,
            GETDATE()
        FROM (
            SELECT CreatedBy, '7D' as Period, Total_7D as Total_Submissions, INT_7D as Reached_INT, OEX_7D as Reached_OEX, HIR_7D as Reached_HIR FROM QualityMetrics
            UNION ALL
            SELECT CreatedBy, 'MTD' as Period, Total_MTD, INT_MTD, OEX_MTD, HIR_MTD FROM QualityMetrics
            UNION ALL
            SELECT CreatedBy, 'QTD' as Period, Total_QTD, INT_QTD, OEX_QTD, HIR_QTD FROM QualityMetrics
            UNION ALL
            SELECT CreatedBy, 'HYTD' as Period, Total_HYTD, INT_HYTD, OEX_HYTD, HIR_HYTD FROM QualityMetrics
            UNION ALL
            SELECT CreatedBy, 'YTD' as Period, Total_YTD, INT_YTD, OEX_YTD, HIR_YTD FROM QualityMetrics
        ) AllPeriods;
        
        -- Log success
        DECLARE @Duration_MS INT = DATEDIFF(MILLISECOND, @RefreshStart, SYSDATETIME());
        PRINT 'CandidateQualityMetricsView refreshed successfully in ' + CAST(@Duration_MS AS VARCHAR(10)) + 'ms';
        
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        PRINT 'Error refreshing CandidateQualityMetricsView: ' + @ErrorMessage;
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO