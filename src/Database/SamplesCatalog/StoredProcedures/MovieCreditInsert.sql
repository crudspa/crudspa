create proc [SamplesCatalog].[MovieCreditInsert] (
     @SessionId uniqueidentifier
    ,@MovieId uniqueidentifier
    ,@Name nvarchar(120)
    ,@Part nvarchar(120)
    ,@Billing int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

declare @ordinal int = (select count(1) from [Samples].[MovieCredit-Active] where MovieId = @MovieId)

insert [Samples].[MovieCredit] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,MovieId
    ,Name
    ,Part
    ,Billing
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@MovieId
    ,@Name
    ,@Part
    ,@Billing
    ,@ordinal
)

commit transaction