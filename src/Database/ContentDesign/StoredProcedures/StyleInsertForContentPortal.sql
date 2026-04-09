create proc [ContentDesign].[StyleInsertForContentPortal] (
     @ContentPortalId uniqueidentifier
) as

declare @updatedBy uniqueidentifier = '22f1a393-c003-4587-8f1d-02369d9c6c53'
declare @now datetimeoffset = sysdatetimeoffset()
declare @changed bit = 0

set nocount on
set xact_abort on
begin transaction

if not exists (
    select 1
    from [Content].[ContentPortal-Active] contentPortal
    where contentPortal.Id = @ContentPortalId
)
begin
    rollback transaction
    raiserror('Content portal not found', 16, 1)
    return
end

;with StyleRank as (
    select
         styleActive.Id
        ,row_number() over (
            partition by styleActive.RuleId
            order by styleBase.Updated desc, styleActive.Id desc
        ) as [Rank]
    from [Content].[Style-Active] styleActive
        inner join [Content].[Style] styleBase on styleActive.Id = styleBase.Id
    where styleActive.ContentPortalId = @ContentPortalId
)
update styleBase
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Content].[Style] styleBase
    inner join StyleRank styleRank on styleBase.Id = styleRank.Id
where styleRank.[Rank] > 1

if @@rowcount > 0
    set @changed = 1

insert [Content].[Style] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ContentPortalId
    ,RuleId
    ,ConfigJson
)
select
     styleSeed.Id
    ,styleSeed.Id
    ,@now
    ,@updatedBy
    ,@ContentPortalId
    ,ruleTable.Id
    ,ruleTable.DefaultValue
from [Content].[Rule-Active] ruleTable
    cross apply (select newid() as Id) styleSeed
where not exists (
    select 1
    from [Content].[Style-Active] style
    where style.ContentPortalId = @ContentPortalId
        and style.RuleId = ruleTable.Id
)

if @@rowcount > 0
    set @changed = 1

if @changed = 1
begin
    update contentPortal
    set
         Updated = @now
        ,UpdatedBy = @updatedBy
        ,StyleRevision = contentPortal.StyleRevision + 1
    from [Content].[ContentPortal] contentPortal
    where contentPortal.Id = @ContentPortalId
        and contentPortal.Id = contentPortal.VersionOf
end

commit transaction