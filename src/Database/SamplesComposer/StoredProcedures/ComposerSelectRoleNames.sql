create proc [SamplesComposer].[ComposerSelectRoleNames] (
     @SessionId uniqueidentifier
) as

set nocount on

select
     role.Id
    ,role.Name
from [Framework].[Role-Active] role
    inner join [Framework].[Organization-Active] organization on role.OrganizationId = organization.Id
    inner join [Samples].[Composer-Active] composer on composer.OrganizationId = organization.Id

where 1 = 1

order by role.Name