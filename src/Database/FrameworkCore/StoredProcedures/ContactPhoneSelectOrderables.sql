create proc [FrameworkCore].[ContactPhoneSelectOrderables] (
    @SessionId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     contactPhone.Id
    ,contactPhone.Phone as Name
    ,contactPhone.Ordinal
    ,contactPhone.ContactId as ParentId
from [Framework].[ContactPhone-Active] contactPhone
    inner join [Framework].[Contact-Active] contact on contactPhone.ContactId = contact.Id
where contactPhone.SupportsSms = 1
    and exists (
        select 1
        from [Content].[Membership-Active] membership
            inner join [Framework].[Portal-Active] portal on membership.PortalId = portal.Id
        where portal.OwnerId = @organizationId
    )
order by contactPhone.Phone