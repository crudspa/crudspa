-- Turn on sqlcmd mode to eliminate Intellisense errors

-- This is the default on Azure, but make sure it's set everywhere
alter database [$(databasename)] set read_committed_snapshot on

:r .\BeforeUpgrade.sql
GO