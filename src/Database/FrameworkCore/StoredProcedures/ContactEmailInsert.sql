create proc [FrameworkCore].[ContactEmailInsert] (
     @SessionId uniqueidentifier
    ,@ContactId uniqueidentifier
    ,@Email nvarchar(75)
    ,@Ordinal int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Framework].[ContactEmail] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ContactId
    ,Email
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ContactId
    ,@Email
    ,@Ordinal
)