create view [Content].[Token-Active] as

select token.Id as Id
    ,token.MembershipId as MembershipId
    ,token.[Key] as [Key]
    ,token.Description as Description
    ,token.Ordinal as Ordinal
from [Content].[Token] token
where 1=1
    and token.IsDeleted = 0