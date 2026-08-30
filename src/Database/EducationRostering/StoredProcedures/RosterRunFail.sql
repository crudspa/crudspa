create proc [EducationRostering].[RosterRunFail] (
     @RosterRunId uniqueidentifier
) as

set xact_abort on

update [Education].[RosterRun]
set Status = N'failed'
    ,Completed = sysdatetimeoffset()
    ,IssueCount = IssueCount + 1
where Id = @RosterRunId
    and Status in (N'started', N'staged')

if @@rowcount = 1
    insert [Education].[RosterIssue] (
         Id
        ,RosterRunId
        ,Kind
        ,Severity
        ,Code
    )
    values (
         newid()
        ,@RosterRunId
        ,N'source'
        ,N'blocking'
        ,N'provider-failed'
    )