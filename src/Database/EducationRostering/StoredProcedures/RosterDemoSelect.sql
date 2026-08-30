create proc [EducationRostering].[RosterDemoSelect] (
    @RosterRunId uniqueidentifier
) as

set nocount on

declare @sourceId uniqueidentifier

select @sourceId = source.Id
from [Education].[RosterRun] run
    inner join [Education].[RosterSource-Active] source on source.Id = run.RosterSourceId
where run.Id = @RosterRunId
    and run.Status = N'applied'

if @sourceId is null
    throw 50000, 'The applied roster run is unavailable.', 1

create table #class (
    ClassroomId uniqueidentifier not null primary key
)

insert #class
select classroom.Id
from [Education].[RosterClass] staged
    inner join [EducationRostering].[RosterSelection](@RosterRunId, @sourceId) selected
        on selected.Kind = N'class'
        and selected.ExternalId = staged.ExternalId
    inner join [Education].[RosterLink-Active] link
        on link.RosterSourceId = @sourceId
        and link.Kind = N'class'
        and link.ExternalId = staged.ExternalId
    inner join [Education].[Classroom-Active] classroom on classroom.Id = link.LocalId
where staged.RosterRunId = @RosterRunId

create table #existing (
     ClassroomId uniqueidentifier not null
    ,AssessmentLevel nvarchar(4) not null
    ,primary key (ClassroomId, AssessmentLevel)
)

insert #existing
select distinct classroom.ClassroomId, contact.LastName
from #class classroom
    inner join [Education].[ClassroomStudent-Active] classroomStudent
        on classroomStudent.ClassroomId = classroom.ClassroomId
    inner join [Education].[Student-Active] student
        on student.Id = classroomStudent.StudentId
        and student.IsTestAccount = 1
    inner join [Framework].[Contact-Active] contact
        on contact.Id = student.ContactId
        and contact.FirstName = N'_Test'
        and contact.LastName in (N'Low', N'Mid', N'High')

select classroom.ClassroomId, level.Name AssessmentLevel
from #class classroom
    cross join (values (N'Low'), (N'Mid'), (N'High')) level(Name)
    left join #existing existing
        on existing.ClassroomId = classroom.ClassroomId
        and existing.AssessmentLevel = level.Name
where existing.ClassroomId is null
order by classroom.ClassroomId, level.Name

select SecretCode
from [Education].[Student-Active]
where SecretCode is not null