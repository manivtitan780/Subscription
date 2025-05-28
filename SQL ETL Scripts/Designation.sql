-- Clear Designation and add from Titan PSS table

-- Step 1: Clear existing data in Subscription.dbo.Designation
TRUNCATE TABLE Subscription.dbo.Designation;

-- Step 2: Set Identity ON for Subscription.dbo.Designation
SET IDENTITY_INSERT Subscription.dbo.Designation ON;

-- Step 3: Insert data from TitanPSS.dbo.Designation into Subscription.dbo.Designation
INSERT INTO Subscription.dbo.Designation
	(Id, Designation, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, Enabled)
SELECT 
	DESIGNATION_ID, DESIGNATION, CREATED_BY, DATE_CREATED, UPDATED_BY, DATE_UPDATED, 1
FROM 
	TitanPSS.dbo.Designation;

-- Step 4: Set Identity OFF for Subscription.dbo.Designation
SET IDENTITY_INSERT Subscription.dbo.Designation ON;
