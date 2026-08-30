create view [Framework].[ExternalIdentityLink-Active] as

select externalIdentityLink.Id as Id
    ,externalIdentityLink.ExternalIdentityId as ExternalIdentityId
    ,externalIdentityLink.UserId as UserId
    ,externalIdentityLink.Method as Method
    ,externalIdentityLink.Approved as Approved
    ,externalIdentityLink.ApprovedById as ApprovedById
from [Framework].[ExternalIdentityLink] externalIdentityLink
where 1=1
    and externalIdentityLink.IsDeleted = 0