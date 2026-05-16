create proc [ContentDisplay].[StylesSelectForPortal] (
     @PortalId uniqueidentifier
) as

select top 1
     contentPortal.PortalId as PortalId
    ,contentPortal.StyleRevision as StyleRevision
    ,(select count(1) from [Content].[Style-Active] style where style.ContentPortalId = contentPortal.Id) as StyleCount
from [Content].[ContentPortal-Active] contentPortal
where contentPortal.PortalId = @PortalId

select
     style.Id
    ,style.ContentPortalId
    ,style.RuleId
    ,style.ConfigJson
    ,ruleTable.Id as RuleId
    ,ruleTable.Name as RuleName
    ,ruleTable.[Key] as RuleKey
    ,ruleTable.TypeId as RuleTypeId
    ,ruleTable.DefaultValue as RuleDefaultValue
    ,ruleType.Id as RuleTypeId
    ,ruleType.Name as RuleTypeName
    ,ruleType.EditorView as RuleTypeEditorView
    ,ruleType.DisplayView as RuleTypeDisplayView
from [Content].[Style-Active] style
    inner join [Content].[ContentPortal-Active] contentPortal on style.ContentPortalId = contentPortal.Id
    inner join [Content].[Rule-Active] ruleTable on style.RuleId = ruleTable.Id
    inner join [Content].[RuleType-Active] ruleType on ruleTable.TypeId = ruleType.Id
where contentPortal.PortalId = @PortalId

select
     font.Id
    ,font.Name
from [Content].[Font-Active] font
    inner join [Content].[ContentPortal-Active] contentPortal on font.ContentPortalId = contentPortal.Id
where contentPortal.PortalId = @PortalId

select
     fontFace.Id
    ,fontFace.FontId
    ,fontFace.FileId
    ,fileTable.Format as FileFormat
    ,fontFace.Style
    ,fontFace.WeightMin
    ,fontFace.WeightMax
from [Content].[FontFace-Active] fontFace
    inner join [Content].[Font-Active] font on fontFace.FontId = font.Id
    inner join [Framework].[FontFile-Active] fileTable on fontFace.FileId = fileTable.Id
    inner join [Content].[ContentPortal-Active] contentPortal on font.ContentPortalId = contentPortal.Id
where contentPortal.PortalId = @PortalId