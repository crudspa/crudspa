create proc [EducationCommon].[ActivityElementUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@ElementId uniqueidentifier
    ,@ActivityId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Education].[ActivityElement]
set
    Updated = @now
    ,UpdatedBy = @SessionId
    ,ElementId = @ElementId
    ,ActivityId = @ActivityId
where Id = @Id