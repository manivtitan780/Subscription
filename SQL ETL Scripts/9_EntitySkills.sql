USE Subscription
GO

TRUNCATE TABLE Subscription.dbo.EntitySkills;

INSERT INTO 
	[dbo].[EntitySkills]
    ([EntityId],[EntityType],[SkillId],[LastUsed],[ExpMonth],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
SELECT 
	[ENTITY_ID],[ENTITY_TYPE_CODE],[SKILL_ID],0,0,[CREATED_BY],[DATE_CREATED],[UPDATED_BY],[DATE_UPDATED]
FROM 
	[TitanPSS].[dbo].[ENTITY_SKILLS]

GO

