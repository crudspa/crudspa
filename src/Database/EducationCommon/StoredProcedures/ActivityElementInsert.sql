create proc [EducationCommon].[ActivityElementInsert] (
     @SessionId uniqueidentifier
    ,@ElementId uniqueidentifier
    ,@ActivityId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Education].[ActivityElement] (
    Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ElementId
    ,ActivityId
)
values (
    @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ElementId
    ,@ActivityId
)