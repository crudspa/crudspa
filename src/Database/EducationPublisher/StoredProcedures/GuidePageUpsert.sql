create proc [EducationPublisher].[GuidePageUpsert] (
     @SessionId uniqueidentifier
    ,@GuideBinderId uniqueidentifier
    ,@PageId uniqueidentifier
    ,@ShowGuide bit
    ,@GuideText nvarchar(max)
    ,@GuideAudioId uniqueidentifier
    ,@ShowNotebook bit
    ,@Id uniqueidentifier output
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

select @Id = Id
from [Education].[GuidePage-Active]
where PageId = @PageId

if (@Id is null)
begin
    set @Id = newid()

    insert [Education].[GuidePage] (
         Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,GuideBinderId
        ,PageId
        ,ShowGuide
        ,GuideText
        ,GuideAudioId
        ,ShowNotebook
    )
    values (
         @Id
        ,@Id
        ,@now
        ,@SessionId
        ,@GuideBinderId
        ,@PageId
        ,@ShowGuide
        ,@GuideText
        ,@GuideAudioId
        ,@ShowNotebook
    )
end
else
begin
    update [Education].[GuidePage]
    set
         Id = @Id
        ,Updated = @now
        ,UpdatedBy = @SessionId
        ,GuideBinderId = @GuideBinderId
        ,ShowGuide = @ShowGuide
        ,GuideText = @GuideText
        ,GuideAudioId = @GuideAudioId
        ,ShowNotebook = @ShowNotebook
    where Id = @Id
end

commit transaction