create proc [SamplesCatalog].[MovieCreditUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Name nvarchar(120)
    ,@Part nvarchar(120)
    ,@Billing int
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Name = @Name
    ,Part = @Part
    ,Billing = @Billing
from [Samples].[MovieCredit] baseTable
    inner join [Samples].[MovieCredit-Active] movieCredit on movieCredit.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction