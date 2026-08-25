create proc [ContentDesign].[TokenSelectForPortal] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

;with PortalTokenCte
as (
    select
         token.Id
        ,token.MembershipId
        ,token.[Key]
        ,token.Description
        ,token.Ordinal
        ,row_number() over (
            partition by token.[Key]
            order by token.Ordinal, token.Id
        ) as RowNumber
    from [Content].[Token-Active] token
        inner join [Content].[Membership-Active] membership on token.MembershipId = membership.Id
        inner join [Framework].[Portal-Active] portal on membership.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where membership.PortalId = @PortalId
        and organization.Id = @organizationId
)

select
     Id
    ,MembershipId
    ,[Key]
    ,Description
    ,Ordinal
from PortalTokenCte
where RowNumber = 1
order by [Key]