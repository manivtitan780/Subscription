USE Subscription
GO

TRUNCATE TABLE Subscription.dbo.CompanyContacts;

DISABLE TRIGGER ALL ON Subscription.dbo.CompanyContacts;

SET IDENTITY_INSERT Subscription.dbo.CompanyContacts ON

INSERT INTO 
	[dbo].[CompanyContacts]
    ([ID],[CompanyID],[ContactPrefix],[ContactFirstName],[ContactMiddleInitial],[ContactLastName],[ContactSuffix],[CompanyLocationID],
	[ContactEmailAddress],[ContactPhone],[ContactPhoneExtension],[ContactFax],[Designation],[Department],[Role],[Notes],
	[PrimaryContact],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
SELECT
    A.CONTACT_ID,A.PARENT_ENTITY_ID,'',A.FIRST_NAME,ISNULL(A.MIDDLE_NAME, '') AS MIDDLE_NAME,A.LAST_NAME,'',0,B.ECOMM,dbo.StripNonNumeric(EP.PHONE) AS PHONE,'','',D.DESIGNATION,
    A.DEPARTMENT,3,ISNULL(E.NOTES, '') AS NOTES,CAST(CASE WHEN A.CONTACT_ID = (SELECT MIN(C2.CONTACT_ID) FROM TitanPSS.dbo.CONTACT C2 WHERE C2.PARENT_ENTITY_ID = A.PARENT_ENTITY_ID)
	THEN 1 ELSE 0 END AS BIT) AS [PRIMARY],A.CREATED_BY,A.DATE_CREATED,A.UPDATED_BY,A.DATE_UPDATED
FROM 
	TitanPSS.dbo.CONTACT A LEFT JOIN TitanPSS.dbo.ENTITY_ECOMM B ON A.CONTACT_ID = B.ENTITY_ID AND B.ENTITY_TYPE_CODE = 'CNT'
	OUTER APPLY (SELECT TOP 1 EP.PHONE FROM TitanPSS.dbo.ENTITY_PHONE EP WHERE EP.ENTITY_ID = A.CONTACT_ID AND EP.ENTITY_TYPE_CODE = 'CNT' 
		AND EP.PHONE_TYPE_CODE IN ('WKPH', 'CELL', 'HMPH', 'FAX', 'OTPH') ORDER BY CASE EP.PHONE_TYPE_CODE WHEN 'WKPH' THEN 1 WHEN 'CELL' THEN 2 WHEN 'HMPH' THEN 3 WHEN 'FAX' THEN 4
		WHEN 'OTPH' THEN 5 ELSE 6 END) EP
	INNER JOIN TitanPSS.dbo.DESIGNATION D ON A.DESIGNATION_ID = D.DESIGNATION_ID
	LEFT JOIN TitanPSS.dbo.ENTITY_NOTES E ON A.CONTACT_ID = E.ENTITY_ID AND E.ENTITY_TYPE_CODE = 'CNT'
ORDER BY A.PARENT_ENTITY_ID;
GO

-- Step 1: Find TOP 1 address per contact from TitanPSS
WITH TopContactAddress AS (
    SELECT
        c.CONTACT_ID,
        ea.ENTITY_ADDRESS_ID,
        ea.ADDRESS,
        ea.CITY,
        ea.STATE_ID,
        ea.ZIP_CODE,
        ROW_NUMBER() OVER (PARTITION BY c.CONTACT_ID ORDER BY ea.ENTITY_ADDRESS_ID) AS rn,
		c.FIRST_NAME, c.MIDDLE_NAME, c.LAST_NAME
    FROM TitanPSS.dbo.CONTACT c
    JOIN TitanPSS.dbo.ENTITY_ADDRESS ea
        ON c.CONTACT_ID = ea.ENTITY_ID
    WHERE ea.ENTITY_TYPE_CODE = 'CNT'
)
-- Step 2: Match against Subscription's CompanyLocations
, MatchedLocation AS (
    SELECT
        tca.CONTACT_ID,
        cl.ID AS CompanyLocationID
    FROM TopContactAddress tca
    JOIN Subscription.dbo.CompanyContacts cc
        ON ISNULL(cc.ContactFirstName,'') COLLATE Latin1_General_CI_AI = ISNULL(tca.FIRST_NAME,'') COLLATE Latin1_General_CI_AI
        AND ISNULL(cc.ContactMiddleInitial,'') COLLATE Latin1_General_CI_AI = ISNULL(tca.MIDDLE_NAME,'') COLLATE Latin1_General_CI_AI
        AND ISNULL(cc.ContactLastName,'') COLLATE Latin1_General_CI_AI = ISNULL(tca.LAST_NAME,'') COLLATE Latin1_General_CI_AI
    JOIN Subscription.dbo.CompanyLocations cl
        ON ISNULL(cl.StreetName,'') COLLATE Latin1_General_CI_AI = ISNULL(tca.ADDRESS,'') COLLATE Latin1_General_CI_AI
        AND ISNULL(cl.City,'') COLLATE Latin1_General_CI_AI = ISNULL(tca.CITY,'') COLLATE Latin1_General_CI_AI
        AND ISNULL(cl.StateID,0) = ISNULL(tca.STATE_ID,0)
        AND ISNULL(cl.ZipCode,'') COLLATE Latin1_General_CI_AI = ISNULL(tca.ZIP_CODE,'') COLLATE Latin1_General_CI_AI
    WHERE tca.rn = 1
)
-- Step 3: Update CompanyContacts with matched CompanyLocationID
UPDATE cc
SET cc.CompanyLocationID = ml.CompanyLocationID
FROM Subscription.dbo.CompanyContacts cc
JOIN MatchedLocation ml
    ON cc.ID = ml.CONTACT_ID

	
DECLARE @Max int = 381;

SELECT
	@Max = MAX(ID)
FROM
	Subscription.dbo.CompanyContacts;

DBCC CHECKIDENT ('Subscription.dbo.CompanyContacts', RESEED, @Max);
