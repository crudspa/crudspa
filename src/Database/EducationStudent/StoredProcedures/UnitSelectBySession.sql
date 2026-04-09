create proc [EducationStudent].[UnitSelectBySession] (
     @SessionId uniqueidentifier
) as
set nocount on

declare @now datetimeoffset = sysdatetimeoffset()
declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'
declare @ConditionGroupControl uniqueidentifier = 'c02332b0-14ee-4897-aa56-4aeba64e8bad'
declare @ConditionGroupTreatment uniqueidentifier = '3befe15f-f053-4f26-826f-583dead12346'

insert [Education].[CurriculumViewed] (
     Id
    ,Updated
    ,UpdatedBy
)
values (
     newid()
    ,@now
    ,@SessionId
)

declare @studentId uniqueidentifier
declare @studentConditionGroupId uniqueidentifier
declare @studentGradeId uniqueidentifier

select
     @studentId = student.Id
    ,@studentConditionGroupId = student.ConditionGroupId
    ,@studentGradeId = student.GradeId
from [Education].[Student-Active] student
    inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
    inner join [Framework].[User-Active] users on users.ContactId = contact.Id
    inner join [Framework].[Session-Active] session on session.UserId = users.Id
        and session.Id = @SessionId
option (recompile)

create table #StudentAchievements (
    AchievementId uniqueidentifier not null primary key clustered
)

insert #StudentAchievements (AchievementId)
select distinct sa.AchievementId
from [Education].[StudentAchievement-Active] sa
where sa.StudentId = @studentId

create table #LicensedUnits (
    Id uniqueidentifier not null primary key clustered
)

insert #LicensedUnits (Id)
select unit.Id
from [Education].[Unit-Active] unit
where unit.ParentId is null
    and exists (
        select 1
        from [EducationStudent].[UnitLicenses](@SessionId, unit.Id) unitLicense
    )

select
     unit.Id
    ,unit.Title
    ,unit.GeneratesAchievementId
    ,unit.RequiresAchievementId
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
from [Education].[Unit-Active] unit
    inner join [Framework].[ImageFile-Active] image on unit.ImageId = image.Id
    inner join #LicensedUnits licensedUnit on licensedUnit.Id = unit.Id
    left join #StudentAchievements requiredAchievement on requiredAchievement.AchievementId = unit.RequiresAchievementId
where unit.StatusId = @ContentStatusComplete
    and unit.GradeId = @studentGradeId
    and unit.ParentId is null
    and ((unit.Treatment = 1 and unit.Control = 1)
        or (unit.Treatment = 1 and @studentConditionGroupId = @ConditionGroupTreatment)
        or (unit.Control = 1 and @studentConditionGroupId = @ConditionGroupControl)
    )
    and (unit.RequiresAchievementId is null or requiredAchievement.AchievementId is not null)
order by unit.Ordinal, unit.Title