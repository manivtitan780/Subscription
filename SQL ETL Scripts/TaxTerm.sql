-- TaxTerm Table

-- Step 1: Truncate the Subscription.dbo.TaxTerm table
TRUNCATE TABLE Subscription.dbo.TaxTerm;

-- Step 2: Insert data from TitanPSS.dbo.TaxTerm into Subscription.dbo.TaxTerm
INSERT INTO 
	Subscription.dbo.TaxTerm 
	(TaxTermCode, TaxTerm, Description, UpdateDate, Enabled)  -- Replace with actual column names
SELECT 
	TAXTERM_ID, TAXTERM, DESCRIPTION, DATE_UPDATED, 1
FROM 
	TitanPSS.dbo.TaxTerm;