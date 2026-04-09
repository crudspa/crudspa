create proc [SamplesCatalog].[ShirtOptionUpdateRelations] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@AllSizes bit
    ,@Sizes Framework.IdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update [Samples].[ShirtOption]
set  Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,AllSizes = @AllSizes
where Id = @Id

if (@AllSizes = 0)
begin
    update [Samples].[ShirtOptionSize]
    set  IsDeleted = 1
        ,Updated = @now
        ,UpdatedBy = @SessionId
    where ShirtOptionId = @Id
        and IsDeleted = 0
        and VersionOf = Id
        and not exists (select 1 from @Sizes where Id = SizeId)

    insert [Samples].[ShirtOptionSize] (Id, VersionOf, Updated, UpdatedBy, ShirtOptionId, SizeId)
    select guid.NewId, guid.NewId, @now, @SessionId, @Id, Id
    from @Sizes
    cross apply (select newid() as NewId) guid
    where not exists (
        select 1 from [Samples].[ShirtOptionSize-Active]
        where ShirtOptionId = @Id and SizeId = Id
    )
end
else
begin
    update [Samples].[ShirtOptionSize]
    set  IsDeleted = 1
        ,Updated = @now
        ,UpdatedBy = @SessionId
    where ShirtOptionId = @Id
        and IsDeleted = 0
        and VersionOf = Id
end

commit transaction