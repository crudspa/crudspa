create proc [EducationCommon].[ActivityTextEntryInsert] (
     @SessionId uniqueidentifier
    ,@AssignmentId uniqueidentifier
    ,@Text nvarchar(max)
    ,@Made datetimeoffset
    ,@Ordinal int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

begin transaction

    insert [Education].[ActivityTextEntry] (
        Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,AssignmentId
        ,Text
        ,Made
        ,Ordinal
    )
    values (
        @Id
        ,@Id
        ,@now
        ,@SessionId
        ,@AssignmentId
        ,@Text
        ,@Made
        ,@Ordinal
    )

commit transaction