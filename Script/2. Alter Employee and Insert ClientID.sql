USE [ABS_LMS]
GO

ALTER TABLE [dbo].[Employee] ADD ClientId INT NOT NULL
GO

ALTER TABLE dbo.EmployeeLeaveHistory ALTER COLUMN NoOfDays decimal(18, 4) not NUll
GO

UPDATE E SET E.CLIENTID = C.CLIENTID
FROM EMPLOYEE E 
INNER JOIN IMPORTEDEMPLOYEE IE ON E.EMPLOYEECODE = IE.EMPLOYEECODE
INNER JOIN CLIENT C ON IE.CLIENT = C.NAME
WHERE E.EMPLOYEECODE = IE.EMPLOYEECODE
GO


ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Client] FOREIGN KEY([ClientId])
REFERENCES [dbo].[Client] ([ClientId])
GO

ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_Client]
GO
