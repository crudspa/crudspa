create proc [ContentMessaging].[MessageSelectForActivation] (
     @SessionId uniqueidentifier
    ,@ActivationId uniqueidentifier
) as

set nocount on

select
     message.Id
    ,message.MembershipId
    ,message.StageId
    ,stage.Name as StageName
    ,message.ActivationId
    ,message.EmailId
    ,message.SmsId
from [Content].[Message-Active] message
    inner join [Content].[Activation-Active] activation on message.ActivationId = activation.Id
    inner join [Content].[Membership-Active] membership on message.MembershipId = membership.Id
    inner join [Content].[Stage-Active] stage on message.StageId = stage.Id
    cross apply [ContentMessaging].[SessionCanReadOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
where message.ActivationId = @ActivationId