create view [Content].[Message-Active] as

select message.Id as Id
    ,message.StageId as StageId
    ,message.Name as Name
    ,message.PopulationId as PopulationId
    ,message.MessageTypeId as MessageTypeId
    ,message.EmailTemplateId as EmailTemplateId
    ,message.SmsTemplateId as SmsTemplateId
    ,message.Ordinal as Ordinal
    ,message.ActivationId as ActivationId
    ,message.DefinitionId as DefinitionId
    ,message.MembershipId as MembershipId
    ,message.EmailId as EmailId
    ,message.SmsId as SmsId
    ,message.ActivationStageId as ActivationStageId
from [Content].[Message] message
where 1=1
    and message.IsDeleted = 0
    and message.VersionOf = message.Id