create view [Framework].[Version-Active] as

select version.Id as Id
    ,version.Created as Created
    ,version.Major as Major
    ,version.Minor as Minor
    ,version.Build as Build
    ,version.Revision as Revision
    ,version.Notes as Notes
from [Framework].[Version] version
where 1=1