USE Subscription
GO

TRUNCATE TABLE Subscription.dbo.Companies;

DBCC CHECKIDENT (Companies, RESEED, 643);

DISABLE TRIGGER ALL ON Subscription.dbo.Companies;

SET IDENTITY_INSERT Subscription.dbo.Companies ON
