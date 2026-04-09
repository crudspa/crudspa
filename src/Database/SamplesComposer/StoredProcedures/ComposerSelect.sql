create proc [SamplesComposer].[ComposerSelect] (
     @SessionId uniqueidentifier
) as

set nocount on

select
     composer.Id
    ,composer.OrganizationId
from [Samples].[Composer-Active] composer
    inner join [Framework].[Organization-Active] organization on composer.OrganizationId = organization.Id
where 1 = 1
    and composer.OrganizationId = (
        select top 1 userTable.OrganizationId
        from [Framework].[Session-Active] session
            inner join [Framework].[User-Active] userTable on userTable.Id = session.UserId
        where session.Id = @SessionId
            and session.Ended is null
    )