USE Subscription
GO

TRUNCATE TABLE Subscription.dbo.Notes;

SET IDENTITY_INSERT Notes ON;

INSERT INTO 
	[dbo].[Notes]
	([Id],[EntityId],[EntityType],[Notes],[IsPrimary],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
SELECT 
	[ENTITY_NOTE_ID],[ENTITY_ID],[ENTITY_TYPE_CODE],[NOTES],CASE [IS_PRIMARY] WHEN 'Y' THEN 1 ELSE 0 END,[CREATED_BY],[DATE_CREATED],[UPDATED_BY],[DATE_UPDATED]
FROM 
	TitanPSS.[dbo].[ENTITY_NOTES]

SET IDENTITY_INSERT Notes OFF;

GO

