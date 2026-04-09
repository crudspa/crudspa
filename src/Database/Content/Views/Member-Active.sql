create view [Content].[Member-Active] as

select member.Id as Id
    ,member.MembershipId as MembershipId
    ,member.ContactId as ContactId
    ,member.Status as Status
from [Content].[Member] member
where 1=1
    and member.IsDeleted = 0
    and member.VersionOf = member.Id