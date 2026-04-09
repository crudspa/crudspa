create proc [FrameworkJobs].[ExportRunInsert] (
     @ProcedureId uniqueidentifier
    ,@Run datetimeoffset
) as

insert [Framework].[ExportRun] (
     Id
    ,ProcedureId
    ,Run
)
values (
     newid()
    ,@ProcedureId
    ,@Run
)