create proc [ContentDisplay].[TrackContentIsAccessible] (
     @LicenseIds [Framework].[IdList] readonly
    ,@TrackId uniqueidentifier = null
    ,@CourseId uniqueidentifier = null
    ,@BinderId uniqueidentifier = null
    ,@PageId uniqueidentifier = null
    ,@ElementId uniqueidentifier = null
    ,@IsAccessible bit output
) as

declare @tracks table (Id uniqueidentifier primary key)

insert @tracks (Id)
select distinct track.Id
from [Content].[Track-Active] track
    left join [Content].[Course-Active] course on course.TrackId = track.Id
    left join [Content].[Page-Active] page on page.BinderId = course.BinderId
    left join [Content].[Section-Active] section on section.PageId = page.Id
    left join [Content].[Element-Active] element on element.SectionId = section.Id
where (@TrackId is not null and track.Id = @TrackId)
    or (@CourseId is not null and course.Id = @CourseId)
    or (@BinderId is not null and course.BinderId = @BinderId)
    or (@PageId is not null and page.Id = @PageId)
    or (@ElementId is not null and element.Id = @ElementId)

set @IsAccessible = case
    when not exists (select 1 from @tracks) then 1
    when exists (
        select 1
        from @tracks selected
        where not exists (
            select 1
            from [Content].[TrackLicense-Active] trackLicense
                inner join [Framework].[License-Active] license on license.Id = trackLicense.LicenseId
            where trackLicense.TrackId = selected.Id
        )
        or exists (
            select 1
            from [Content].[TrackLicense-Active] trackLicense
                inner join [Framework].[License-Active] license on license.Id = trackLicense.LicenseId
                inner join @LicenseIds sessionLicense on sessionLicense.Id = license.Id
            where trackLicense.TrackId = selected.Id
        )
    ) then 1
    else 0
end