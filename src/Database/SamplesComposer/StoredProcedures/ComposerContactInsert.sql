create proc [SamplesComposer].[ComposerContactInsert] (
     @SessionId uniqueidentifier
    ,@ContactId uniqueidentifier
    ,@UserId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Samples].[ComposerContact] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ContactId
    ,UserId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ContactId
    ,@UserId
)

commit transaction