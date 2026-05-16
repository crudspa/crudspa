create proc [ContentDesign].[SmsTemplateSelectFull]  (
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
     smsTemplate.Id
    ,smsTemplate.MembershipId
    ,membership.Name as MembershipName
    ,smsTemplate.Title
    ,smsTemplate.Body
from [Content].[SmsTemplate-Active] smsTemplate
    inner join [Content].[Membership-Active] membership on smsTemplate.MembershipId = membership.Id
    inner join [Framework].[Portal-Active] portal on membership.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where organization.Id = @organizationId
order by smsTemplate.Title