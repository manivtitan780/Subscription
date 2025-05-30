/*USE TitanPSS
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CandidateTemp]') AND type in (N'U'))
DROP TABLE [dbo].[CandidateTemp]
GO

SELECT * INTO CandidateTemp FROM dbo.CANDIDATE;

UPDATE TitanPSS.dbo.CandidateTemp SET HOURLY_RATE = (HOURLY_RATE/1000) WHERE HOURLY_RATE > 9999
*/

USE Subscription
GO

TRUNCATE TABLE Subscription.dbo.Candidate;

DELETE FROM Subscription.dbo.CandidateView;

DBCC CHECKIDENT (Candidate, RESEED, 14359);

DISABLE TRIGGER ALL ON Subscription.dbo.Candidate;

SET IDENTITY_INSERT Subscription.dbo.Candidate ON


INSERT INTO 
    Subscription.[dbo].[Candidate]
    ([ID],[FirstName],[LastName],[MiddleName],[Title],[Address1],[Address2],[City],[StateId],[ZipCode],[Email],[Phone1],[Phone2],[Phone3],
    [Phone3Ext],[EligibilityId],[ExperienceId],[Experience],[JobOptions],[Communication],[TaxTerm],[SalaryLow],[SalaryHigh],[HourlyRate],
    [HourlyRateHigh],[VendorId],[Relocate],[RelocNotes],[Background],[SecurityClearanceNotes],[Keywords],[Summary],[ExperienceSummary],
    [Objective],[RateCandidate],[RateNotes],[MPC],[MPCNotes],[Status],[TextResume],[OriginalResume],[FormattedResume],[OriginalFileId],
    [FormattedFileId],[LinkedIn],[Facebook],[Twitter],[Google],[ReferAccountMgr],[Refer],[EEO],[ParsedXML],[JsonFileName],[CreatedBy],
    [CreatedDate],[UpdatedBy],[UpdatedDate])
SELECT
    A.CANDIDATE_ID,A.FIRST_NAME,A.LAST_NAME,ISNULL(A.MIDDLE_NAME,'') Mid,ISNULL(A.TITLE,'') Title, ISNULL(TRIM(B.Address), '') Add1, '' Add2, ISNULL(TRIM(B.City), '') City, 
    ISNULL(B.STATE_ID, 0) StateId,ISNULL(TRIM(B.ZIP_CODE), '') Zip,ISNULL(TRIM(C.ECOMM), '') Email,ISNULL(dbo.StripNonNumeric(D.PHONE), '') Ph1,'' Ph2,'' Ph3,
    ISNULL(D.PHONE_EXT, '') Ext,A.ELIGIBILITY_ID,A.EXPERIENCE_ID, 0 ExpMonth,'F' JobOpt,A.COMMUNICATION,CASE WHEN LEN(
            TRIM(',' FROM
                CASE WHEN [TAXTERM_1099] = 'Y' THEN '1,' ELSE '' END +
                CASE WHEN [TAXTERM_CORP_TO_CORP] = 'Y' THEN 'C,' ELSE '' END +
                CASE WHEN [TAXTERM_CORP_TO_CORP_SELF] = 'Y' THEN 'S,' ELSE '' END +
                CASE WHEN [TAXTERM_W2CONTRACT] = 'Y' THEN 'W,' ELSE '' END +
                CASE WHEN [TAXTERM_W2EMPLOYEE] = 'Y' THEN 'E,' ELSE '' END
            )
        ) > 0
        THEN TRIM(',' FROM
                CASE WHEN [TAXTERM_1099] = 'Y' THEN '1,' ELSE '' END +
                CASE WHEN [TAXTERM_CORP_TO_CORP] = 'Y' THEN 'C,' ELSE '' END +
                CASE WHEN [TAXTERM_CORP_TO_CORP_SELF] = 'Y' THEN 'S,' ELSE '' END +
                CASE WHEN [TAXTERM_W2CONTRACT] = 'Y' THEN 'W,' ELSE '' END +
                CASE WHEN [TAXTERM_W2EMPLOYEE] = 'Y' THEN 'E,' ELSE '' END
        )
        ELSE 'E'
    END TaxTerm,CAST(ISNULL(A.SALARY_LOW,0) as numeric(9,2)) SalLow,CAST(ISNULL(A.SALARY_HIGH,0) as numeric(9,2)) SalHigh,CAST(A.HOURLY_RATE as numeric(6,2)),CAST(A.HOURLY_RATE as numeric(6,2)),A.VENDOR_ID,CAST(CASE WHEN A.RELOCATE='N' THEN 0 ELSE 1 END as bit) Relocate,
    '' RelNotes,CAST(CASE WHEN A.BACKGROUND_CHECK='N' THEN 0 ELSE 1 END as bit) Sec,'' SecNotes,ISNULL(A.KEYWORDS,'') Keywords,'' Summ,'' ExpSumm,'' Objective,3 Rating,
    '[{"DateTime":"' + CONVERT(varchar(19),A.DATE_CREATED,126) + '","Name":"' + A.CREATED_BY + '","Rating":3,"Comment":"Created"}]' RateNotes,
    CAST(CASE WHEN A.IS_HOT='N' THEN 0 ELSE 1 END as bit) MPC,'[{"DateTime":"' + CONVERT(varchar(19),A.DATE_CREATED,126) + '","Name":"' + A.CREATED_BY + '","MPC":' + CASE WHEN A.IS_HOT='N' THEN 'false' ELSE 'true' END + ',"Comment":"Created"}]' MPCNotes,
    A.CURRENT_STATUS,ISNULL(A.TEXT_RESUME,'') TxtRes,ISNULL(A.ORIGINAL_RESUME,'') OrgRes,ISNULL(A.PRONTO_RESUME,'') FormRes,
    REPLACE(ISNULL(CAST(A.ORIGINAL_RESUME_FILE_ID as varchar(36)),''),'-','') OrgResID,REPLACE(ISNULL(CAST(A.PRONTO_RESUME_FILE_ID as varchar(36)),''),'-','') FormResID,'' LinkedIn,
    '' Facebook,'' X,'' Google,'' RefAcc,0 Refer,0 Eeo,'' PXML,'' JSONF,A.CREATED_BY,A.DATE_CREATED,A.UPDATED_BY,A.DATE_UPDATED
FROM
    TitanPSS.dbo.CandidateTemp A LEFT JOIN TitanPSS.dbo.ENTITY_ADDRESS B ON A.CANDIDATE_ID = B.ENTITY_ID AND B.ENTITY_TYPE_CODE = 'CND' AND B.IS_PRIMARY = 'Y'
    LEFT JOIN [TitanPSS].[dbo].[ENTITY_ECOMM] C ON A.CANDIDATE_ID = C.ENTITY_ID AND C.ENTITY_TYPE_CODE = 'CND' AND C.IS_PRIMARY = 'Y'
    LEFT JOIN [TitanPSS].[dbo].[ENTITY_PHONE] D ON A.CANDIDATE_ID = D.ENTITY_ID AND D.ENTITY_TYPE_CODE = 'CND' AND D.IS_PRIMARY = 'Y'
--WHERE
--    CANDIDATE_ID BETWEEN 16001 and 17000
GO

SET IDENTITY_INSERT Subscription.dbo.Candidate OFF;

ENABLE TRIGGER ALL ON Subscription.dbo.Candidate;
