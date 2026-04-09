create proc [EducationDistrict].[AssessmentAssignmentInsert] (
     @SessionId uniqueidentifier
    ,@AssessmentId uniqueidentifier
    ,@StudentId uniqueidentifier
    ,@StartAfter datetimeoffset(7)
    ,@EndBefore datetimeoffset(7)
    ,@Id uniqueidentifier output
) as

declare @districtId uniqueidentifier = (
    select top 1 district.Id
    from [Education].[District-Active] district
        inner join [Education].[DistrictContact-Active] districtContact on districtContact.DistrictId = district.Id
        inner join [Framework].[User-Active] userTable on districtContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Education].[AssessmentAssignment] (
    Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,AssessmentId
    ,StudentId
    ,Assigned
    ,StartAfter
    ,EndBefore
)
values (
    @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@AssessmentId
    ,@StudentId
    ,@now
    ,@StartAfter
    ,@EndBefore
)

if not exists (
    select 1
    from [Education].[AssessmentAssignment-Active] assessmentAssignment
        inner join [Education].[Student-Active] student on assessmentAssignment.StudentId = student.Id
        inner join [Education].[Family-Active] family on student.FamilyId = family.Id
        inner join [Education].[School-Active] school on family.SchoolId = school.Id
    where assessmentAssignment.Id = @Id
        and school.DistrictId = @districtId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction