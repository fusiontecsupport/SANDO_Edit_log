USE SCFS_ERP;
SELECT s.name
FROM sys.schemas s
WHERE s.principal_id = USER_ID('ftec');

ALTER AUTHORIZATION ON SCHEMA::db_owner TO dbo;
ALTER AUTHORIZATION ON SCHEMA::db_accessadmin TO dbo;
ALTER AUTHORIZATION ON SCHEMA::db_securityadmin TO dbo;
ALTER AUTHORIZATION ON SCHEMA::db_backupoperator TO dbo;
ALTER AUTHORIZATION ON SCHEMA::db_datareader TO dbo;
ALTER AUTHORIZATION ON SCHEMA::db_datawriter TO dbo;
ALTER AUTHORIZATION ON SCHEMA::db_ddladmin TO dbo;







