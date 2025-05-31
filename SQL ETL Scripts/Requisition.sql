USE TitanPSS
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RequirementTemp]') AND type in (N'U'))
DROP TABLE [dbo].[RequirementTemp]
GO

SELECT * INTO RequirementTemp FROM dbo.REQUIREMENT;

USE Subscription
GO

TRUNCATE TABLE Subscription.dbo.Requisitions;

DELETE FROM Subscription.dbo.RequisitionView;

DBCC CHECKIDENT (Requisitions, RESEED, 3021);

DISABLE TRIGGER ALL ON Subscription.dbo.Requisitions;

SET IDENTITY_INSERT Subscription.dbo.Requisitions ON

INSERT INTO 
	Subscription.[dbo].[Requisitions]
    ([Id],[Code],[CompanyId],[HiringMgrId],[PosTitle],[Description],[Positions],[Duration],[DurationCode],[Location],[ExperienceId],
	[ExpRateLow],[ExpRateHigh],[ExpLoadLow],[ExpLoadHigh],[PlacementFee],[PlacementType],[JobOption],[ReportTo],[SalaryLow],
	[SalaryHigh],[ExpPaid],[ExpStart],[Status],[AlertOn],[AlertTimeout],[AlertRepFreq],[AlertEnd],[AlertMsg],[AlertMail],
	[IsHot],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[SkillsReq],[Education],[Eligibility],[SecurityClearance],[SecurityType],
	[Benefits],[BenefitsNotes],[OFCCP],[AttachName],[AttachFileType],[AttachContentType],[Due],[BenefitName],[BenefitFileType],
	[BenefitContentType],[City],[StateId],[Zip],[AlertLastSent],[AttachName2],[AttachFileType2],[AttachContentType2],[AttachName3],
	[AttachFileType3],[AttachContentType3],[Attachments],[BenefitsAttach],[Attachments2],[Attachments3],[AssignedRecruiter],
	[SecondaryRecruiter],[MandatoryRequirement],[PreferredRequirement],[OptionalRequirement])
SELECT
	REQUIREMENT_ID,REQUIREMENT_CODE,CLIENT_ID,CLIENT_CONTACT_ID,JOB_TITLE,JOB_DESC,NO_OF_POSITIONS,JOB_DURATION,DURATION_CODE,A.STATE_ID,EXPERIENCE_ID,
	EXPECTED_RATE_LOW,EXPECTED_RATE_HIGH,0,0,10,1,CASE A.JOB_OPTION WHEN 'C' THEN 'W' WHEN 'H' THEN '1' WHEN 'D' THEN 'F' END,'',SALARY_LOW,
	SALARY_HIGH,CAST(CASE EXPENSES_PAID WHEN 'Y' THEN 1 ELSE 0 END as bit),EXPECTED_START_DATE,CURRENT_STATUS,CAST(CASE ALERT_ON WHEN 'Y' THEN 1 ELSE 0 END as bit),
	ALERT_TIMEOUT,ALERT_REPEAT_FREQUENCY,ALERT_END_DATE,ALERT_MSG,CAST(CASE ALERT_EMAIL WHEN 'Y' THEN 1 ELSE 0 END as bit),CASE IS_HOT WHEN 'Y' THEN 2 ELSE 1 END, 
	A.CREATED_BY,A.DATE_CREATED,A.UPDATED_BY,A.DATE_UPDATED,
	ISNULL((SELECT STRING_AGG(S.SKILL, ', ') FROM TitanPSS.dbo.ENTITY_SKILLS ES JOIN TitanPSS.dbo.SKILLS S ON ES.SKILL_ID = S.SKILL_ID WHERE ES.ENTITY_ID = A.REQUIREMENT_ID 
	AND ES.ENTITY_TYPE_CODE = 'REQ'), '') AS SKILLS_LIST,NULL,NULL,0,0,0,'',0,NULL,'.doc',NULL,DUE_DATE,NULL,'.doc',NULL,ISNULL(B.CITY,''),ISNULL(B.STATE_ID,1),ISNULL(B.ZIP_CODE,''),NULL,
	NULL,'.doc',NULL,NULL,'.doc',NULL,NULL,NULL,'.doc',NULL,''	,'','','',''
FROM
	TitanPSS.dbo.RequirementTemp A INNER JOIN TitanPSS.dbo.ENTITY_ADDRESS B ON A.CLIENT_ID=B.ENTITY_ID AND ENTITY_TYPE_CODE='CLI'
GO

SET IDENTITY_INSERT Subscription.dbo.Candidate OFF;

ENABLE TRIGGER ALL ON Subscription.dbo.Candidate;
