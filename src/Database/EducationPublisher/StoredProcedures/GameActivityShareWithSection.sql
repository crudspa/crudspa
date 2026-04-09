create proc [EducationPublisher].[GameActivityShareWithSection] (
     @SessionId uniqueidentifier
    ,@SourceGameActivityId uniqueidentifier
    ,@TargetGameSectionId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

insert [Education].[GameActivity] (
    Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,SectionId
    ,ActivityId
    ,ThemeWord
    ,Rigorous
    ,GroupId
    ,Ordinal
)
select g.generatedGuid
    ,g.generatedGuid
    ,@now
    ,@SessionId
    ,@TargetGameSectionId
    ,sourceGameActivity.ActivityId
    ,sourceGameActivity.ThemeWord
    ,sourceGameActivity.Rigorous
    ,sourceGameActivity.GroupId
    ,isnull((select (max(Ordinal) + 1) from [Education].[GameActivity-Active] where SectionId = @TargetGameSectionId), 0)
from [Education].[GameActivity-Active] sourceGameActivity
    left join (select newid() as generatedGuid) as g on 1=1
where sourceGameActivity.Id = @SourceGameActivityId