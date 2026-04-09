create view [Education].[AssessmentAssignment-Active] as

select assessmentAssignment.Id as Id
    ,assessmentAssignment.AssessmentId as AssessmentId
    ,assessmentAssignment.StudentId as StudentId
    ,assessmentAssignment.Assigned as Assigned
    ,assessmentAssignment.StartAfter as StartAfter
    ,assessmentAssignment.EndBefore as EndBefore
    ,assessmentAssignment.Started as Started
    ,assessmentAssignment.Completed as Completed
    ,assessmentAssignment.Terminated as Terminated
from [Education].[AssessmentAssignment] assessmentAssignment
where 1=1
    and assessmentAssignment.IsDeleted = 0
    and assessmentAssignment.VersionOf = assessmentAssignment.Id