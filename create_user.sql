USE master;
GO

-- Create Development user
IF EXISTS(SELECT * FROM sys.server_principals WHERE name = 'OAuthDev')
BEGIN
    ALTER SERVER ROLE [sysadmin] DROP MEMBER [OAuthDev];

	USE [Nexus OAuth (Development)]
	DROP USER [OAuthDev];

    DROP LOGIN [OAuthDev];
END

USE [master];
GO

CREATE LOGIN [OAuthDev] WITH PASSWORD = 'D3vel0pm3nt';
GO

USE [Nexus OAuth (Development)];
GO

CREATE USER [OAuthDev] FOR LOGIN [OAuthDev];
GO

GRANT SELECT,UPDATE,DELETE TO [OAuthDev]
GO

-- Create Production user
IF EXISTS(SELECT * FROM sys.server_principals WHERE name = 'OAuth')
BEGIN
    ALTER SERVER ROLE [sysadmin] DROP MEMBER [OAuth];

	USE [Nexus OAuth]
	DROP USER [OAuth];

    DROP LOGIN [OAuth];
END

USE [master];
GO

CREATE LOGIN [OAuth] WITH PASSWORD = 'N3xu$C0mp@n1';
GO

USE [Nexus OAuth];
GO

CREATE USER [OAuth] FOR LOGIN [OAuth];
GO

GRANT SELECT,UPDATE,DELETE TO [OAuth]
GO
