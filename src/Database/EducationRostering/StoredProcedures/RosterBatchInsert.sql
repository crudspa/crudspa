create proc [EducationRostering].[RosterBatchInsert] (
     @RosterRunId uniqueidentifier
    ,@Json nvarchar(max)
) as

set xact_abort on

if not exists (
    select 1
    from [Education].[RosterRun] rosterRun with (updlock, holdlock)
    where rosterRun.Id = @RosterRunId
        and rosterRun.Status = N'started'
)
    throw 50000, 'Roster run is unavailable or no longer accepting data.', 1

insert [Education].[RosterSchool] (
     Id, RosterRunId, ExternalId, SisId, Name, Kind, Status, SourceHash
)
select
     newid(), @RosterRunId, trim(value.ExternalId), nullif(trim(value.SisId), N''), trim(value.Name), value.Kind, value.Status
    ,hashbytes('SHA2_256', concat_ws(nchar(31), value.ExternalId, value.SisId, value.Name, value.Kind, value.Status))
from openjson(@Json, '$.Schools') with (
     ExternalId nvarchar(255)
    ,SisId nvarchar(255)
    ,Name nvarchar(255)
    ,Kind nvarchar(25)
    ,Status nvarchar(25)
) value

insert [Education].[RosterTerm] (
     Id, RosterRunId, ExternalId, SisId, Name, Kind, Starts, Ends, Status, SourceHash
)
select
     newid(), @RosterRunId, trim(value.ExternalId), nullif(trim(value.SisId), N''), trim(value.Name), value.Kind
    ,value.Starts, value.Ends, value.Status
    ,hashbytes('SHA2_256', concat_ws(nchar(31), value.ExternalId, value.SisId, value.Name, value.Kind, convert(nvarchar(10), value.Starts, 23), convert(nvarchar(10), value.Ends, 23), value.Status))
from openjson(@Json, '$.Terms') with (
     ExternalId nvarchar(255)
    ,SisId nvarchar(255)
    ,Name nvarchar(255)
    ,Kind nvarchar(25)
    ,Starts date
    ,Ends date
    ,Status nvarchar(25)
) value

insert [Education].[RosterCourse] (
     Id, RosterRunId, ExternalId, SisId, Name, Number, Status, SourceHash
)
select
     newid(), @RosterRunId, trim(value.ExternalId), nullif(trim(value.SisId), N''), trim(value.Name), nullif(trim(value.Number), N''), value.Status
    ,hashbytes('SHA2_256', concat_ws(nchar(31), value.ExternalId, value.SisId, value.Name, value.Number, value.Status))
from openjson(@Json, '$.Courses') with (
     ExternalId nvarchar(255)
    ,SisId nvarchar(255)
    ,Name nvarchar(255)
    ,Number nvarchar(75)
    ,Status nvarchar(25)
) value

insert [Education].[RosterClass] (
     Id, RosterRunId, ExternalId, SisId, SchoolExternalId, CourseExternalId, TermExternalId, Name, Grade, Subject, Status, SmallClassroom, SourceHash
)
select
     newid(), @RosterRunId, trim(value.ExternalId), nullif(trim(value.SisId), N''), trim(value.SchoolExternalId)
    ,nullif(trim(value.CourseExternalId), N''), nullif(trim(value.TermExternalId), N''), trim(value.Name)
    ,nullif(trim(value.Grade), N''), nullif(trim(value.Subject), N''), value.Status, value.SmallClassroom
    ,hashbytes('SHA2_256', concat_ws(nchar(31), value.ExternalId, value.SisId, value.SchoolExternalId, value.CourseExternalId, value.TermExternalId, value.Name, value.Grade, value.Subject, value.Status, value.SmallClassroom))
from openjson(@Json, '$.Classes') with (
     ExternalId nvarchar(255)
    ,SisId nvarchar(255)
    ,SchoolExternalId nvarchar(255)
    ,CourseExternalId nvarchar(255)
    ,TermExternalId nvarchar(255)
    ,Name nvarchar(255)
    ,Grade nvarchar(25)
    ,Subject nvarchar(75)
    ,Status nvarchar(25)
    ,SmallClassroom bit
) value

insert [Education].[RosterPerson] (
     Id, RosterRunId, ExternalId, SisId, FirstName, LastName, Email, Status, AuthIssuer, AuthSubject, AssessmentLevel, SourceHash
)
select
     newid(), @RosterRunId, trim(value.ExternalId), nullif(trim(value.SisId), N''), trim(value.FirstName), trim(value.LastName)
    ,nullif(trim(value.Email), N''), value.Status, nullif(trim(value.AuthIssuer), N''), nullif(trim(value.AuthSubject), N''), nullif(trim(value.AssessmentLevel), N'')
    ,hashbytes('SHA2_256', concat_ws(nchar(31), value.ExternalId, value.SisId, value.FirstName, value.LastName, value.Email, value.Status, value.AuthIssuer, value.AuthSubject, value.AssessmentLevel))
from openjson(@Json, '$.People') with (
     ExternalId nvarchar(255)
    ,SisId nvarchar(255)
    ,FirstName nvarchar(100)
    ,LastName nvarchar(100)
    ,Email nvarchar(255)
    ,Status nvarchar(25)
    ,AuthIssuer nvarchar(500)
    ,AuthSubject nvarchar(255)
    ,AssessmentLevel nvarchar(25)
) value

insert [Education].[RosterRole] (
     Id, RosterRunId, ExternalId, PersonExternalId, SchoolExternalId, Role, Grade, [Primary], SourceHash
)
select
     newid(), @RosterRunId, trim(value.ExternalId), trim(value.PersonExternalId), nullif(trim(value.SchoolExternalId), N'')
    ,value.Role, nullif(trim(value.Grade), N''), value.[Primary]
    ,hashbytes('SHA2_256', concat_ws(nchar(31), value.ExternalId, value.PersonExternalId, value.SchoolExternalId, value.Role, value.Grade, value.[Primary]))
from openjson(@Json, '$.Roles') with (
     ExternalId nvarchar(255)
    ,PersonExternalId nvarchar(255)
    ,SchoolExternalId nvarchar(255)
    ,Role nvarchar(25)
    ,Grade nvarchar(25)
    ,[Primary] bit
) value

insert [Education].[RosterEnrollment] (
     Id, RosterRunId, ExternalId, PersonExternalId, ClassExternalId, SchoolExternalId, Role, [Primary], Status, SourceHash
)
select
     newid(), @RosterRunId, trim(value.ExternalId), trim(value.PersonExternalId), trim(value.ClassExternalId)
    ,nullif(trim(value.SchoolExternalId), N''), value.Role, value.[Primary], value.Status
    ,hashbytes('SHA2_256', concat_ws(nchar(31), value.ExternalId, value.PersonExternalId, value.ClassExternalId, value.SchoolExternalId, value.Role, value.[Primary], value.Status))
from openjson(@Json, '$.Enrollments') with (
     ExternalId nvarchar(255)
    ,PersonExternalId nvarchar(255)
    ,ClassExternalId nvarchar(255)
    ,SchoolExternalId nvarchar(255)
    ,Role nvarchar(25)
    ,[Primary] bit
    ,Status nvarchar(25)
) value