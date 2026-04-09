create proc [ContentDesign].[MemberSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     member.Id
    ,member.MembershipId
    ,member.Status
    ,contact.Id as ContactId
    ,contact.FirstName as ContactFirstName
    ,contact.LastName as ContactLastName
    ,contactEmail.Email as ContactEmailEmail
from [Content].[Member-Active] member
    inner join [Content].[Membership-Active] membership on member.MembershipId = membership.Id
    inner join [Framework].[Portal-Active] portal on membership.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    inner join [Framework].[Contact-Active] contact on member.ContactId = contact.Id
    outer apply (
        select top (1) email.Email
        from [Framework].[ContactEmail-Active] email
        where email.ContactId = contact.Id
        order by email.Ordinal
    ) contactEmail
where member.Id = @Id
    and organization.Id = @organizationId

select
     tokenValue.Id
    ,tokenValue.TokenId
    ,tokenValue.ContactId
    ,tokenValue.Value
    ,token.[Key] as TokenKey
from [Content].[TokenValue-Active] tokenValue
    inner join [Content].[Token-Active] token on tokenValue.TokenId = token.Id
    inner join [Framework].[Contact-Active] contact on tokenValue.ContactId = contact.Id
    inner join [Content].[Member-Active] member on member.ContactId = contact.Id
    inner join [Content].[Membership-Active] membership on member.MembershipId = membership.Id
    inner join [Framework].[Portal-Active] portal on membership.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where member.Id = @Id
    and organization.Id = @organizationId