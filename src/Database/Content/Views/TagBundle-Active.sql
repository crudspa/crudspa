create view [Content].[TagBundle-Active] as

select tagBundle.Id as Id
    ,tagBundle.TagId as TagId
    ,tagBundle.BundleId as BundleId
from [Content].[TagBundle] tagBundle
where 1=1
    and tagBundle.IsDeleted = 0
    and tagBundle.VersionOf = tagBundle.Id