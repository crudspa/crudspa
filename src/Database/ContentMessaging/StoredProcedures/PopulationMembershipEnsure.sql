create proc [ContentMessaging].[PopulationMembershipEnsure] (
     @SessionId uniqueidentifier
    ,@PopulationId uniqueidentifier
    ,@OrganizationId uniqueidentifier
    ,@ActivationScopeId uniqueidentifier = null
) as

set nocount on
set xact_abort on

declare @portalId uniqueidentifier = (select PortalId from [Content].[Population-Active] where Id = @PopulationId)
declare @accessOrganizationId uniqueidentifier = @OrganizationId

if @ActivationScopeId is not null
begin
    set @accessOrganizationId = null

    select @accessOrganizationId = activation.OrganizationId
    from [Content].[ActivationScope-Active] scope
        inner join [Content].[Activation-Active] activation on activation.Id = scope.ActivationId
    where scope.Id = @ActivationScopeId
        and scope.OrganizationId = @OrganizationId
end

if @portalId is null
    or @accessOrganizationId is null
    or not exists (select 1 from [ContentMessaging].[SessionCanWriteOrganization](@SessionId, @portalId, @accessOrganizationId))
    throw 51000, 'Population access denied.', 1

declare @membershipId uniqueidentifier
declare @now datetimeoffset = sysdatetimeoffset()

begin transaction

select @membershipId = Id
from [Content].[Membership-Active] with (updlock, holdlock)
where PopulationId = @PopulationId and OrganizationId = @OrganizationId
    and ((ActivationScopeId = @ActivationScopeId) or (ActivationScopeId is null and @ActivationScopeId is null))

if @membershipId is null
begin
    set @membershipId = newid()

    insert [Content].[Membership] (
         Id, VersionOf, Updated, UpdatedBy, PortalId, PopulationId, OrganizationId,
         Name, Description, SupportsOptOut, ActivationScopeId
    )
    select
         @membershipId, @membershipId, @now, @SessionId, population.PortalId, population.Id, @OrganizationId,
         population.Name, population.Description, population.SupportsOptOut, @ActivationScopeId
    from [Content].[Population-Active] population
    where population.Id = @PopulationId
end

commit transaction

select @membershipId