create proc [EducationSchool].[StudentDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Education].[Student]
set Updated = @now
    ,UpdatedBy = @SessionId
    ,DeletedBySchool = 1
where Id = @Id
    and exists (
        select 1
        from [Education].[Family-Active] family
        where family.Id = FamilyId
            and family.SchoolId in (select Id from [EducationSchool].[MySchools] (@SessionId))
    )