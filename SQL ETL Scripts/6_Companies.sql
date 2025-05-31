USE Subscription
GO

TRUNCATE TABLE Subscription.dbo.Companies;

DISABLE TRIGGER ALL ON Subscription.dbo.Companies;

SET IDENTITY_INSERT Subscription.dbo.Companies ON

INSERT INTO 
	[Subscription].[dbo].[Companies]
    ([ID],[CompanyName],[EIN],[WebsiteURL],[DUN],[NAICSCode],[Status],[Notes],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
SELECT
	A.CLIENT_ID,A.CLIENT_NAME,'',ISNULL(A.WEBSITE,''),'','',CASE A.CURRENT_STATUS WHEN 'ACT' THEN 1 ELSE 0 END, B.NOTES,A.CREATED_BY,A.DATE_CREATED,A.UPDATED_BY,A.DATE_UPDATED
FROM
	TitanPSS.dbo.CLIENT A LEFT JOIN TitanPSS.dbo.ENTITY_NOTES B ON A.CLIENT_ID = B.ENTITY_ID AND B.ENTITY_TYPE_CODE = 'CLI'
GO

DECLARE @Max int = 1;

SELECT
	@Max = MAX(ID)
FROM
	Subscription.dbo.Companies;

DBCC CHECKIDENT ('Subscription.dbo.Companies', RESEED, @Max);