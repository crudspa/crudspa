create view [Education].[GuideBinder-Active] as

select guideBinder.Id as Id
    ,guideBinder.BinderId as BinderId
    ,guideBinder.GuideImageId as GuideImageId
from [Education].[GuideBinder] guideBinder
where 1=1
    and guideBinder.IsDeleted = 0
    and guideBinder.VersionOf = guideBinder.Id