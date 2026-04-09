create proc [EducationSchool].[AssessmentAssignmentResetProgress] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @Updated datetimeoffset = sysdatetimeoffset()
declare @Error int

create table #Assignments (
    AssignmentId uniqueidentifier not null primary key clustered
)

insert into #Assignments (AssignmentId)
select assessmentAssignment.Id
from [Education].[AssessmentAssignment-Active] assessmentAssignment
where assessmentAssignment.Id = @Id
and assessmentAssignment.Started is not null

set nocount on
set xact_abort on
begin transaction

update baseTable
set  IsDeleted = 1
    ,Updated = @Updated
    ,UpdatedBy = @SessionId
from [Education].[ListenChoiceSelection] baseTable
    inner join #Assignments assignments on assignments.AssignmentId = baseTable.AssignmentId
where baseTable.IsDeleted = 0

select @Error = @@error
if @Error <> 0 goto RollbackTransaction

update baseTable
set  IsDeleted = 1
    ,Updated = @Updated
    ,UpdatedBy = @SessionId
from [Education].[ListenPartCompleted] baseTable
    inner join #Assignments assignments on assignments.AssignmentId = baseTable.AssignmentId
where baseTable.IsDeleted = 0

select @Error = @@error
if @Error <> 0 goto RollbackTransaction

update baseTable
set  IsDeleted = 1
    ,Updated = @Updated
    ,UpdatedBy = @SessionId
from [Education].[ListenTextEntry] baseTable
    inner join #Assignments assignments on assignments.AssignmentId = baseTable.AssignmentId
where baseTable.IsDeleted = 0

select @Error = @@error
if @Error <> 0 goto RollbackTransaction

update baseTable
set  IsDeleted = 1
    ,Updated = @Updated
    ,UpdatedBy = @SessionId
from [Education].[ReadChoiceSelection] baseTable
    inner join #Assignments assignments on assignments.AssignmentId = baseTable.AssignmentId
where baseTable.IsDeleted = 0

select @Error = @@error
if @Error <> 0 goto RollbackTransaction

update baseTable
set  IsDeleted = 1
    ,Updated = @Updated
    ,UpdatedBy = @SessionId
from [Education].[ReadPartCompleted] baseTable
    inner join #Assignments assignments on assignments.AssignmentId = baseTable.AssignmentId
where baseTable.IsDeleted = 0

select @Error = @@error
if @Error <> 0 goto RollbackTransaction

update baseTable
set  IsDeleted = 1
    ,Updated = @Updated
    ,UpdatedBy = @SessionId
from [Education].[ReadTextEntry] baseTable
    inner join #Assignments assignments on assignments.AssignmentId = baseTable.AssignmentId
where baseTable.IsDeleted = 0

select @Error = @@error
if @Error <> 0 goto RollbackTransaction

update baseTable
set  IsDeleted = 1
    ,Updated = @Updated
    ,UpdatedBy = @SessionId
from [Education].[VocabChoiceSelection] baseTable
    inner join #Assignments assignments on assignments.AssignmentId = baseTable.AssignmentId
where baseTable.IsDeleted = 0

select @Error = @@error
if @Error <> 0 goto RollbackTransaction

update baseTable
set  IsDeleted = 1
    ,Updated = @Updated
    ,UpdatedBy = @SessionId
from [Education].[VocabPartCompleted] baseTable
    inner join #Assignments assignments on assignments.AssignmentId = baseTable.AssignmentId
where baseTable.IsDeleted = 0

select @Error = @@error
if @Error <> 0 goto RollbackTransaction

update baseTable
set  Started = null
    ,Completed = null
    ,Updated = @Updated
    ,UpdatedBy = @SessionId
from [Education].[AssessmentAssignment] baseTable
    inner join #Assignments assignments on assignments.AssignmentId = baseTable.Id

select @Error = @@error
if @Error <> 0 goto RollbackTransaction

commit transaction
drop table #Assignments
return

RollbackTransaction:
rollback transaction
drop table #Assignments
raiserror ('Assessment assignment reset failed.', 16, 1)
return