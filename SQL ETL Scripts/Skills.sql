-- Skills Table

-- Step 1: Truncate the Subscription.dbo.Skills table
TRUNCATE TABLE Subscription.dbo.Skills;

-- Step 2: Set identity insert on for Subscription.dbo.Skills
SET IDENTITY_INSERT Subscription.dbo.Skills ON;

-- Step 3: Insert data from TitanPSS.dbo.Skills into Subscription.dbo.Skills
INSERT INTO 
	Subscription.dbo.Skills 
	(Id, Skill, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, Enabled)  -- Replace with actual column names
SELECT 
	SKILL_ID, SKILL, CREATED_BY, DATE_CREATED, UPDATED_BY, DATE_UPDATED, 1
FROM 
	TitanPSS.dbo.Skills;

-- Step 4: Set identity insert off for Subscription.dbo.Skills
SET IDENTITY_INSERT Subscription.dbo.Skills OFF;