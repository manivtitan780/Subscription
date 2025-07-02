Use [Subscription]

ALTER PROCEDURE [dbo].[DashboardAccountsManager]
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
    
    -- Common date variables
    DECLARE @Today DATE = GETDATE();
    DECLARE @YearStart DATE = DATEADD(YEAR, DATEDIFF(YEAR, 0, @Today), 0);
    
    -- Common date calculations
    DECLARE @StartDate7D DATE = DATEADD(DAY, -7, @Today);
    DECLARE @StartDateMTD DATE = DATEADD(MONTH, DATEDIFF(MONTH, 0, @Today), 0);
    DECLARE @StartDateQTD DATE = DATEADD(QUARTER, DATEDIFF(QUARTER, 0, @Today), 0);
    DECLARE @StartDateHYTD DATE = CASE 
        WHEN MONTH(@Today) <= 6 THEN DATEFROMPARTS(YEAR(@Today), 1, 1)
        ELSE DATEFROMPARTS(YEAR(@Today), 7, 1)
    END;
    
    -- Generic Query for Dropdown
    SELECT UserName as KeyValue, FirstName + ' ' + LastName as Text
    FROM dbo.Users 
    WHERE UserName = @User
    FOR JSON AUTO;
    
    /***************************************************************************
    * RESULT SET 1: TIME-BOUND METRICS (Conditional Aggregation)
    * Returns 6 different queries with time period analysis
    ***************************************************************************/
    
    -- Create temp table for requisition metrics
    DROP TABLE IF EXISTS #RequisitionMetrics;
    
    SELECT 
        @User as [User],
        -- Total requisitions created
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
    INTO #RequisitionMetrics
    FROM (SELECT 1 as dummy) d -- Ensures we always have at least one row
    LEFT JOIN RequisitionView R ON R.CreatedBy = @User;
    
    -- Create temp table for submission status metrics
    DROP TABLE IF EXISTS #SubmissionMetrics;
    
    WITH StatusSubmissions AS (
        SELECT 
            S.RequisitionId,
            S.CandidateId,
            S.Status,
            MAX(S.CreatedDate) as LatestStatusDate
        FROM Submissions S
        INNER JOIN Requisitions R ON R.Id = S.RequisitionId AND R.CreatedBy = @User
        WHERE S.Status IN ('INT', 'OEX', 'HIR')
        GROUP BY S.RequisitionId, S.CandidateId, S.Status
    )
    SELECT 
        @User as [User],
        -- Interview status counts
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
    INTO #SubmissionMetrics
    FROM (SELECT 1 as dummy) d -- Ensures we always have at least one row
    LEFT JOIN StatusSubmissions SS ON 1=1;
    
    /***************************************************************************
    * Query 1: Total Requisitions Created
    ***************************************************************************/
    SELECT [User], TOTAL_LAST7D_COUNT as LAST7D_COUNT, TOTAL_MTD_COUNT as MTD_COUNT, TOTAL_QTD_COUNT as QTD_COUNT, TOTAL_HYTD_COUNT as HYTD_COUNT, TOTAL_YTD_COUNT as YTD_COUNT
    FROM #RequisitionMetrics FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;
    
    /***************************************************************************
    * Query 2: Active Requisitions (OPN, NEW, PAR)
    ***************************************************************************/
    SELECT [User], ACTIVE_LAST7D_COUNT as LAST7D_COUNT, ACTIVE_MTD_COUNT as MTD_COUNT, ACTIVE_QTD_COUNT as QTD_COUNT, ACTIVE_HYTD_COUNT as HYTD_COUNT, ACTIVE_YTD_COUNT as YTD_COUNT
    FROM #RequisitionMetrics FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;
    
    /***************************************************************************
    * Query 3: Candidates in Interview (INT)
    ***************************************************************************/
    SELECT [User], INT_LAST7D_COUNT as LAST7D_COUNT, INT_MTD_COUNT as MTD_COUNT, INT_QTD_COUNT as QTD_COUNT, INT_HYTD_COUNT as HYTD_COUNT, INT_YTD_COUNT as YTD_COUNT
    FROM #SubmissionMetrics FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;
    
    /***************************************************************************
    * Query 4: Offers Extended (OEX)
    ***************************************************************************/
    SELECT [User], OEX_LAST7D_COUNT as LAST7D_COUNT, OEX_MTD_COUNT as MTD_COUNT, OEX_QTD_COUNT as QTD_COUNT, OEX_HYTD_COUNT as HYTD_COUNT, OEX_YTD_COUNT as YTD_COUNT
    FROM #SubmissionMetrics FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;
    
    /***************************************************************************
    * Query 5: Candidates Hired (HIR)
    ***************************************************************************/
    SELECT [User], HIR_LAST7D_COUNT as LAST7D_COUNT, HIR_MTD_COUNT as MTD_COUNT, HIR_QTD_COUNT as QTD_COUNT, HIR_HYTD_COUNT as HYTD_COUNT, HIR_YTD_COUNT as YTD_COUNT
    FROM #SubmissionMetrics FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;
    
    /***************************************************************************
    * Query 6: Hire-to-Offer Ratio (HIR/OEX)
    ***************************************************************************/
    SELECT 
        [User],
        CASE WHEN OEX_LAST7D_COUNT = 0 THEN 0.00 ELSE CAST(HIR_LAST7D_COUNT AS DECIMAL(10,2)) / CAST(OEX_LAST7D_COUNT AS DECIMAL(10,2)) END as LAST7D_RATIO,
        CASE WHEN OEX_MTD_COUNT = 0 THEN 0.00 ELSE CAST(HIR_MTD_COUNT AS DECIMAL(10,2)) / CAST(OEX_MTD_COUNT AS DECIMAL(10,2)) END as MTD_RATIO,
        CASE WHEN OEX_QTD_COUNT = 0 THEN 0.00 ELSE CAST(HIR_QTD_COUNT AS DECIMAL(10,2)) / CAST(OEX_QTD_COUNT AS DECIMAL(10,2)) END as QTD_RATIO,
        CASE WHEN OEX_HYTD_COUNT = 0 THEN 0.00 ELSE CAST(HIR_HYTD_COUNT AS DECIMAL(10,2)) / CAST(OEX_HYTD_COUNT AS DECIMAL(10,2)) END as HYTD_RATIO,
        CASE WHEN OEX_YTD_COUNT = 0 THEN 0.00 ELSE CAST(HIR_YTD_COUNT AS DECIMAL(10,2)) / CAST(OEX_YTD_COUNT AS DECIMAL(10,2)) END as YTD_RATIO
    FROM #SubmissionMetrics FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;
    
    /***************************************************************************
    * RESULT SET 2: RECENT ACTIVITY REPORT
    * Query 7: 30-Day Submission Activity
    ***************************************************************************/
    DECLARE @StartDate30 DATE = CASE 
        WHEN DATEADD(DAY, -30, @Today) < @YearStart THEN @YearStart
        ELSE DATEADD(DAY, -30, @Today)
    END;
    DECLARE @return varchar(max);

    WITH CompanySubmissionCounts AS (
        -- Get submission counts by company for sorting
        SELECT 
            C.CompanyName,
            COUNT(DISTINCT CAST(S.RequisitionId AS VARCHAR(10)) + '-' + CAST(S.CandidateId AS VARCHAR(10))) as SubmissionCount
        FROM Submissions S
        INNER JOIN Requisitions R ON R.Id = S.RequisitionId AND R.CreatedBy = @User
        INNER JOIN Companies C ON C.ID = R.CompanyId
        WHERE S.CreatedDate >= @StartDate30 AND S.CreatedDate <= DATEADD(DAY, 1, @Today)
        GROUP BY C.CompanyName
    ),
    SubmissionSummary AS (
        -- Get first and last activity for each Req+Cand combination
        SELECT 
            S.RequisitionId,
            S.CandidateId,
            MIN(S.CreatedDate) as DateFirstSubmitted,
            MAX(S.CreatedDate) as LastActivityDate
        FROM Submissions S
        INNER JOIN Requisitions R ON R.Id = S.RequisitionId AND R.CreatedBy = @User
        GROUP BY S.RequisitionId, S.CandidateId
        HAVING MIN(S.CreatedDate) >= @StartDate30 AND MIN(S.CreatedDate) <= DATEADD(DAY, 1, @Today)
    ),
    LastActivity AS (
        -- Get the status and notes from the most recent submission
        SELECT 
            S.RequisitionId,
            S.CandidateId,
            S.Status,
            S.Notes,
            S.CreatedDate,
            ROW_NUMBER() OVER (PARTITION BY S.RequisitionId, S.CandidateId ORDER BY S.CreatedDate DESC) as RN
        FROM Submissions S
        INNER JOIN SubmissionSummary SS 
            ON SS.RequisitionId = S.RequisitionId 
            AND SS.CandidateId = S.CandidateId
    )

    SELECT @return =
    (SELECT 
        C.CompanyName + ' - [' + R.Code + '] - ' + CAST(R.Positions as varchar(5)) + ' Positions - ' + R.PosTitle as Company,
        LTRIM(RTRIM(ISNULL(CAND.FirstName, '') + ' ' + ISNULL(CAND.LastName, ''))) as [CandidateName],
        LA.Status as [CurrentStatus],
        CAST(SS.DateFirstSubmitted AS DATE) as [DateFirstSubmitted],
        CAST(SS.LastActivityDate AS DATE) as [LastActivityDate],
        ISNULL(LA.Notes, '') as [ActivityNotes],
        @User [User]
    FROM SubmissionSummary SS
    INNER JOIN LastActivity LA ON LA.RequisitionId = SS.RequisitionId AND LA.CandidateId = SS.CandidateId AND LA.RN = 1
    INNER JOIN Requisitions R ON R.Id = SS.RequisitionId
    INNER JOIN Companies C ON C.ID = R.CompanyId
    INNER JOIN Candidate CAND ON CAND.ID = SS.CandidateId
    INNER JOIN CompanySubmissionCounts CSC ON CSC.CompanyName = C.CompanyName
    ORDER BY 
        CSC.SubmissionCount DESC,  -- Companies with more submissions first
        C.CompanyName ASC,         -- Alphabetical for ties
        R.Id ASC,                  -- RequisitionID within company
        SS.DateFirstSubmitted ASC
    FOR JSON PATH); -- Date First Submitted within requisition
    
    SELECT @return;

    /***************************************************************************
    * RESULT SET 3: PLACEMENT REPORT
    * Query 8: 3-Month Hire Report
    ***************************************************************************/
    DECLARE @StartDate90 DATE = CASE 
        WHEN DATEADD(MONTH, -3, @Today) < @YearStart THEN @YearStart
        ELSE DATEADD(MONTH, -3, @Today)
    END;

    WITH HiredCandidates AS (
        -- Get the latest HIR record for each Req+Cand combination
        SELECT 
            S.RequisitionId,
            S.CandidateId,
            MAX(S.CreatedDate) as DateHired,
            ROW_NUMBER() OVER (PARTITION BY S.RequisitionId, S.CandidateId ORDER BY MAX(S.CreatedDate) DESC) as RN
        FROM Submissions S
        INNER JOIN Requisitions R ON R.Id = S.RequisitionId AND R.CreatedBy = @User
        WHERE S.Status = 'HIR'
        GROUP BY S.RequisitionId, S.CandidateId
        HAVING MAX(S.CreatedDate) >= @StartDate90 AND MAX(S.CreatedDate) <= DATEADD(DAY, 1, @Today)
    )

    SELECT @return = 
    (SELECT 
        C.CompanyName as Company,
        R.Code as [RequisitionNumber],
        R.Positions as [NumPosition],
        R.PosTitle as Title,
        LTRIM(RTRIM(ISNULL(CAND.FirstName, '') + ' ' + ISNULL(CAND.LastName, ''))) as [CandidateName],
        CAST(HC.DateHired AS DATE) as [DateHired],
        CAST(0.00 AS DECIMAL(9,2)) as [SalaryOffered],
        CAST(GETDATE() AS DATE) as [StartDate],
        CAST(GETDATE() AS DATE) as [DateInvoiced],
        CAST(GETDATE() AS DATE) as [DatePaid],
        CAST(0.00 AS DECIMAL(9,2)) as [PlacementFee],
        CAST(0.00 AS DECIMAL(5,2)) as [CommissionPercent],
        CAST(0.00 AS DECIMAL(9,2)) as [CommissionEarned],
        @User [User]
    FROM HiredCandidates HC
    INNER JOIN Requisitions R ON R.Id = HC.RequisitionId
    INNER JOIN Companies C ON C.ID = R.CompanyId
    INNER JOIN Candidate CAND ON CAND.ID = HC.CandidateId
    WHERE HC.RN = 1
    ORDER BY 
        C.CompanyName ASC,
        R.Id ASC,
        HC.DateHired DESC
    FOR JSON PATH);
    
    SELECT @return;
    
    /***************************************************************************
    * RESULT SET 4: TIMING ANALYTICS
    * Query 1: Time to Fill, Time to Hire, and Time in Stage metrics (By Requisition)
    * Query 2: Time to Fill, Time to Hire, and Time in Stage metrics (By Company)
    ***************************************************************************/
    
    -- Create temp table for timing analytics
    DROP TABLE IF EXISTS #TimingAnalytics;
    
    -- Get requisitions from last year
    DECLARE @StartDate365 DATE = DATEADD(YEAR, -1, @Today);
    
    WITH RequisitionBase AS (
        -- Get requisitions created by user in last year
        SELECT 
            R.Id as RequisitionId,
            R.Code as RequisitionCode,
            C.CompanyName,
            R.PosTitle as Title,
            R.CreatedDate as RequisitionCreatedDate,
            R.CreatedBy,
            -- Time to Fill calculation (only for FUL status)
            CASE 
                WHEN R.Status = 'FUL' THEN DATEDIFF(DAY, R.CreatedDate, R.UpdatedDate)
                ELSE NULL 
            END as TimeToFill
        FROM Requisitions R
        INNER JOIN Companies C ON R.CompanyId = C.ID
        WHERE R.CreatedBy = @User 
            AND CAST(R.CreatedDate AS DATE) >= @StartDate365
    ),
    FirstSubmissions AS (
        -- Get first submission date for each candidate+requisition combo
        SELECT 
            S.RequisitionId,
            S.CandidateId,
            MIN(S.CreatedDate) as FirstSubmissionDate
        FROM Submissions S
        INNER JOIN RequisitionBase RB ON S.RequisitionId = RB.RequisitionId
        GROUP BY S.RequisitionId, S.CandidateId
    ),
    TimeToHireCalc AS (
        -- Calculate Time to Hire for hired candidates
        SELECT 
            FS.RequisitionId,
            FS.CandidateId,
            FS.FirstSubmissionDate,
            MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HireDate,
            MAX(CASE WHEN S.Status = 'HIR' THEN DATEDIFF(DAY, FS.FirstSubmissionDate, S.CreatedDate) END) as TimeToHire
        FROM FirstSubmissions FS
        INNER JOIN Submissions S ON FS.RequisitionId = S.RequisitionId AND FS.CandidateId = S.CandidateId
        GROUP BY FS.RequisitionId, FS.CandidateId, FS.FirstSubmissionDate
    ),
    StageTimings AS (
        -- Calculate time in each stage
        SELECT 
            FS.RequisitionId,
            FS.CandidateId,
            -- Get the latest date for each stage (in case of multiple records for same stage)
            MAX(CASE WHEN S.Status = 'PEN' THEN S.CreatedDate END) as PEN_Date,
            MAX(CASE WHEN S.Status = 'REJ' THEN S.CreatedDate END) as REJ_Date,
            MAX(CASE WHEN S.Status = 'HLD' THEN S.CreatedDate END) as HLD_Date,
            MAX(CASE WHEN S.Status = 'PHN' THEN S.CreatedDate END) as PHN_Date,
            MAX(CASE WHEN S.Status = 'URW' THEN S.CreatedDate END) as URW_Date,
            MAX(CASE WHEN S.Status = 'INT' THEN S.CreatedDate END) as INT_Date,
            MAX(CASE WHEN S.Status = 'RHM' THEN S.CreatedDate END) as RHM_Date,
            MAX(CASE WHEN S.Status = 'DEC' THEN S.CreatedDate END) as DEC_Date,
            MAX(CASE WHEN S.Status = 'NOA' THEN S.CreatedDate END) as NOA_Date,
            MAX(CASE WHEN S.Status = 'OEX' THEN S.CreatedDate END) as OEX_Date,
            MAX(CASE WHEN S.Status = 'ODC' THEN S.CreatedDate END) as ODC_Date,
            MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HIR_Date,
            MAX(CASE WHEN S.Status = 'WDR' THEN S.CreatedDate END) as WDR_Date
        FROM FirstSubmissions FS
        INNER JOIN Submissions S ON FS.RequisitionId = S.RequisitionId AND FS.CandidateId = S.CandidateId
        GROUP BY FS.RequisitionId, FS.CandidateId
    ),
    StageTimeCalculations AS (
        -- Calculate days spent in each stage
        SELECT 
            ST.RequisitionId,
            ST.CandidateId,
            FS.FirstSubmissionDate,
            -- Calculate time in each stage (from current stage to next stage or end)
            CASE 
                WHEN ST.PEN_Date IS NULL THEN 0
                WHEN ST.REJ_Date IS NOT NULL AND ST.PEN_Date <= ST.REJ_Date THEN DATEDIFF(DAY, ST.PEN_Date, ST.REJ_Date)
                WHEN ST.HLD_Date IS NOT NULL AND ST.PEN_Date <= ST.HLD_Date THEN DATEDIFF(DAY, ST.PEN_Date, ST.HLD_Date)
                WHEN ST.PHN_Date IS NOT NULL AND ST.PEN_Date <= ST.PHN_Date THEN DATEDIFF(DAY, ST.PEN_Date, ST.PHN_Date)
                ELSE DATEDIFF(DAY, ST.PEN_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as PEN_Days,
            
            CASE 
                WHEN ST.REJ_Date IS NULL THEN 0
                ELSE 0 -- REJ is final stage
            END as REJ_Days,
            
            CASE 
                WHEN ST.HLD_Date IS NULL THEN 0
                WHEN ST.PHN_Date IS NOT NULL AND ST.HLD_Date <= ST.PHN_Date THEN DATEDIFF(DAY, ST.HLD_Date, ST.PHN_Date)
                WHEN ST.URW_Date IS NOT NULL AND ST.HLD_Date <= ST.URW_Date THEN DATEDIFF(DAY, ST.HLD_Date, ST.URW_Date)
                ELSE DATEDIFF(DAY, ST.HLD_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as HLD_Days,
            
            CASE 
                WHEN ST.PHN_Date IS NULL THEN 0
                WHEN ST.URW_Date IS NOT NULL AND ST.PHN_Date <= ST.URW_Date THEN DATEDIFF(DAY, ST.PHN_Date, ST.URW_Date)
                WHEN ST.INT_Date IS NOT NULL AND ST.PHN_Date <= ST.INT_Date THEN DATEDIFF(DAY, ST.PHN_Date, ST.INT_Date)
                ELSE DATEDIFF(DAY, ST.PHN_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as PHN_Days,
            
            CASE 
                WHEN ST.URW_Date IS NULL THEN 0
                WHEN ST.INT_Date IS NOT NULL AND ST.URW_Date <= ST.INT_Date THEN DATEDIFF(DAY, ST.URW_Date, ST.INT_Date)
                WHEN ST.DEC_Date IS NOT NULL AND ST.URW_Date <= ST.DEC_Date THEN DATEDIFF(DAY, ST.URW_Date, ST.DEC_Date)
                ELSE DATEDIFF(DAY, ST.URW_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as URW_Days,
            
            CASE 
                WHEN ST.INT_Date IS NULL THEN 0
                WHEN ST.DEC_Date IS NOT NULL AND ST.INT_Date <= ST.DEC_Date THEN DATEDIFF(DAY, ST.INT_Date, ST.DEC_Date)
                WHEN ST.NOA_Date IS NOT NULL AND ST.INT_Date <= ST.NOA_Date THEN DATEDIFF(DAY, ST.INT_Date, ST.NOA_Date)
                ELSE DATEDIFF(DAY, ST.INT_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as INT_Days,
            
            CASE 
                WHEN ST.RHM_Date IS NULL THEN 0
                ELSE 0 -- RHM is final stage
            END as RHM_Days,
            
            CASE 
                WHEN ST.DEC_Date IS NULL THEN 0
                WHEN ST.NOA_Date IS NOT NULL AND ST.DEC_Date <= ST.NOA_Date THEN DATEDIFF(DAY, ST.DEC_Date, ST.NOA_Date)
                WHEN ST.OEX_Date IS NOT NULL AND ST.DEC_Date <= ST.OEX_Date THEN DATEDIFF(DAY, ST.DEC_Date, ST.OEX_Date)
                ELSE DATEDIFF(DAY, ST.DEC_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as DEC_Days,
            
            CASE 
                WHEN ST.NOA_Date IS NULL THEN 0
                WHEN ST.OEX_Date IS NOT NULL AND ST.NOA_Date <= ST.OEX_Date THEN DATEDIFF(DAY, ST.NOA_Date, ST.OEX_Date)
                ELSE DATEDIFF(DAY, ST.NOA_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as NOA_Days,
            
            CASE 
                WHEN ST.OEX_Date IS NULL THEN 0
                WHEN ST.HIR_Date IS NOT NULL AND ST.OEX_Date <= ST.HIR_Date THEN DATEDIFF(DAY, ST.OEX_Date, ST.HIR_Date)
                WHEN ST.ODC_Date IS NOT NULL AND ST.OEX_Date <= ST.ODC_Date THEN DATEDIFF(DAY, ST.OEX_Date, ST.ODC_Date)
                WHEN ST.WDR_Date IS NOT NULL AND ST.OEX_Date <= ST.WDR_Date THEN DATEDIFF(DAY, ST.OEX_Date, ST.WDR_Date)
                ELSE DATEDIFF(DAY, ST.OEX_Date, @Today)
            END as OEX_Days,
            
            CASE 
                WHEN ST.ODC_Date IS NULL THEN 0
                ELSE 0 -- ODC is final stage
            END as ODC_Days,
            
            CASE 
                WHEN ST.HIR_Date IS NULL THEN 0
                ELSE 0 -- HIR is final stage
            END as HIR_Days,
            
            CASE 
                WHEN ST.WDR_Date IS NULL THEN 0
                ELSE 0 -- WDR is final stage
            END as WDR_Days
            
        FROM StageTimings ST
        INNER JOIN FirstSubmissions FS ON ST.RequisitionId = FS.RequisitionId AND ST.CandidateId = FS.CandidateId
    )
    
    -- Query 1: Timing Analytics by Requisition
    SELECT 
        @User as [User],
        RB.RequisitionCode,
        RB.Title,
        -- Average Time to Fill for this requisition (if filled)
        CEILING(AVG(CAST(RB.TimeToFill as FLOAT))) as AvgTimeToFill,
        -- Average Time to Hire for this requisition  
        CEILING(AVG(CAST(TTH.TimeToHire as FLOAT))) as AvgTimeToHire,
        -- Average time in each stage for this requisition (rounded up to next integer)
        CEILING(AVG(CAST(STC.PEN_Days as FLOAT))) as AvgPEN_Days,
        CEILING(AVG(CAST(STC.REJ_Days as FLOAT))) as AvgREJ_Days,
        CEILING(AVG(CAST(STC.HLD_Days as FLOAT))) as AvgHLD_Days,
        CEILING(AVG(CAST(STC.PHN_Days as FLOAT))) as AvgPHN_Days,
        CEILING(AVG(CAST(STC.URW_Days as FLOAT))) as AvgURW_Days,
        CEILING(AVG(CAST(STC.INT_Days as FLOAT))) as AvgINT_Days,
        CEILING(AVG(CAST(STC.RHM_Days as FLOAT))) as AvgRHM_Days,
        CEILING(AVG(CAST(STC.DEC_Days as FLOAT))) as AvgDEC_Days,
        CEILING(AVG(CAST(STC.NOA_Days as FLOAT))) as AvgNOA_Days,
        CEILING(AVG(CAST(STC.OEX_Days as FLOAT))) as AvgOEX_Days,
        CEILING(AVG(CAST(STC.ODC_Days as FLOAT))) as AvgODC_Days,
        CEILING(AVG(CAST(STC.HIR_Days as FLOAT))) as AvgHIR_Days,
        CEILING(AVG(CAST(STC.WDR_Days as FLOAT))) as AvgWDR_Days,
        COUNT(DISTINCT CONCAT(STC.RequisitionId, '-', STC.CandidateId)) as TotalCandidates
    INTO #TimingAnalytics
    FROM RequisitionBase RB
    LEFT JOIN TimeToHireCalc TTH ON RB.RequisitionId = TTH.RequisitionId
    LEFT JOIN StageTimeCalculations STC ON RB.RequisitionId = STC.RequisitionId
    GROUP BY RB.RequisitionCode, RB.Title;
    
    -- Return the timing analytics results by requisition
    SELECT 
        [User],
        RequisitionCode,
        Title,
        ISNULL(AvgTimeToFill, 0) as TimeToFill_Days,
        ISNULL(AvgTimeToHire, 0) as TimeToHire_Days,
        ISNULL(AvgPEN_Days, 0) as PEN_Days,
        ISNULL(AvgREJ_Days, 0) as REJ_Days,
        ISNULL(AvgHLD_Days, 0) as HLD_Days,
        ISNULL(AvgPHN_Days, 0) as PHN_Days,
        ISNULL(AvgURW_Days, 0) as URW_Days,
        ISNULL(AvgINT_Days, 0) as INT_Days,
        ISNULL(AvgRHM_Days, 0) as RHM_Days,
        ISNULL(AvgDEC_Days, 0) as DEC_Days,
        ISNULL(AvgNOA_Days, 0) as NOA_Days,
        ISNULL(AvgOEX_Days, 0) as OEX_Days,
        ISNULL(AvgODC_Days, 0) as ODC_Days,
        ISNULL(AvgHIR_Days, 0) as HIR_Days,
        ISNULL(AvgWDR_Days, 0) as WDR_Days,
        TotalCandidates
    FROM #TimingAnalytics
    ORDER BY RequisitionCode
    FOR JSON PATH;
    
    -- Clean up timing analytics temp table
    DROP TABLE #TimingAnalytics;
    
    -- Query 2: Timing Analytics by Company
    WITH RequisitionBase AS (
        -- Get requisitions created by user in last year
        SELECT 
            R.Id as RequisitionId,
            C.CompanyName,
            R.CreatedDate as RequisitionCreatedDate,
            R.CreatedBy,
            -- Time to Fill calculation (only for FUL status)
            CASE 
                WHEN R.Status = 'FUL' THEN DATEDIFF(DAY, R.CreatedDate, R.UpdatedDate)
                ELSE NULL 
            END as TimeToFill
        FROM Requisitions R
        INNER JOIN Companies C ON R.CompanyId = C.ID
        WHERE R.CreatedBy = @User 
            AND CAST(R.CreatedDate AS DATE) >= @StartDate365
    ),
    FirstSubmissions AS (
        -- Get first submission date for each candidate+requisition combo
        SELECT 
            S.RequisitionId,
            S.CandidateId,
            MIN(S.CreatedDate) as FirstSubmissionDate
        FROM Submissions S
        INNER JOIN RequisitionBase RB ON S.RequisitionId = RB.RequisitionId
        GROUP BY S.RequisitionId, S.CandidateId
    ),
    TimeToHireCalc AS (
        -- Calculate Time to Hire for hired candidates
        SELECT 
            FS.RequisitionId,
            FS.CandidateId,
            FS.FirstSubmissionDate,
            MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HireDate,
            MAX(CASE WHEN S.Status = 'HIR' THEN DATEDIFF(DAY, FS.FirstSubmissionDate, S.CreatedDate) END) as TimeToHire
        FROM FirstSubmissions FS
        INNER JOIN Submissions S ON FS.RequisitionId = S.RequisitionId AND FS.CandidateId = S.CandidateId
        GROUP BY FS.RequisitionId, FS.CandidateId, FS.FirstSubmissionDate
    ),
    StageTimings AS (
        -- Calculate time in each stage
        SELECT 
            FS.RequisitionId,
            FS.CandidateId,
            -- Get the latest date for each stage (in case of multiple records for same stage)
            MAX(CASE WHEN S.Status = 'PEN' THEN S.CreatedDate END) as PEN_Date,
            MAX(CASE WHEN S.Status = 'REJ' THEN S.CreatedDate END) as REJ_Date,
            MAX(CASE WHEN S.Status = 'HLD' THEN S.CreatedDate END) as HLD_Date,
            MAX(CASE WHEN S.Status = 'PHN' THEN S.CreatedDate END) as PHN_Date,
            MAX(CASE WHEN S.Status = 'URW' THEN S.CreatedDate END) as URW_Date,
            MAX(CASE WHEN S.Status = 'INT' THEN S.CreatedDate END) as INT_Date,
            MAX(CASE WHEN S.Status = 'RHM' THEN S.CreatedDate END) as RHM_Date,
            MAX(CASE WHEN S.Status = 'DEC' THEN S.CreatedDate END) as DEC_Date,
            MAX(CASE WHEN S.Status = 'NOA' THEN S.CreatedDate END) as NOA_Date,
            MAX(CASE WHEN S.Status = 'OEX' THEN S.CreatedDate END) as OEX_Date,
            MAX(CASE WHEN S.Status = 'ODC' THEN S.CreatedDate END) as ODC_Date,
            MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HIR_Date,
            MAX(CASE WHEN S.Status = 'WDR' THEN S.CreatedDate END) as WDR_Date
        FROM FirstSubmissions FS
        INNER JOIN Submissions S ON FS.RequisitionId = S.RequisitionId AND FS.CandidateId = S.CandidateId
        GROUP BY FS.RequisitionId, FS.CandidateId
    ),
    StageTimeCalculations AS (
        -- Calculate days spent in each stage
        SELECT 
            ST.RequisitionId,
            ST.CandidateId,
            FS.FirstSubmissionDate,
            -- Calculate time in each stage (from current stage to next stage or end)
            CASE 
                WHEN ST.PEN_Date IS NULL THEN 0
                WHEN ST.REJ_Date IS NOT NULL AND ST.PEN_Date <= ST.REJ_Date THEN DATEDIFF(DAY, ST.PEN_Date, ST.REJ_Date)
                WHEN ST.HLD_Date IS NOT NULL AND ST.PEN_Date <= ST.HLD_Date THEN DATEDIFF(DAY, ST.PEN_Date, ST.HLD_Date)
                WHEN ST.PHN_Date IS NOT NULL AND ST.PEN_Date <= ST.PHN_Date THEN DATEDIFF(DAY, ST.PEN_Date, ST.PHN_Date)
                ELSE DATEDIFF(DAY, ST.PEN_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as PEN_Days,
            
            CASE 
                WHEN ST.REJ_Date IS NULL THEN 0
                ELSE 0 -- REJ is final stage
            END as REJ_Days,
            
            CASE 
                WHEN ST.HLD_Date IS NULL THEN 0
                WHEN ST.PHN_Date IS NOT NULL AND ST.HLD_Date <= ST.PHN_Date THEN DATEDIFF(DAY, ST.HLD_Date, ST.PHN_Date)
                WHEN ST.URW_Date IS NOT NULL AND ST.HLD_Date <= ST.URW_Date THEN DATEDIFF(DAY, ST.HLD_Date, ST.URW_Date)
                ELSE DATEDIFF(DAY, ST.HLD_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as HLD_Days,
            
            CASE 
                WHEN ST.PHN_Date IS NULL THEN 0
                WHEN ST.URW_Date IS NOT NULL AND ST.PHN_Date <= ST.URW_Date THEN DATEDIFF(DAY, ST.PHN_Date, ST.URW_Date)
                WHEN ST.INT_Date IS NOT NULL AND ST.PHN_Date <= ST.INT_Date THEN DATEDIFF(DAY, ST.PHN_Date, ST.INT_Date)
                ELSE DATEDIFF(DAY, ST.PHN_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as PHN_Days,
            
            CASE 
                WHEN ST.URW_Date IS NULL THEN 0
                WHEN ST.INT_Date IS NOT NULL AND ST.URW_Date <= ST.INT_Date THEN DATEDIFF(DAY, ST.URW_Date, ST.INT_Date)
                WHEN ST.DEC_Date IS NOT NULL AND ST.URW_Date <= ST.DEC_Date THEN DATEDIFF(DAY, ST.URW_Date, ST.DEC_Date)
                ELSE DATEDIFF(DAY, ST.URW_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as URW_Days,
            
            CASE 
                WHEN ST.INT_Date IS NULL THEN 0
                WHEN ST.DEC_Date IS NOT NULL AND ST.INT_Date <= ST.DEC_Date THEN DATEDIFF(DAY, ST.INT_Date, ST.DEC_Date)
                WHEN ST.NOA_Date IS NOT NULL AND ST.INT_Date <= ST.NOA_Date THEN DATEDIFF(DAY, ST.INT_Date, ST.NOA_Date)
                ELSE DATEDIFF(DAY, ST.INT_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as INT_Days,
            
            CASE 
                WHEN ST.RHM_Date IS NULL THEN 0
                ELSE 0 -- RHM is final stage
            END as RHM_Days,
            
            CASE 
                WHEN ST.DEC_Date IS NULL THEN 0
                WHEN ST.NOA_Date IS NOT NULL AND ST.DEC_Date <= ST.NOA_Date THEN DATEDIFF(DAY, ST.DEC_Date, ST.NOA_Date)
                WHEN ST.OEX_Date IS NOT NULL AND ST.DEC_Date <= ST.OEX_Date THEN DATEDIFF(DAY, ST.DEC_Date, ST.OEX_Date)
                ELSE DATEDIFF(DAY, ST.DEC_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as DEC_Days,
            
            CASE 
                WHEN ST.NOA_Date IS NULL THEN 0
                WHEN ST.OEX_Date IS NOT NULL AND ST.NOA_Date <= ST.OEX_Date THEN DATEDIFF(DAY, ST.NOA_Date, ST.OEX_Date)
                ELSE DATEDIFF(DAY, ST.NOA_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as NOA_Days,
            
            CASE 
                WHEN ST.OEX_Date IS NULL THEN 0
                WHEN ST.HIR_Date IS NOT NULL AND ST.OEX_Date <= ST.HIR_Date THEN DATEDIFF(DAY, ST.OEX_Date, ST.HIR_Date)
                WHEN ST.ODC_Date IS NOT NULL AND ST.OEX_Date <= ST.ODC_Date THEN DATEDIFF(DAY, ST.OEX_Date, ST.ODC_Date)
                WHEN ST.WDR_Date IS NOT NULL AND ST.OEX_Date <= ST.WDR_Date THEN DATEDIFF(DAY, ST.OEX_Date, ST.WDR_Date)
                ELSE DATEDIFF(DAY, ST.OEX_Date, @Today)
            END as OEX_Days,
            
            CASE 
                WHEN ST.ODC_Date IS NULL THEN 0
                ELSE 0 -- ODC is final stage
            END as ODC_Days,
            
            CASE 
                WHEN ST.HIR_Date IS NULL THEN 0
                ELSE 0 -- HIR is final stage
            END as HIR_Days,
            
            CASE 
                WHEN ST.WDR_Date IS NULL THEN 0
                ELSE 0 -- WDR is final stage
            END as WDR_Days
            
        FROM StageTimings ST
        INNER JOIN FirstSubmissions FS ON ST.RequisitionId = FS.RequisitionId AND ST.CandidateId = FS.CandidateId
    )
    
    -- Return the timing analytics results by company
    SELECT 
        @User as [User],
        RB.CompanyName,
        -- Average Time to Fill for this company (if filled)
        ISNULL(CEILING(AVG(CAST(RB.TimeToFill as FLOAT))), 0) as TimeToFill_Days,
        -- Average Time to Hire for this company  
        ISNULL(CEILING(AVG(CAST(TTH.TimeToHire as FLOAT))), 0) as TimeToHire_Days,
        -- Average time in each stage for this company (rounded up to next integer)
        ISNULL(CEILING(AVG(CAST(STC.PEN_Days as FLOAT))), 0) as PEN_Days,
        ISNULL(CEILING(AVG(CAST(STC.REJ_Days as FLOAT))), 0) as REJ_Days,
        ISNULL(CEILING(AVG(CAST(STC.HLD_Days as FLOAT))), 0) as HLD_Days,
        ISNULL(CEILING(AVG(CAST(STC.PHN_Days as FLOAT))), 0) as PHN_Days,
        ISNULL(CEILING(AVG(CAST(STC.URW_Days as FLOAT))), 0) as URW_Days,
        ISNULL(CEILING(AVG(CAST(STC.INT_Days as FLOAT))), 0) as INT_Days,
        ISNULL(CEILING(AVG(CAST(STC.RHM_Days as FLOAT))), 0) as RHM_Days,
        ISNULL(CEILING(AVG(CAST(STC.DEC_Days as FLOAT))), 0) as DEC_Days,
        ISNULL(CEILING(AVG(CAST(STC.NOA_Days as FLOAT))), 0) as NOA_Days,
        ISNULL(CEILING(AVG(CAST(STC.OEX_Days as FLOAT))), 0) as OEX_Days,
        ISNULL(CEILING(AVG(CAST(STC.ODC_Days as FLOAT))), 0) as ODC_Days,
        ISNULL(CEILING(AVG(CAST(STC.HIR_Days as FLOAT))), 0) as HIR_Days,
        ISNULL(CEILING(AVG(CAST(STC.WDR_Days as FLOAT))), 0) as WDR_Days,
        COUNT(DISTINCT CONCAT(STC.RequisitionId, '-', STC.CandidateId)) as TotalCandidates,
        COUNT(DISTINCT RB.RequisitionId) as TotalRequisitions
    FROM RequisitionBase RB
    LEFT JOIN TimeToHireCalc TTH ON RB.RequisitionId = TTH.RequisitionId
    LEFT JOIN StageTimeCalculations STC ON RB.RequisitionId = STC.RequisitionId
    GROUP BY RB.CompanyName
    ORDER BY RB.CompanyName
    FOR JSON PATH;
    
    -- Clean up temp tables
    DROP TABLE #RequisitionMetrics;
    DROP TABLE #SubmissionMetrics;
END;

GO

ALTER PROCEDURE [dbo].[DashboardAdmin]
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;
    
    DECLARE @Today DATE = GETDATE();
    DECLARE @YearStart DATE = DATEFROMPARTS(YEAR(@Today), 1, 1);
    
    -- Generic Query for Dropdown
    SELECT UserName as KeyValue, FirstName + ' ' + LastName as Text
    FROM dbo.Users 
    WHERE Role = 5 AND Status = 1
    FOR JSON AUTO;
    
    -- Create temp table for submission data
    CREATE TABLE #SubmissionData (
        CandidateId INT,
        RequisitionId INT,
        Status CHAR(3) COLLATE Latin1_General_CI_AI,
        MaxCreatedDate DATETIME,
        CreatedBy VARCHAR(10) COLLATE Latin1_General_CI_AI
    );
    
    -- Populate temp table with submission data
    INSERT INTO #SubmissionData
    SELECT 
        S.CandidateId,
        S.RequisitionId,
        S.Status,
        MAX(S.CreatedDate) as MaxCreatedDate,
        R.CreatedBy
    FROM dbo.Submissions S INNER JOIN dbo.Requisitions R ON S.RequisitionId = R.ID
    WHERE S.Status IN ('INT', 'OEX', 'HIR')
    GROUP BY S.CandidateId, S.RequisitionId, S.Status, R.CreatedBy
    HAVING R.CreatedBy IS NOT NULL;
    
    -- Common date calculations
    DECLARE @StartDate7D DATE = DATEADD(DAY, -6, @Today);
    DECLARE @StartDateMTD DATE = DATEADD(DAY, 1, EOMONTH(@Today, -1));
    DECLARE @StartDateQTD DATE = DATEADD(QUARTER, DATEDIFF(QUARTER, 0, @Today), 0);
    DECLARE @StartDateHYTD DATE = CASE 
        WHEN MONTH(@Today) <= 6 THEN DATEFROMPARTS(YEAR(@Today), 1, 1)
        ELSE DATEFROMPARTS(YEAR(@Today), 7, 1)
    END;
    
    -- Common Users CTE
    WITH AllUsers AS (
        SELECT DISTINCT R.CreatedBy
        FROM dbo.Requisitions R INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1
        WHERE R.CreatedBy IS NOT NULL AND R.CreatedDate >= DATEADD(YEAR, -1, GETDATE())
    )
    
    -- SET 1 - Query 1: Requisitions Created (Conditional Aggregation)
    SELECT 
        U.CreatedBy as [User],
        SUM(CASE WHEN CAST(R.CreatedDate AS DATE) BETWEEN @StartDate7D AND @Today THEN 1 ELSE 0 END) as LAST7D_COUNT,
        SUM(CASE WHEN CAST(R.CreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today THEN 1 ELSE 0 END) as MTD_COUNT,
        SUM(CASE WHEN CAST(R.CreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today THEN 1 ELSE 0 END) as QTD_COUNT,
        SUM(CASE WHEN CAST(R.CreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today THEN 1 ELSE 0 END) as HYTD_COUNT,
        SUM(CASE WHEN CAST(R.CreatedDate AS DATE) BETWEEN @YearStart AND @Today THEN 1 ELSE 0 END) as YTD_COUNT
    FROM AllUsers U
    LEFT JOIN dbo.Requisitions R ON R.CreatedBy = U.CreatedBy
    GROUP BY U.CreatedBy
    ORDER BY U.CreatedBy
        FOR JSON PATH
        ;

    
    -- SET 1 - Query 2: Active Requisitions Updated (Conditional Aggregation)
    WITH AllUsers AS (
        SELECT DISTINCT R.CreatedBy
        FROM dbo.Requisitions R INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1
        WHERE R.CreatedBy IS NOT NULL AND R.CreatedDate >= DATEADD(YEAR, -1, GETDATE())
    )
    SELECT 
        U.CreatedBy as [User],
        SUM(CASE WHEN CAST(R.UpdatedDate AS DATE) BETWEEN @StartDate7D AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN 1 ELSE 0 END) as LAST7D_COUNT,
        SUM(CASE WHEN CAST(R.UpdatedDate AS DATE) BETWEEN @StartDateMTD AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN 1 ELSE 0 END) as MTD_COUNT,
        SUM(CASE WHEN CAST(R.UpdatedDate AS DATE) BETWEEN @StartDateQTD AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN 1 ELSE 0 END) as QTD_COUNT,
        SUM(CASE WHEN CAST(R.UpdatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN 1 ELSE 0 END) as HYTD_COUNT,
        SUM(CASE WHEN CAST(R.UpdatedDate AS DATE) BETWEEN @YearStart AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN 1 ELSE 0 END) as YTD_COUNT
    FROM AllUsers U
    LEFT JOIN dbo.Requisitions R ON R.CreatedBy = U.CreatedBy
    GROUP BY U.CreatedBy
    ORDER BY U.CreatedBy
    FOR JSON PATH
    ;

    
    -- SET 1 - Query 3: Face-to-Face Interviews (INT) (Conditional Aggregation)
    WITH AllUsers AS (
        SELECT DISTINCT R.CreatedBy
        FROM dbo.Requisitions R INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1
        WHERE R.CreatedBy IS NOT NULL AND R.CreatedDate >= DATEADD(YEAR, -1, GETDATE())
    )
    SELECT 
        U.CreatedBy as [User],
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDate7D AND @Today AND SD.Status = 'INT' THEN 1 ELSE 0 END) as LAST7D_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today AND SD.Status = 'INT' THEN 1 ELSE 0 END) as MTD_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today AND SD.Status = 'INT' THEN 1 ELSE 0 END) as QTD_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND SD.Status = 'INT' THEN 1 ELSE 0 END) as HYTD_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @YearStart AND @Today AND SD.Status = 'INT' THEN 1 ELSE 0 END) as YTD_COUNT
    FROM AllUsers U
    LEFT JOIN #SubmissionData SD ON SD.CreatedBy = U.CreatedBy
    GROUP BY U.CreatedBy
    ORDER BY U.CreatedBy    
    FOR JSON PATH
    ;

    
    -- SET 1 - Query 4: Offers Extended (OEX) (Conditional Aggregation)
    WITH AllUsers AS (
        SELECT DISTINCT R.CreatedBy
        FROM dbo.Requisitions R INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1
        WHERE R.CreatedBy IS NOT NULL AND R.CreatedDate >= DATEADD(YEAR, -1, GETDATE())
    )
    SELECT 
        U.CreatedBy as [User],
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDate7D AND @Today AND SD.Status = 'OEX' THEN 1 ELSE 0 END) as LAST7D_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today AND SD.Status = 'OEX' THEN 1 ELSE 0 END) as MTD_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today AND SD.Status = 'OEX' THEN 1 ELSE 0 END) as QTD_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND SD.Status = 'OEX' THEN 1 ELSE 0 END) as HYTD_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @YearStart AND @Today AND SD.Status = 'OEX' THEN 1 ELSE 0 END) as YTD_COUNT
    FROM AllUsers U
    LEFT JOIN #SubmissionData SD ON SD.CreatedBy = U.CreatedBy
    GROUP BY U.CreatedBy
    ORDER BY U.CreatedBy   
    FOR JSON PATH
    ;

    
    -- SET 1 - Query 5: Hires (HIR) (Conditional Aggregation)
    WITH AllUsers AS (
        SELECT DISTINCT R.CreatedBy
        FROM dbo.Requisitions R INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1
        WHERE R.CreatedBy IS NOT NULL AND R.CreatedDate >= DATEADD(YEAR, -1, GETDATE())
    )
    SELECT 
        U.CreatedBy as [User],
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDate7D AND @Today AND SD.Status = 'HIR' THEN 1 ELSE 0 END) as LAST7D_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today AND SD.Status = 'HIR' THEN 1 ELSE 0 END) as MTD_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today AND SD.Status = 'HIR' THEN 1 ELSE 0 END) as QTD_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND SD.Status = 'HIR' THEN 1 ELSE 0 END) as HYTD_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @YearStart AND @Today AND SD.Status = 'HIR' THEN 1 ELSE 0 END) as YTD_COUNT
    FROM AllUsers U
    LEFT JOIN #SubmissionData SD ON SD.CreatedBy = U.CreatedBy
    GROUP BY U.CreatedBy
    ORDER BY U.CreatedBy    
    FOR JSON PATH
    ;

    
    -- SET 1 - Query 6: OEX to HIR Ratio (Conditional Aggregation)
    WITH AllUsers AS (
        SELECT DISTINCT R.CreatedBy
        FROM dbo.Requisitions R INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1
        WHERE R.CreatedBy IS NOT NULL AND R.CreatedDate >= DATEADD(YEAR, -1, GETDATE())
    ),
    RatioCalculations AS (
        SELECT 
            U.CreatedBy as [User],
            -- 7D calculations
            SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDate7D AND @Today AND SD.Status = 'OEX' THEN 1 ELSE 0 END) as OEX_7D,
            SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDate7D AND @Today AND SD.Status = 'HIR' THEN 1 ELSE 0 END) as HIR_7D,
            -- MTD calculations
            SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today AND SD.Status = 'OEX' THEN 1 ELSE 0 END) as OEX_MTD,
            SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today AND SD.Status = 'HIR' THEN 1 ELSE 0 END) as HIR_MTD,
            -- QTD calculations
            SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today AND SD.Status = 'OEX' THEN 1 ELSE 0 END) as OEX_QTD,
            SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today AND SD.Status = 'HIR' THEN 1 ELSE 0 END) as HIR_QTD,
            -- HYTD calculations
            SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND SD.Status = 'OEX' THEN 1 ELSE 0 END) as OEX_HYTD,
            SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND SD.Status = 'HIR' THEN 1 ELSE 0 END) as HIR_HYTD,
            -- YTD calculations
            SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @YearStart AND @Today AND SD.Status = 'OEX' THEN 1 ELSE 0 END) as OEX_YTD,
            SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @YearStart AND @Today AND SD.Status = 'HIR' THEN 1 ELSE 0 END) as HIR_YTD
        FROM AllUsers U
        LEFT JOIN #SubmissionData SD ON SD.CreatedBy = U.CreatedBy
        GROUP BY U.CreatedBy
    )
    SELECT 
        [User],
        CASE WHEN OEX_7D = 0 THEN 0.00 ELSE CAST(HIR_7D AS DECIMAL(10,2)) / CAST(OEX_7D AS DECIMAL(10,2)) END as LAST7D_RATIO,
        CASE WHEN OEX_MTD = 0 THEN 0.00 ELSE CAST(HIR_MTD AS DECIMAL(10,2)) / CAST(OEX_MTD AS DECIMAL(10,2)) END as MTD_RATIO,
        CASE WHEN OEX_QTD = 0 THEN 0.00 ELSE CAST(HIR_QTD AS DECIMAL(10,2)) / CAST(OEX_QTD AS DECIMAL(10,2)) END as QTD_RATIO,
        CASE WHEN OEX_HYTD = 0 THEN 0.00 ELSE CAST(HIR_HYTD AS DECIMAL(10,2)) / CAST(OEX_HYTD AS DECIMAL(10,2)) END as HYTD_RATIO,
        CASE WHEN OEX_YTD = 0 THEN 0.00 ELSE CAST(HIR_YTD AS DECIMAL(10,2)) / CAST(OEX_YTD AS DECIMAL(10,2)) END as YTD_RATIO
    FROM RatioCalculations
    ORDER BY [User]    
    FOR JSON PATH
    ;

    -- SET 2 - Query 1: Company Submission Activity
    DECLARE @StartDateSet2 DATE = CASE 
        WHEN DATEADD(DAY, -30, @Today) < DATEFROMPARTS(YEAR(@Today), 1, 1) 
        THEN DATEFROMPARTS(YEAR(@Today), 1, 1)
        ELSE DATEADD(DAY, -30, @Today)
    END;
    DECLARE @return varchar(max);
    
    WITH SubmissionSummary AS (
        SELECT 
            S.RequisitionId,
            S.CandidateId,
            MIN(S.CreatedDate) as DateFirstSubmitted,
            MAX(S.CreatedDate) as LastActivityDate
        FROM dbo.Submissions S
        INNER JOIN dbo.Requisitions R ON S.RequisitionId = R.Id
        GROUP BY S.RequisitionId, S.CandidateId
        HAVING MIN(S.CreatedDate) >= @StartDateSet2
    ),
    CurrentStatus AS (
        SELECT 
            SS.RequisitionId,
            SS.CandidateId,
            SS.DateFirstSubmitted,
            SS.LastActivityDate,
            S.Status as CurrentStatus,
            S.Notes as ActivityNotes
        FROM SubmissionSummary SS
        INNER JOIN dbo.Submissions S ON S.RequisitionId = SS.RequisitionId 
            AND S.CandidateId = SS.CandidateId 
            AND S.CreatedDate = SS.LastActivityDate
    ),
    CompanySubmissionCounts AS (
        SELECT 
            C.CompanyName,
            COUNT(*) as SubmissionCount
        FROM CurrentStatus CS
        INNER JOIN dbo.Requisitions R ON CS.RequisitionId = R.Id
        INNER JOIN dbo.Companies C ON R.CompanyId = C.ID
        GROUP BY C.CompanyName
    )

    SELECT @return = 
    (SELECT 
        C.CompanyName + ' - [' + R.Code + '] - ' + CAST(R.Positions as varchar(5)) + ' Positions - ' + R.PosTitle as Company,
        LTRIM(RTRIM(ISNULL(CAND.FirstName, '') + ' ' + ISNULL(CAND.LastName, ''))) as [CandidateName],
        CS.CurrentStatus as [CurrentStatus],
        CS.DateFirstSubmitted as [DateFirstSubmitted],
        CS.LastActivityDate as [LastActivityDate],
        ISNULL(CS.ActivityNotes, '') as [ActivityNotes],
        R.CreatedBy [User]
    FROM CurrentStatus CS
    INNER JOIN dbo.Requisitions R ON CS.RequisitionId = R.Id
    INNER JOIN dbo.Companies C ON R.CompanyId = C.ID
    INNER JOIN dbo.Candidate CAND ON CS.CandidateId = CAND.ID
    INNER JOIN CompanySubmissionCounts CSC ON C.CompanyName = CSC.CompanyName
    ORDER BY 
        CSC.SubmissionCount DESC,
        C.CompanyName ASC,
        R.Id ASC,
        CS.DateFirstSubmitted ASC
    FOR JSON PATH);

    SELECT @return;
    
    -- SET 3 - Query 1: Hired Candidates Report
    DECLARE @StartDateSet3 DATE = DATEADD(MONTH, -3, @Today);
    
    WITH HiredCandidates AS (
        SELECT 
            S.RequisitionId,
            S.CandidateId,
            MAX(S.CreatedDate) as DateHired
        FROM dbo.Submissions S
        INNER JOIN dbo.Requisitions R ON S.RequisitionId = R.Id
        WHERE S.Status = 'HIR'
        GROUP BY S.RequisitionId, S.CandidateId, R.CreatedBy
        HAVING MAX(S.CreatedDate) >= @StartDateSet3
    )

    SELECT @return =
    (SELECT 
        C.CompanyName as Company,
        R.Code as [RequisitionNumber],
        R.Positions as [NumPosition],
        R.PosTitle as Title,
        LTRIM(RTRIM(ISNULL(CAND.FirstName, '') + ' ' + ISNULL(CAND.LastName, ''))) as [CandidateName],
        HC.DateHired as [DateHired],
        CAST(0.00 AS DECIMAL(9,2)) as [SalaryOffered],
        @Today as [StartDate],
        @Today as [DateInvoiced],
        @Today as [DatePaid],
        CAST(0.00 AS DECIMAL(9,2)) as [Placementfee],
        CAST(0.00 AS DECIMAL(5,2)) as [CommissionPercent],
        CAST(0.00 AS DECIMAL(9,2)) as [CommissionEarned],
        R.CreatedBy [User]
    FROM HiredCandidates HC
    INNER JOIN dbo.Requisitions R ON HC.RequisitionId = R.Id
    INNER JOIN dbo.Companies C ON R.CompanyId = C.ID
    INNER JOIN dbo.Candidate CAND ON HC.CandidateId = CAND.ID
    ORDER BY 
        R.CreatedBy,
        C.CompanyName,
        R.Code,
        HC.DateHired
    FOR JSON PATH);
    
    SELECT @return
    -- Clean up
    DROP TABLE #SubmissionData;
    /***************************************************************************
    * RESULT SET 4: ADMIN TIMING ANALYTICS (2 Queries)
    * Query 1: Time to Fill, Time to Hire, and Time in Stage metrics (By Requisition) 
    * Query 2: Time to Fill, Time to Hire, and Time in Stage metrics (By Company)
    * Shows data for ALL USERS (Admin perspective)
    ***************************************************************************/
    
    -- Get requisitions from last year for all users
    DECLARE @StartDate365 DATE = DATEADD(YEAR, -1, @Today);
    
    -- Create temp tables for admin timing analytics
    DROP TABLE IF EXISTS #AdminRequisitionBase;
    DROP TABLE IF EXISTS #AdminFirstSubmissions;
    DROP TABLE IF EXISTS #AdminTimeToHireCalc;
    DROP TABLE IF EXISTS #AdminStageTimings;
    DROP TABLE IF EXISTS #AdminStageTimeCalculations;
    
    -- Temp table 1: Get requisitions created by ALL USERS in last year
    SELECT 
        R.Id as RequisitionId,
        R.Code as RequisitionCode,
        ISNULL(C.CompanyName, 'Unknown Company') as CompanyName,
        R.PosTitle as Title,
        R.CreatedDate as RequisitionCreatedDate,
        R.CreatedBy,
        -- Time to Fill calculation (only for FUL status)
        CASE 
            WHEN R.Status = 'FUL' THEN DATEDIFF(DAY, R.CreatedDate, R.UpdatedDate)
            ELSE NULL 
        END as TimeToFill
    INTO #AdminRequisitionBase
    FROM Requisitions R
    LEFT JOIN Companies C ON R.CompanyId = C.ID
    INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1
    WHERE CAST(R.CreatedDate AS DATE) >= @StartDate365
        AND R.CreatedBy IS NOT NULL;
    
    -- Temp table 2: Get first submission date for each candidate+requisition combo (all users)
    SELECT 
        S.RequisitionId,
        S.CandidateId,
        MIN(S.CreatedDate) as FirstSubmissionDate
    INTO #AdminFirstSubmissions
    FROM Submissions S
    INNER JOIN #AdminRequisitionBase ARB ON S.RequisitionId = ARB.RequisitionId
    GROUP BY S.RequisitionId, S.CandidateId;
    
    -- Temp table 3: Calculate Time to Hire for hired candidates (all users)
    SELECT 
        AFS.RequisitionId,
        AFS.CandidateId,
        AFS.FirstSubmissionDate,
        MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HireDate,
        MAX(CASE WHEN S.Status = 'HIR' THEN DATEDIFF(DAY, AFS.FirstSubmissionDate, S.CreatedDate) END) as TimeToHire
    INTO #AdminTimeToHireCalc
    FROM #AdminFirstSubmissions AFS
    INNER JOIN Submissions S ON AFS.RequisitionId = S.RequisitionId AND AFS.CandidateId = S.CandidateId
    GROUP BY AFS.RequisitionId, AFS.CandidateId, AFS.FirstSubmissionDate;
    
    -- Temp table 4: Calculate time in each stage for all users
    SELECT 
        AFS.RequisitionId,
        AFS.CandidateId,
        -- Get the latest date for each stage (in case of multiple records for same stage)
        MAX(CASE WHEN S.Status = 'PEN' THEN S.CreatedDate END) as PEN_Date,
        MAX(CASE WHEN S.Status = 'REJ' THEN S.CreatedDate END) as REJ_Date,
        MAX(CASE WHEN S.Status = 'HLD' THEN S.CreatedDate END) as HLD_Date,
        MAX(CASE WHEN S.Status = 'PHN' THEN S.CreatedDate END) as PHN_Date,
        MAX(CASE WHEN S.Status = 'URW' THEN S.CreatedDate END) as URW_Date,
        MAX(CASE WHEN S.Status = 'INT' THEN S.CreatedDate END) as INT_Date,
        MAX(CASE WHEN S.Status = 'RHM' THEN S.CreatedDate END) as RHM_Date,
        MAX(CASE WHEN S.Status = 'DEC' THEN S.CreatedDate END) as DEC_Date,
        MAX(CASE WHEN S.Status = 'NOA' THEN S.CreatedDate END) as NOA_Date,
        MAX(CASE WHEN S.Status = 'OEX' THEN S.CreatedDate END) as OEX_Date,
        MAX(CASE WHEN S.Status = 'ODC' THEN S.CreatedDate END) as ODC_Date,
        MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HIR_Date,
        MAX(CASE WHEN S.Status = 'WDR' THEN S.CreatedDate END) as WDR_Date
    INTO #AdminStageTimings
    FROM #AdminFirstSubmissions AFS
    INNER JOIN Submissions S ON AFS.RequisitionId = S.RequisitionId AND AFS.CandidateId = S.CandidateId
    GROUP BY AFS.RequisitionId, AFS.CandidateId;
    
    -- Temp table 5: Calculate time spent in each stage for all users
    SELECT 
        AST.RequisitionId,
        AST.CandidateId,
        
        CASE 
            WHEN AST.PEN_Date IS NULL THEN 0
            WHEN AST.REJ_Date IS NOT NULL THEN DATEDIFF(DAY, AST.PEN_Date, AST.REJ_Date)
            WHEN AST.HLD_Date IS NOT NULL THEN DATEDIFF(DAY, AST.PEN_Date, AST.HLD_Date)
            WHEN AST.PHN_Date IS NOT NULL THEN DATEDIFF(DAY, AST.PEN_Date, AST.PHN_Date)
            ELSE DATEDIFF(DAY, AST.PEN_Date, @Today)
        END as PEN_Days,
        
        CASE 
            WHEN AST.REJ_Date IS NULL THEN 0
            ELSE 0 -- REJ is final stage
        END as REJ_Days,
        
        CASE 
            WHEN AST.HLD_Date IS NULL THEN 0
            WHEN AST.PHN_Date IS NOT NULL THEN DATEDIFF(DAY, AST.HLD_Date, AST.PHN_Date)
            WHEN AST.URW_Date IS NOT NULL THEN DATEDIFF(DAY, AST.HLD_Date, AST.URW_Date)
            ELSE DATEDIFF(DAY, AST.HLD_Date, @Today)
        END as HLD_Days,
        
        CASE 
            WHEN AST.PHN_Date IS NULL THEN 0
            WHEN AST.URW_Date IS NOT NULL THEN DATEDIFF(DAY, AST.PHN_Date, AST.URW_Date)
            WHEN AST.INT_Date IS NOT NULL THEN DATEDIFF(DAY, AST.PHN_Date, AST.INT_Date)
            ELSE DATEDIFF(DAY, AST.PHN_Date, @Today)
        END as PHN_Days,
        
        CASE 
            WHEN AST.URW_Date IS NULL THEN 0
            WHEN AST.INT_Date IS NOT NULL THEN DATEDIFF(DAY, AST.URW_Date, AST.INT_Date)
            ELSE DATEDIFF(DAY, AST.URW_Date, @Today)
        END as URW_Days,
        
        CASE 
            WHEN AST.INT_Date IS NULL THEN 0
            WHEN AST.RHM_Date IS NOT NULL THEN DATEDIFF(DAY, AST.INT_Date, AST.RHM_Date)
            WHEN AST.DEC_Date IS NOT NULL THEN DATEDIFF(DAY, AST.INT_Date, AST.DEC_Date)
            ELSE DATEDIFF(DAY, AST.INT_Date, @Today)
        END as INT_Days,
        
        CASE 
            WHEN AST.RHM_Date IS NULL THEN 0
            WHEN AST.DEC_Date IS NOT NULL THEN DATEDIFF(DAY, AST.RHM_Date, AST.DEC_Date)
            ELSE DATEDIFF(DAY, AST.RHM_Date, @Today)
        END as RHM_Days,
        
        CASE 
            WHEN AST.DEC_Date IS NULL THEN 0
            WHEN AST.NOA_Date IS NOT NULL THEN DATEDIFF(DAY, AST.DEC_Date, AST.NOA_Date)
            WHEN AST.OEX_Date IS NOT NULL THEN DATEDIFF(DAY, AST.DEC_Date, AST.OEX_Date)
            ELSE DATEDIFF(DAY, AST.DEC_Date, @Today)
        END as DEC_Days,
        
        CASE 
            WHEN AST.NOA_Date IS NULL THEN 0
            WHEN AST.OEX_Date IS NOT NULL THEN DATEDIFF(DAY, AST.NOA_Date, AST.OEX_Date)
            ELSE DATEDIFF(DAY, AST.NOA_Date, @Today)
        END as NOA_Days,
        
        CASE 
            WHEN AST.OEX_Date IS NULL THEN 0
            WHEN AST.ODC_Date IS NOT NULL THEN DATEDIFF(DAY, AST.OEX_Date, AST.ODC_Date)
            WHEN AST.HIR_Date IS NOT NULL THEN DATEDIFF(DAY, AST.OEX_Date, AST.HIR_Date)
            WHEN AST.WDR_Date IS NOT NULL THEN DATEDIFF(DAY, AST.OEX_Date, AST.WDR_Date)
            ELSE DATEDIFF(DAY, AST.OEX_Date, @Today)
        END as OEX_Days,
        
        CASE 
            WHEN AST.ODC_Date IS NULL THEN 0
            ELSE 0 -- ODC is final stage
        END as ODC_Days,
        
        CASE 
            WHEN AST.HIR_Date IS NULL THEN 0
            ELSE 0 -- HIR is final stage
        END as HIR_Days,
        
        CASE 
            WHEN AST.WDR_Date IS NULL THEN 0
            ELSE 0 -- WDR is final stage
        END as WDR_Days
        
    INTO #AdminStageTimeCalculations
    FROM #AdminStageTimings AST;
    
    -- Query 1: Timing Analytics by Requisition (Admin view - all users)
    SELECT 
        'ALL_USERS' as [User],
        ARB.RequisitionCode,
        ARB.Title,
        ARB.CreatedBy as RequisitionOwner,
        -- Average Time to Fill for this requisition (if filled)
        ISNULL(CEILING(AVG(CAST(ARB.TimeToFill as FLOAT))), 0) as TimeToFill_Days,
        -- Average Time to Hire for this requisition  
        ISNULL(CEILING(AVG(CAST(ATTH.TimeToHire as FLOAT))), 0) as TimeToHire_Days,
        -- Average time in each stage for this requisition (rounded up to next integer)
        ISNULL(CEILING(AVG(CAST(ASTC.PEN_Days as FLOAT))), 0) as PEN_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.REJ_Days as FLOAT))), 0) as REJ_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.HLD_Days as FLOAT))), 0) as HLD_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.PHN_Days as FLOAT))), 0) as PHN_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.URW_Days as FLOAT))), 0) as URW_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.INT_Days as FLOAT))), 0) as INT_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.RHM_Days as FLOAT))), 0) as RHM_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.DEC_Days as FLOAT))), 0) as DEC_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.NOA_Days as FLOAT))), 0) as NOA_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.OEX_Days as FLOAT))), 0) as OEX_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.ODC_Days as FLOAT))), 0) as ODC_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.HIR_Days as FLOAT))), 0) as HIR_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.WDR_Days as FLOAT))), 0) as WDR_Days,
        COUNT(DISTINCT CONCAT(ASTC.RequisitionId, '-', ASTC.CandidateId)) as TotalCandidates
    FROM #AdminRequisitionBase ARB
    LEFT JOIN #AdminTimeToHireCalc ATTH ON ARB.RequisitionId = ATTH.RequisitionId
    LEFT JOIN #AdminStageTimeCalculations ASTC ON ARB.RequisitionId = ASTC.RequisitionId
    GROUP BY ARB.RequisitionCode, ARB.Title, ARB.CreatedBy
    ORDER BY ARB.RequisitionCode
    FOR JSON PATH;
    
    -- Query 2: Timing Analytics by Company (Admin view - all users)
    SELECT 
        'ALL_USERS' as [User],
        ARB.CompanyName,
        -- Average Time to Fill for this company (if filled)
        ISNULL(CEILING(AVG(CAST(ARB.TimeToFill as FLOAT))), 0) as TimeToFill_Days,
        -- Average Time to Hire for this company  
        ISNULL(CEILING(AVG(CAST(ATTH.TimeToHire as FLOAT))), 0) as TimeToHire_Days,
        -- Average time in each stage for this company (rounded up to next integer)
        ISNULL(CEILING(AVG(CAST(ASTC.PEN_Days as FLOAT))), 0) as PEN_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.REJ_Days as FLOAT))), 0) as REJ_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.HLD_Days as FLOAT))), 0) as HLD_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.PHN_Days as FLOAT))), 0) as PHN_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.URW_Days as FLOAT))), 0) as URW_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.INT_Days as FLOAT))), 0) as INT_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.RHM_Days as FLOAT))), 0) as RHM_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.DEC_Days as FLOAT))), 0) as DEC_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.NOA_Days as FLOAT))), 0) as NOA_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.OEX_Days as FLOAT))), 0) as OEX_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.ODC_Days as FLOAT))), 0) as ODC_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.HIR_Days as FLOAT))), 0) as HIR_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.WDR_Days as FLOAT))), 0) as WDR_Days,
        COUNT(DISTINCT CONCAT(ASTC.RequisitionId, '-', ASTC.CandidateId)) as TotalCandidates,
        COUNT(DISTINCT ARB.RequisitionId) as TotalRequisitions,
        COUNT(DISTINCT ARB.CreatedBy) as TotalRecruiters
    FROM #AdminRequisitionBase ARB
    LEFT JOIN #AdminTimeToHireCalc ATTH ON ARB.RequisitionId = ATTH.RequisitionId
    LEFT JOIN #AdminStageTimeCalculations ASTC ON ARB.RequisitionId = ASTC.RequisitionId
    GROUP BY ARB.CompanyName
    ORDER BY ARB.CompanyName
    FOR JSON PATH;

    -- Clean up admin timing temp tables
    DROP TABLE IF EXISTS #AdminRequisitionBase;
    DROP TABLE IF EXISTS #AdminFirstSubmissions;
    DROP TABLE IF EXISTS #AdminTimeToHireCalc;
    DROP TABLE IF EXISTS #AdminStageTimings;
    DROP TABLE IF EXISTS #AdminStageTimeCalculations;
    
    -- Clean up existing temp tables (from current DashboardAdmin)
    DROP TABLE IF EXISTS #RequisitionMetrics;
    DROP TABLE IF EXISTS #SubmissionMetrics;    
END

GO

ALTER PROCEDURE [dbo].[DashboardRecruiters]
    @User VARCHAR(10) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;
    
    --DECLARE @User varchar(10);
    -- Set default user if not provided
    IF @User IS NULL
        SET @User = 'KEVIN';
    
    -- Generic Query for Dropdown
    SELECT UserName as KeyValue, FirstName + ' ' + LastName as Text
    FROM dbo.Users 
    WHERE UserName = @User
    FOR JSON AUTO;
    
    -- Common date variables
    DECLARE @Today DATE = CAST(GETDATE() as date);
    DECLARE @YearStart DATE = DATEFROMPARTS(YEAR(@Today), 1, 1);
    
    -- Get the earliest submission date for boundary checking
    DECLARE @MinDate DATE = (SELECT MIN(CAST(CreatedDate AS DATE)) FROM Submissions WHERE CreatedBy = @User);
    
    -- Adjust YearStart if needed based on MinDate
    SET @YearStart = CASE 
        WHEN @MinDate IS NULL OR @MinDate > @YearStart THEN @MinDate
        ELSE @YearStart
    END;
    
    -- Common date calculations
    DECLARE @StartDate7D DATE = DATEADD(DAY, -6, @Today);
    DECLARE @StartDateMTD DATE = DATEADD(DAY, 1, EOMONTH(@Today, -1));
    DECLARE @StartDateQTD DATE = DATEADD(QUARTER, DATEDIFF(QUARTER, 0, @Today), 0);
    DECLARE @StartDateHYTD DATE = CASE 
        WHEN MONTH(@Today) <= 6 THEN DATEFROMPARTS(YEAR(@Today), 1, 1)
        ELSE DATEFROMPARTS(YEAR(@Today), 7, 1)
    END;
    
    -- =====================================================
    -- SET 1: Submission Pipeline Metrics (Cached with Temp Tables)
    -- =====================================================
    
    -- Create temp table for base submission/requisition data
    DROP TABLE IF EXISTS #BaseMetrics;
    
    SELECT 
        @User as [User],
        -- Submission counts
        ISNULL(SUM(CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDate7D AND @Today THEN 1 ELSE 0 END), 0) as SUB_LAST7D_COUNT,
        ISNULL(SUM(CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today THEN 1 ELSE 0 END), 0) as SUB_MTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today THEN 1 ELSE 0 END), 0) as SUB_QTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today THEN 1 ELSE 0 END), 0) as SUB_HYTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @YearStart AND @Today THEN 1 ELSE 0 END), 0) as SUB_YTD_COUNT,
        -- Active requisition counts
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDate7D AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN S.RequisitionId END), 0) as REQ_LAST7D_COUNT,
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN S.RequisitionId END), 0) as REQ_MTD_COUNT,
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN S.RequisitionId END), 0) as REQ_QTD_COUNT,
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN S.RequisitionId END), 0) as REQ_HYTD_COUNT,
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @YearStart AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN S.RequisitionId END), 0) as REQ_YTD_COUNT
    INTO #BaseMetrics
    FROM (SELECT 1 as dummy) d -- Ensures we always have at least one row
    LEFT JOIN Submissions S ON S.CreatedBy = @User
    LEFT JOIN Requisitions R ON S.RequisitionId = R.Id;
    
    -- Query 1: Total Submissions by Recruiter
    SELECT [User], SUB_LAST7D_COUNT as LAST7D_COUNT, SUB_MTD_COUNT as MTD_COUNT, SUB_QTD_COUNT as QTD_COUNT, SUB_HYTD_COUNT as HYTD_COUNT, SUB_YTD_COUNT as YTD_COUNT
    FROM #BaseMetrics FOR JSON PATH;--, WITHOUT_ARRAY_WRAPPER;
    
    -- Query 2: Active Requisitions Worked On
    SELECT [User], REQ_LAST7D_COUNT as LAST7D_COUNT, REQ_MTD_COUNT as MTD_COUNT, REQ_QTD_COUNT as QTD_COUNT, REQ_HYTD_COUNT as HYTD_COUNT, REQ_YTD_COUNT as YTD_COUNT
    FROM #BaseMetrics FOR JSON PATH;--, WITHOUT_ARRAY_WRAPPER;
    
    -- Create temp table for status-based queries (3-6)
    DROP TABLE IF EXISTS #RecruiterSubmissions;
    
    WITH FirstSubmissions AS (
        SELECT 
            RequisitionId,
            CandidateId,
            MIN(CreatedDate) as FirstSubmissionDate
        FROM Submissions
        GROUP BY RequisitionId, CandidateId
        HAVING MIN(CASE WHEN CreatedBy = @User THEN CreatedDate END) = MIN(CreatedDate)
    )
    SELECT 
        FS.RequisitionId,
        FS.CandidateId,
        S.Status,
        MAX(S.CreatedDate) as LatestStatusDate
    INTO #RecruiterSubmissions
    FROM FirstSubmissions FS
    INNER JOIN Submissions S 
        ON FS.RequisitionId = S.RequisitionId 
        AND FS.CandidateId = S.CandidateId
        AND S.Status IN ('INT', 'OEX', 'HIR')
    GROUP BY FS.RequisitionId, FS.CandidateId, S.Status;
    
    -- Create temp table for status-based data to avoid repetition
    DROP TABLE IF EXISTS #StatusMetrics;
    
    SELECT 
        @User as [User],
        -- Interview counts
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND RS.Status = 'INT' THEN 1 ELSE 0 END), 0) as INT_LAST7D_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND RS.Status = 'INT' THEN 1 ELSE 0 END), 0) as INT_MTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND RS.Status = 'INT' THEN 1 ELSE 0 END), 0) as INT_QTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND RS.Status = 'INT' THEN 1 ELSE 0 END), 0) as INT_HYTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @YearStart AND @Today AND RS.Status = 'INT' THEN 1 ELSE 0 END), 0) as INT_YTD_COUNT,
        -- Offer counts
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND RS.Status = 'OEX' THEN 1 ELSE 0 END), 0) as OEX_LAST7D_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND RS.Status = 'OEX' THEN 1 ELSE 0 END), 0) as OEX_MTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND RS.Status = 'OEX' THEN 1 ELSE 0 END), 0) as OEX_QTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND RS.Status = 'OEX' THEN 1 ELSE 0 END), 0) as OEX_HYTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @YearStart AND @Today AND RS.Status = 'OEX' THEN 1 ELSE 0 END), 0) as OEX_YTD_COUNT,
        -- Hire counts
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND RS.Status = 'HIR' THEN 1 ELSE 0 END), 0) as HIR_LAST7D_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND RS.Status = 'HIR' THEN 1 ELSE 0 END), 0) as HIR_MTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND RS.Status = 'HIR' THEN 1 ELSE 0 END), 0) as HIR_QTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND RS.Status = 'HIR' THEN 1 ELSE 0 END), 0) as HIR_HYTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @YearStart AND @Today AND RS.Status = 'HIR' THEN 1 ELSE 0 END), 0) as HIR_YTD_COUNT
    INTO #StatusMetrics
    FROM (SELECT 1 as dummy) d -- Ensures we always have at least one row
    LEFT JOIN #RecruiterSubmissions RS ON 1=1;
    
    -- Query 3: Interview Status
    SELECT [User], INT_LAST7D_COUNT as LAST7D_COUNT, INT_MTD_COUNT as MTD_COUNT, INT_QTD_COUNT as QTD_COUNT, INT_HYTD_COUNT as HYTD_COUNT, INT_YTD_COUNT as YTD_COUNT
    FROM #StatusMetrics FOR JSON PATH;--, WITHOUT_ARRAY_WRAPPER;
    
    -- Query 4: Offer Extended Status
    SELECT [User], OEX_LAST7D_COUNT as LAST7D_COUNT, OEX_MTD_COUNT as MTD_COUNT, OEX_QTD_COUNT as QTD_COUNT, OEX_HYTD_COUNT as HYTD_COUNT, OEX_YTD_COUNT as YTD_COUNT
    FROM #StatusMetrics FOR JSON PATH;--, WITHOUT_ARRAY_WRAPPER;
    
    -- Query 5: Hired Status
    SELECT [User], HIR_LAST7D_COUNT as LAST7D_COUNT, HIR_MTD_COUNT as MTD_COUNT, HIR_QTD_COUNT as QTD_COUNT, HIR_HYTD_COUNT as HYTD_COUNT, HIR_YTD_COUNT as YTD_COUNT
    FROM #StatusMetrics FOR JSON PATH;--, WITHOUT_ARRAY_WRAPPER;
    
    -- Query 6: OEX to HIR Conversion Ratio
    SELECT 
        [User],
        CASE WHEN OEX_LAST7D_COUNT = 0 THEN 0.00 ELSE CAST(HIR_LAST7D_COUNT AS DECIMAL(10,2)) / CAST(OEX_LAST7D_COUNT AS DECIMAL(10,2)) END as LAST7D_RATIO,
        CASE WHEN OEX_MTD_COUNT = 0 THEN 0.00 ELSE CAST(HIR_MTD_COUNT AS DECIMAL(10,2)) / CAST(OEX_MTD_COUNT AS DECIMAL(10,2)) END as MTD_RATIO,
        CASE WHEN OEX_QTD_COUNT = 0 THEN 0.00 ELSE CAST(HIR_QTD_COUNT AS DECIMAL(10,2)) / CAST(OEX_QTD_COUNT AS DECIMAL(10,2)) END as QTD_RATIO,
        CASE WHEN OEX_HYTD_COUNT = 0 THEN 0.00 ELSE CAST(HIR_HYTD_COUNT AS DECIMAL(10,2)) / CAST(OEX_HYTD_COUNT AS DECIMAL(10,2)) END as HYTD_RATIO,
        CASE WHEN OEX_YTD_COUNT = 0 THEN 0.00 ELSE CAST(HIR_YTD_COUNT AS DECIMAL(10,2)) / CAST(OEX_YTD_COUNT AS DECIMAL(10,2)) END as YTD_RATIO
    FROM #StatusMetrics FOR JSON PATH;--, WITHOUT_ARRAY_WRAPPER;
    
    DROP TABLE #RecruiterSubmissions;
    DROP TABLE #BaseMetrics;
    DROP TABLE #StatusMetrics;
    
    -- =====================================================
    -- SET 2: Recent Activity Detail
    -- =====================================================
    
    -- Get date range (last 30 days but not before year start)
    DECLARE @StartDate DATE = CASE 
        WHEN DATEADD(DAY, -30, @Today) < @YearStart THEN @YearStart
        ELSE DATEADD(DAY, -30, @Today)
    END;
    DECLARE @return varchar(max);
   
    WITH FirstSubmissions AS (
        -- Get submissions where user made the first submission
        SELECT 
            RequisitionId,
            CandidateId,
            MIN(CreatedDate) as FirstSubmissionDate
        FROM Submissions
        GROUP BY RequisitionId, CandidateId
        HAVING MIN(CASE WHEN CreatedBy = @User THEN CreatedDate END) = MIN(CreatedDate)
            AND MIN(CreatedDate) >= @StartDate
    ),
    LatestActivity AS (
        -- Get the latest activity for each req+candidate combo
        SELECT 
            FS.RequisitionId,
            FS.CandidateId,
            FS.FirstSubmissionDate,
            S.Status as CurrentStatus,
            S.CreatedDate as LastActivityDate,
            S.Notes as ActivityNotes
        FROM FirstSubmissions FS
        INNER JOIN Submissions S
            ON FS.RequisitionId = S.RequisitionId 
            AND FS.CandidateId = S.CandidateId
        WHERE S.CreatedDate = (
            SELECT MAX(S2.CreatedDate)
            FROM Submissions S2
            WHERE S2.RequisitionId = S.RequisitionId
                AND S2.CandidateId = S.CandidateId
        )
    ),
    CompanyCounts AS (
        -- Count submissions per company for sorting
        SELECT 
            R.CompanyId,
            COUNT(*) as SubmissionCount
        FROM LatestActivity LA
        INNER JOIN Requisitions R ON LA.RequisitionId = R.Id
        GROUP BY R.CompanyId
    )

    SELECT @return =
    (SELECT 
        TRIM(C.CompanyName + ' - [' + R.Code + '] - ' + CAST(R.Positions as varchar(5)) + ' Positions - ' + R.PosTitle) as [Company],
        LTRIM(RTRIM(ISNULL(CAND.FirstName, '') + ' ' + ISNULL(CAND.LastName, ''))) as [CandidateName],
        LA.CurrentStatus as [CurrentStatus],
        CAST(LA.FirstSubmissionDate AS DATE) as [DateFirstSubmitted],
        CAST(LA.LastActivityDate AS DATE) as [LastActivityDate],
        ISNULL(LA.ActivityNotes, '') as [ActivityNotes],
        @User as [User]
    FROM LatestActivity LA
    INNER JOIN Requisitions R ON LA.RequisitionId = R.Id
    INNER JOIN Companies C ON R.CompanyId = C.ID
    INNER JOIN Candidate CAND ON LA.CandidateId = CAND.ID
    INNER JOIN CompanyCounts CC ON R.CompanyId = CC.CompanyId
    ORDER BY 
        CC.SubmissionCount DESC,  -- Higher submission count first
        C.CompanyName ASC,        -- Alphabetically for ties
        R.Id ASC,                 -- RequisitionId within company
        LA.FirstSubmissionDate ASC
    FOR JSON PATH); -- First submission date
    
    SELECT @return;

    -- =====================================================
    -- SET 3: Placement Reporting
    -- =====================================================
    
    -- Get date range (last 3 months but not before year start)
    DECLARE @HireStartDate DATE = CASE 
        WHEN DATEADD(MONTH, -3, @Today) < @YearStart THEN @YearStart
        ELSE DATEADD(MONTH, -3, @Today)
    END;
    
    WITH FirstSubmissions AS (
        -- Get submissions where user made the first submission
        SELECT 
            RequisitionId,
            CandidateId,
            MIN(CreatedDate) as FirstSubmissionDate
        FROM Submissions
        GROUP BY RequisitionId, CandidateId
        HAVING MIN(CASE WHEN CreatedBy = @User THEN CreatedDate END) = MIN(CreatedDate)
    ),
    HiredCandidates AS (
        -- Get candidates with HIR status in last 3 months
        SELECT 
            FS.RequisitionId,
            FS.CandidateId,
            MAX(S.CreatedDate) as HireDate
        FROM FirstSubmissions FS
        INNER JOIN Submissions S
            ON FS.RequisitionId = S.RequisitionId 
            AND FS.CandidateId = S.CandidateId
            AND S.Status = 'HIR'
        WHERE CAST(S.CreatedDate AS DATE) >= @HireStartDate
        GROUP BY FS.RequisitionId, FS.CandidateId
    )

    SELECT @return = 
    (SELECT 
        C.CompanyName as Company,
        R.Code as [RequisitionNumber],
        R.Positions as [NumPosition],
        R.PosTitle as Title,
        CONCAT(CAND.FirstName, ' ', CAND.LastName) as [CandidateName],
        CAST(HC.HireDate AS DATE) as [DateHired],
        CAST(0.00 AS DECIMAL(9,2)) as [SalaryOffered],
        CAST(GETDATE() AS DATE) as [StartDate],
        CAST(GETDATE() AS DATE) as [DateInvoiced],
        CAST(GETDATE() AS DATE) as [DatePaid],
        CAST(0.00 AS DECIMAL(9,2)) as [Placementfee],
        CAST(0.00 AS DECIMAL(5,2)) as [CommissionPercent],
        CAST(0.00 AS DECIMAL(9,2)) as [CommissionEarned],
        @User [User]
    FROM HiredCandidates HC
    INNER JOIN Requisitions R ON HC.RequisitionId = R.Id
    INNER JOIN Companies C ON R.CompanyId = C.ID
    INNER JOIN Candidate CAND ON HC.CandidateId = CAND.ID
    ORDER BY 
        C.CompanyName ASC,
        R.Id ASC,
        HC.HireDate DESC
    FOR JSON PATH);
    
    SELECT @return;

    /***************************************************************************
    * RESULT SET 4: RECRUITER TIMING ANALYTICS (2 Queries)
    * Query 1: Time to Fill, Time to Hire, and Time in Stage metrics (By Requisition) 
    * Query 2: Time to Fill, Time to Hire, and Time in Stage metrics (By Company)
    * Shows data for SPECIFIC USER submissions only (Recruiter perspective)
    ***************************************************************************/
    
    -- Get requisitions from last year where this user made submissions
    DECLARE @StartDate365 DATE = DATEADD(YEAR, -1, @Today);
    
    -- Create temp tables for recruiter timing analytics
    DROP TABLE IF EXISTS #RecruiterRequisitionBase;
    DROP TABLE IF EXISTS #RecruiterFirstSubmissions;
    DROP TABLE IF EXISTS #RecruiterTimeToHireCalc;
    DROP TABLE IF EXISTS #RecruiterStageTimings;
    DROP TABLE IF EXISTS #RecruiterStageTimeCalculations;
    
    -- Temp table 1: Get requisitions where this user made submissions in last year
    SELECT DISTINCT
        R.Id as RequisitionId,
        R.Code as RequisitionCode,
        ISNULL(C.CompanyName, 'Unknown Company') as CompanyName,
        R.PosTitle as Title,
        R.CreatedDate as RequisitionCreatedDate,
        S.CreatedBy as SubmissionCreatedBy,
        -- Time to Fill calculation (only for FUL status)
        CASE 
            WHEN R.Status = 'FUL' THEN DATEDIFF(DAY, R.CreatedDate, R.UpdatedDate)
            ELSE NULL 
        END as TimeToFill
    INTO #RecruiterRequisitionBase
    FROM Submissions S
    INNER JOIN Requisitions R ON S.RequisitionId = R.Id
    LEFT JOIN Companies C ON R.CompanyId = C.ID
    WHERE S.CreatedBy = @User 
        AND CAST(S.CreatedDate AS DATE) >= @StartDate365;
    
    -- Temp table 2: Get first submission date for each candidate+requisition combo by this user
    SELECT 
        S.RequisitionId,
        S.CandidateId,
        MIN(S.CreatedDate) as FirstSubmissionDate
    INTO #RecruiterFirstSubmissions
    FROM Submissions S
    INNER JOIN #RecruiterRequisitionBase RRB ON S.RequisitionId = RRB.RequisitionId
    WHERE S.CreatedBy = @User
    GROUP BY S.RequisitionId, S.CandidateId;
    
    -- Temp table 3: Calculate Time to Hire for candidates submitted by this user
    SELECT 
        RFS.RequisitionId,
        RFS.CandidateId,
        RFS.FirstSubmissionDate,
        MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HireDate,
        MAX(CASE WHEN S.Status = 'HIR' THEN DATEDIFF(DAY, RFS.FirstSubmissionDate, S.CreatedDate) END) as TimeToHire
    INTO #RecruiterTimeToHireCalc
    FROM #RecruiterFirstSubmissions RFS
    INNER JOIN Submissions S ON RFS.RequisitionId = S.RequisitionId AND RFS.CandidateId = S.CandidateId
    WHERE S.CreatedBy = @User
    GROUP BY RFS.RequisitionId, RFS.CandidateId, RFS.FirstSubmissionDate;
    
    -- Temp table 4: Calculate time in each stage for candidates submitted by this user
    SELECT 
        RFS.RequisitionId,
        RFS.CandidateId,
        -- Get the latest date for each stage (in case of multiple records for same stage)
        MAX(CASE WHEN S.Status = 'PEN' THEN S.CreatedDate END) as PEN_Date,
        MAX(CASE WHEN S.Status = 'REJ' THEN S.CreatedDate END) as REJ_Date,
        MAX(CASE WHEN S.Status = 'HLD' THEN S.CreatedDate END) as HLD_Date,
        MAX(CASE WHEN S.Status = 'PHN' THEN S.CreatedDate END) as PHN_Date,
        MAX(CASE WHEN S.Status = 'URW' THEN S.CreatedDate END) as URW_Date,
        MAX(CASE WHEN S.Status = 'INT' THEN S.CreatedDate END) as INT_Date,
        MAX(CASE WHEN S.Status = 'RHM' THEN S.CreatedDate END) as RHM_Date,
        MAX(CASE WHEN S.Status = 'DEC' THEN S.CreatedDate END) as DEC_Date,
        MAX(CASE WHEN S.Status = 'NOA' THEN S.CreatedDate END) as NOA_Date,
        MAX(CASE WHEN S.Status = 'OEX' THEN S.CreatedDate END) as OEX_Date,
        MAX(CASE WHEN S.Status = 'ODC' THEN S.CreatedDate END) as ODC_Date,
        MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HIR_Date,
        MAX(CASE WHEN S.Status = 'WDR' THEN S.CreatedDate END) as WDR_Date
    INTO #RecruiterStageTimings
    FROM #RecruiterFirstSubmissions RFS
    INNER JOIN Submissions S ON RFS.RequisitionId = S.RequisitionId AND RFS.CandidateId = S.CandidateId
    WHERE S.CreatedBy = @User
    GROUP BY RFS.RequisitionId, RFS.CandidateId;
    
    -- Temp table 5: Calculate time spent in each stage for this user's candidates
    SELECT 
        RST.RequisitionId,
        RST.CandidateId,
        
        CASE 
            WHEN RST.PEN_Date IS NULL THEN 0
            WHEN RST.REJ_Date IS NOT NULL THEN DATEDIFF(DAY, RST.PEN_Date, RST.REJ_Date)
            WHEN RST.HLD_Date IS NOT NULL THEN DATEDIFF(DAY, RST.PEN_Date, RST.HLD_Date)
            WHEN RST.PHN_Date IS NOT NULL THEN DATEDIFF(DAY, RST.PEN_Date, RST.PHN_Date)
            ELSE DATEDIFF(DAY, RST.PEN_Date, @Today)
        END as PEN_Days,
        
        CASE 
            WHEN RST.REJ_Date IS NULL THEN 0
            ELSE 0 -- REJ is final stage
        END as REJ_Days,
        
        CASE 
            WHEN RST.HLD_Date IS NULL THEN 0
            WHEN RST.PHN_Date IS NOT NULL THEN DATEDIFF(DAY, RST.HLD_Date, RST.PHN_Date)
            WHEN RST.URW_Date IS NOT NULL THEN DATEDIFF(DAY, RST.HLD_Date, RST.URW_Date)
            ELSE DATEDIFF(DAY, RST.HLD_Date, @Today)
        END as HLD_Days,
        
        CASE 
            WHEN RST.PHN_Date IS NULL THEN 0
            WHEN RST.URW_Date IS NOT NULL THEN DATEDIFF(DAY, RST.PHN_Date, RST.URW_Date)
            WHEN RST.INT_Date IS NOT NULL THEN DATEDIFF(DAY, RST.PHN_Date, RST.INT_Date)
            ELSE DATEDIFF(DAY, RST.PHN_Date, @Today)
        END as PHN_Days,
        
        CASE 
            WHEN RST.URW_Date IS NULL THEN 0
            WHEN RST.INT_Date IS NOT NULL THEN DATEDIFF(DAY, RST.URW_Date, RST.INT_Date)
            ELSE DATEDIFF(DAY, RST.URW_Date, @Today)
        END as URW_Days,
        
        CASE 
            WHEN RST.INT_Date IS NULL THEN 0
            WHEN RST.RHM_Date IS NOT NULL THEN DATEDIFF(DAY, RST.INT_Date, RST.RHM_Date)
            WHEN RST.DEC_Date IS NOT NULL THEN DATEDIFF(DAY, RST.INT_Date, RST.DEC_Date)
            ELSE DATEDIFF(DAY, RST.INT_Date, @Today)
        END as INT_Days,
        
        CASE 
            WHEN RST.RHM_Date IS NULL THEN 0
            WHEN RST.DEC_Date IS NOT NULL THEN DATEDIFF(DAY, RST.RHM_Date, RST.DEC_Date)
            ELSE DATEDIFF(DAY, RST.RHM_Date, @Today)
        END as RHM_Days,
        
        CASE 
            WHEN RST.DEC_Date IS NULL THEN 0
            WHEN RST.NOA_Date IS NOT NULL THEN DATEDIFF(DAY, RST.DEC_Date, RST.NOA_Date)
            WHEN RST.OEX_Date IS NOT NULL THEN DATEDIFF(DAY, RST.DEC_Date, RST.OEX_Date)
            ELSE DATEDIFF(DAY, RST.DEC_Date, @Today)
        END as DEC_Days,
        
        CASE 
            WHEN RST.NOA_Date IS NULL THEN 0
            WHEN RST.OEX_Date IS NOT NULL THEN DATEDIFF(DAY, RST.NOA_Date, RST.OEX_Date)
            ELSE DATEDIFF(DAY, RST.NOA_Date, @Today)
        END as NOA_Days,
        
        CASE 
            WHEN RST.OEX_Date IS NULL THEN 0
            WHEN RST.ODC_Date IS NOT NULL THEN DATEDIFF(DAY, RST.OEX_Date, RST.ODC_Date)
            WHEN RST.HIR_Date IS NOT NULL THEN DATEDIFF(DAY, RST.OEX_Date, RST.HIR_Date)
            WHEN RST.WDR_Date IS NOT NULL THEN DATEDIFF(DAY, RST.OEX_Date, RST.WDR_Date)
            ELSE DATEDIFF(DAY, RST.OEX_Date, @Today)
        END as OEX_Days,
        
        CASE 
            WHEN RST.ODC_Date IS NULL THEN 0
            ELSE 0 -- ODC is final stage
        END as ODC_Days,
        
        CASE 
            WHEN RST.HIR_Date IS NULL THEN 0
            ELSE 0 -- HIR is final stage
        END as HIR_Days,
        
        CASE 
            WHEN RST.WDR_Date IS NULL THEN 0
            ELSE 0 -- WDR is final stage
        END as WDR_Days
        
    INTO #RecruiterStageTimeCalculations
    FROM #RecruiterStageTimings RST;
    
    -- Query 1: Timing Analytics by Requisition (Recruiter view - user filtered)
    SELECT 
        @User as [User],
        RRB.RequisitionCode,
        RRB.Title,
        -- Average Time to Fill for this requisition (if filled)
        ISNULL(CEILING(AVG(CAST(RRB.TimeToFill as FLOAT))), 0) as TimeToFill_Days,
        -- Average Time to Hire for this requisition  
        ISNULL(CEILING(AVG(CAST(RTTH.TimeToHire as FLOAT))), 0) as TimeToHire_Days,
        -- Average time in each stage for this requisition (rounded up to next integer)
        ISNULL(CEILING(AVG(CAST(RSTC.PEN_Days as FLOAT))), 0) as PEN_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.REJ_Days as FLOAT))), 0) as REJ_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.HLD_Days as FLOAT))), 0) as HLD_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.PHN_Days as FLOAT))), 0) as PHN_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.URW_Days as FLOAT))), 0) as URW_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.INT_Days as FLOAT))), 0) as INT_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.RHM_Days as FLOAT))), 0) as RHM_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.DEC_Days as FLOAT))), 0) as DEC_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.NOA_Days as FLOAT))), 0) as NOA_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.OEX_Days as FLOAT))), 0) as OEX_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.ODC_Days as FLOAT))), 0) as ODC_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.HIR_Days as FLOAT))), 0) as HIR_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.WDR_Days as FLOAT))), 0) as WDR_Days,
        COUNT(DISTINCT CONCAT(RSTC.RequisitionId, '-', RSTC.CandidateId)) as TotalCandidates
    FROM #RecruiterRequisitionBase RRB
    LEFT JOIN #RecruiterTimeToHireCalc RTTH ON RRB.RequisitionId = RTTH.RequisitionId
    LEFT JOIN #RecruiterStageTimeCalculations RSTC ON RRB.RequisitionId = RSTC.RequisitionId
    GROUP BY RRB.RequisitionCode, RRB.Title
    ORDER BY RRB.RequisitionCode
    FOR JSON PATH;
    
    -- Query 2: Timing Analytics by Company (Recruiter view - user filtered)
    SELECT 
        @User as [User],
        RRB.CompanyName,
        -- Average Time to Fill for this company (if filled)
        ISNULL(CEILING(AVG(CAST(RRB.TimeToFill as FLOAT))), 0) as TimeToFill_Days,
        -- Average Time to Hire for this company  
        ISNULL(CEILING(AVG(CAST(RTTH.TimeToHire as FLOAT))), 0) as TimeToHire_Days,
        -- Average time in each stage for this company (rounded up to next integer)
        ISNULL(CEILING(AVG(CAST(RSTC.PEN_Days as FLOAT))), 0) as PEN_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.REJ_Days as FLOAT))), 0) as REJ_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.HLD_Days as FLOAT))), 0) as HLD_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.PHN_Days as FLOAT))), 0) as PHN_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.URW_Days as FLOAT))), 0) as URW_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.INT_Days as FLOAT))), 0) as INT_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.RHM_Days as FLOAT))), 0) as RHM_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.DEC_Days as FLOAT))), 0) as DEC_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.NOA_Days as FLOAT))), 0) as NOA_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.OEX_Days as FLOAT))), 0) as OEX_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.ODC_Days as FLOAT))), 0) as ODC_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.HIR_Days as FLOAT))), 0) as HIR_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.WDR_Days as FLOAT))), 0) as WDR_Days,
        COUNT(DISTINCT CONCAT(RSTC.RequisitionId, '-', RSTC.CandidateId)) as TotalCandidates,
        COUNT(DISTINCT RRB.RequisitionId) as TotalRequisitions
    FROM #RecruiterRequisitionBase RRB
    LEFT JOIN #RecruiterTimeToHireCalc RTTH ON RRB.RequisitionId = RTTH.RequisitionId
    LEFT JOIN #RecruiterStageTimeCalculations RSTC ON RRB.RequisitionId = RSTC.RequisitionId
    GROUP BY RRB.CompanyName
    ORDER BY RRB.CompanyName
    FOR JSON PATH;

    -- Clean up recruiter timing temp tables
    DROP TABLE IF EXISTS #RecruiterRequisitionBase;
    DROP TABLE IF EXISTS #RecruiterFirstSubmissions;
    DROP TABLE IF EXISTS #RecruiterTimeToHireCalc;
    DROP TABLE IF EXISTS #RecruiterStageTimings;
    DROP TABLE IF EXISTS #RecruiterStageTimeCalculations;
    
    -- Clean up existing temp tables (from current DashboardRecruiters)
    DROP TABLE IF EXISTS #BaseMetrics;
    DROP TABLE IF EXISTS #RecruiterSubmissionMetrics;

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
            SELECT UserName, 
                   -- Adjust YearStart per user based on their earliest submission
                   CASE 
                       WHEN (SELECT MIN(CAST(CreatedDate AS DATE)) FROM Submissions WHERE CreatedBy = UserName) IS NULL 
                            OR (SELECT MIN(CAST(CreatedDate AS DATE)) FROM Submissions WHERE CreatedBy = UserName) > @YearStart 
                       THEN ISNULL((SELECT MIN(CAST(CreatedDate AS DATE)) FROM Submissions WHERE CreatedBy = UserName), @YearStart)
                       ELSE @YearStart
                   END as UserYearStart
            FROM dbo.Users 
            WHERE Role = 4 AND Status = 1
        ),
        BaseMetrics AS (
            -- Replicate #BaseMetrics temp table logic for all users
            SELECT 
                U.UserName as [User],
                U.UserYearStart,
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
            FROM AllRecruiters U
            LEFT JOIN Submissions S ON S.CreatedBy = U.UserName
            LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
            GROUP BY U.UserName, U.UserYearStart
        ),
        StatusMetrics AS (
            -- Replicate #StatusMetrics logic for all users
            SELECT 
                U.UserName as [User],
                U.UserYearStart,
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
                    FS.RequisitionId,
                    FS.CandidateId,
                    FS.UserName,
                    S.Status,
                    MAX(S.CreatedDate) as LatestStatusDate
                FROM (
                    SELECT 
                        S.RequisitionId,
                        S.CandidateId,
                        U2.UserName,
                        MIN(S.CreatedDate) as FirstSubmissionDate
                    FROM Submissions S
                    CROSS JOIN AllRecruiters U2
                    WHERE S.CreatedBy = U2.UserName
                    GROUP BY S.RequisitionId, S.CandidateId, U2.UserName
                    HAVING MIN(CASE WHEN S.CreatedBy = U2.UserName THEN S.CreatedDate END) = MIN(S.CreatedDate)
                ) FS
                INNER JOIN Submissions S 
                    ON FS.RequisitionId = S.RequisitionId 
                    AND FS.CandidateId = S.CandidateId
                    AND S.Status IN ('INT', 'OEX', 'HIR')
                GROUP BY FS.RequisitionId, FS.CandidateId, FS.UserName, S.Status
            ) RS ON RS.UserName = U.UserName
            GROUP BY U.UserName, U.UserYearStart
        )
        
        -- Generate all period rows for all users
         INSERT INTO [dbo].[SubmissionMetricsView] 
        ([CreatedBy], [MetricDate], [Period], [INT_Count], [OEX_Count], [HIR_Count], [Total_Submissions], [OEX_HIR_Ratio], [Requisitions_Created], [Active_Requisitions_Updated], [RefreshDate])
        
       SELECT 
            BM.[User] as CreatedBy,
            @Today as MetricDate,
            P.Period,
            
            -- Status counts by period
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
            
            -- Total submissions
            CASE P.Period
                WHEN '7D' THEN BM.SUB_LAST7D_COUNT
                WHEN 'MTD' THEN BM.SUB_MTD_COUNT
                WHEN 'QTD' THEN BM.SUB_QTD_COUNT
                WHEN 'HYTD' THEN BM.SUB_HYTD_COUNT
                WHEN 'YTD' THEN BM.SUB_YTD_COUNT
            END as Total_Submissions,
            
            -- OEX:HIR Ratio
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
            
            -- Requisitions created (usually 0 for recruiters)
            0 as Requisitions_Created,
            
            -- Active requisitions worked on
            CASE P.Period
                WHEN '7D' THEN BM.REQ_LAST7D_COUNT
                WHEN 'MTD' THEN BM.REQ_MTD_COUNT
                WHEN 'QTD' THEN BM.REQ_QTD_COUNT
                WHEN 'HYTD' THEN BM.REQ_HYTD_COUNT
                WHEN 'YTD' THEN BM.REQ_YTD_COUNT
            END as Active_Requisitions_Updated,
            
            GETDATE() as RefreshDate
            
        FROM BaseMetrics BM
        CROSS JOIN (VALUES ('7D'), ('MTD'), ('QTD'), ('HYTD'), ('YTD')) P(Period)
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
        DECLARE @StartDateHYTD DATE = CASE 
            WHEN MONTH(@Today) <= 6 THEN DATEFROMPARTS(YEAR(@Today), 1, 1)
            ELSE DATEFROMPARTS(YEAR(@Today), 7, 1)
        END;
        
        -- Single INSERT using JOINs for all accounts managers and all periods
        WITH AllAccountsManagers AS (
            SELECT UserName
            FROM dbo.Users 
            WHERE Role IN (5, 6) AND Status = 1  -- Role 5 = AM, Role 6 = Full Desk
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
            FROM AllAccountsManagers U
            LEFT JOIN RequisitionView R ON R.CreatedBy = U.UserName
            GROUP BY U.UserName
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
                    S.RequisitionId,
                    S.CandidateId,
                    S.Status,
                    MAX(S.CreatedDate) as LatestStatusDate,
                    R.CreatedBy as RequisitionOwner
                FROM Submissions S
                INNER JOIN Requisitions R ON R.Id = S.RequisitionId
                WHERE S.Status IN ('INT', 'OEX', 'HIR')
                GROUP BY S.RequisitionId, S.CandidateId, S.Status, R.CreatedBy
            ) SS ON SS.RequisitionOwner = U.UserName
            GROUP BY U.UserName
        )
        
        -- Generate all period rows for all accounts managers
        INSERT INTO [dbo].[SubmissionMetricsView] 
        ([CreatedBy], [MetricDate], [Period], [INT_Count], [OEX_Count], [HIR_Count], [Total_Submissions], [OEX_HIR_Ratio], [Requisitions_Created], [Active_Requisitions_Updated], [RefreshDate])
        
        SELECT 
            RM.[User] as CreatedBy,
            @Today as MetricDate,
            P.Period,
            
            -- Status counts by period
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
            
            -- Total submissions (0 for accounts managers - they don't submit, others submit to their reqs)
            0 as Total_Submissions,
            
            -- OEX:HIR Ratio
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
            
            -- Requisitions created by accounts manager
            CASE P.Period
                WHEN '7D' THEN RM.TOTAL_LAST7D_COUNT
                WHEN 'MTD' THEN RM.TOTAL_MTD_COUNT
                WHEN 'QTD' THEN RM.TOTAL_QTD_COUNT
                WHEN 'HYTD' THEN RM.TOTAL_HYTD_COUNT
                WHEN 'YTD' THEN RM.TOTAL_YTD_COUNT
            END as Requisitions_Created,
            
            -- Active requisitions (their reqs that are still active)
            CASE P.Period
                WHEN '7D' THEN RM.ACTIVE_LAST7D_COUNT
                WHEN 'MTD' THEN RM.ACTIVE_MTD_COUNT
                WHEN 'QTD' THEN RM.ACTIVE_QTD_COUNT
                WHEN 'HYTD' THEN RM.ACTIVE_HYTD_COUNT
                WHEN 'YTD' THEN RM.ACTIVE_YTD_COUNT
            END as Active_Requisitions_Updated,
            
            GETDATE() as RefreshDate
            
        FROM RequisitionMetrics RM
        CROSS JOIN (VALUES ('7D'), ('MTD'), ('QTD'), ('HYTD'), ('YTD')) P(Period)
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
        FROM dbo.Users U LEFT JOIN dbo.Requisitions R ON U.UserName = R.CreatedBy AND R.CreatedDate >= DATEADD(YEAR, -1, GETDATE()) -- Only last year's activity
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