create proc [ContentDesign].[StyleSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

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
    inner join [Content].[Rule-Active] ruleTable on style.RuleId = ruleTable.Id
    inner join [Content].[ContentPortal-Active] contentPortal on style.ContentPortalId = contentPortal.Id
    inner join [Framework].[Portal-Active] portal on contentPortal.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    inner join [Content].[RuleType-Active] ruleType on ruleTable.TypeId = ruleType.Id
where style.Id = @Id
    and organization.Id = @organizationId