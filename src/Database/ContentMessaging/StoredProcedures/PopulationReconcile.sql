create proc [ContentMessaging].[PopulationReconcile] (
     @SessionId uniqueidentifier
    ,@MembershipId uniqueidentifier
    ,@Members [ContentMessaging].[GuidList] readonly
    ,@Tokens [ContentMessaging].[PopulationTokenList] readonly
    ,@TokenValues [ContentMessaging].[PopulationTokenValueList] readonly
) as

set nocount on
set xact_abort on

declare @now datetimeoffset = sysdatetimeoffset()
declare @added int = 0
declare @removed int = 0
declare @preserved int = 0
declare @optedOut int = 0

if not exists (
    select 1
    from [Content].[Membership-Active] membership
        left join [Content].[ActivationScope-Active] scope on scope.Id = membership.ActivationScopeId
            and scope.OrganizationId = membership.OrganizationId
        left join [Content].[Activation-Active] activation on activation.Id = scope.ActivationId
        cross apply [ContentMessaging].[SessionCanWriteOrganization](
            @SessionId,
            membership.PortalId,
            case when membership.ActivationScopeId is null then membership.OrganizationId else activation.OrganizationId end)
    where membership.Id = @MembershipId
        and membership.PopulationId is not null
        and (membership.ActivationScopeId is null or activation.Id is not null)
)
    throw 50000, 'The virtual Membership is unavailable to this session.', 1

if exists (
    select 1
    from @Members source
        left join [Framework].[Contact-Active] contact on source.Id = contact.Id
    where contact.Id is null
)
    throw 50000, 'The Population resolver returned an invalid Contact.', 1

if exists (
    select 1
    from @TokenValues value
        left join @Members member on value.ContactId = member.Id
        left join @Tokens token on value.[Key] = token.[Key]
    where member.Id is null or token.[Key] is null
)
    throw 50000, 'Population token values must reference submitted Members and Tokens.', 1

begin transaction

insert [Content].[Member] (
     Id, VersionOf, Updated, UpdatedBy, MembershipId, ContactId, Status
)
select
     generated.Id, generated.Id, @now, @SessionId, @MembershipId, source.Id, 0
from @Members source
    cross apply (select newid() as Id) generated
where not exists (
    select 1
    from [Content].[Member-Active] member with (updlock, holdlock)
    where member.MembershipId = @MembershipId
        and member.ContactId = source.Id
)

set @added = @@rowcount

update member
set
     Updated = @now
    ,UpdatedBy = @SessionId
    ,IsDeleted = 1
from [Content].[Member] member
where member.Id = member.VersionOf
    and member.IsDeleted = 0
    and member.MembershipId = @MembershipId
    and member.Status = 0
    and not exists (select 1 from @Members source where source.Id = member.ContactId)

set @removed = @@rowcount

select @preserved = count(*)
from [Content].[Member-Active] member
where member.MembershipId = @MembershipId
    and member.Status = 1

select @optedOut = count(*)
from [Content].[Member-Active] member
where member.MembershipId = @MembershipId
    and member.Status = 2

update token
set
     IsDeleted = 0
    ,Description = source.Description
    ,Ordinal = source.Ordinal
from [Content].[Token] token
    inner join @Tokens source on token.[Key] = source.[Key]
where token.MembershipId = @MembershipId

insert [Content].[Token] (Id, IsDeleted, MembershipId, [Key], Description, Ordinal)
select newid(), 0, @MembershipId, source.[Key], source.Description, source.Ordinal
from @Tokens source
where not exists (
    select 1 from [Content].[Token] token
    where token.MembershipId = @MembershipId and token.[Key] = source.[Key]
)

update token
set IsDeleted = 1
from [Content].[Token] token
where token.MembershipId = @MembershipId
    and token.IsDeleted = 0
    and not exists (select 1 from @Tokens source where source.[Key] = token.[Key])

update value
set
     Updated = @now
    ,UpdatedBy = @SessionId
    ,Value = source.Value
from [Content].[TokenValue] value
    inner join [Content].[Token] token on value.TokenId = token.Id
    inner join @TokenValues source on source.ContactId = value.ContactId
        and source.[Key] = token.[Key]
where token.MembershipId = @MembershipId
    and token.IsDeleted = 0

insert [Content].[TokenValue] (Id, Updated, UpdatedBy, TokenId, ContactId, Value)
select newid(), @now, @SessionId, token.Id, source.ContactId, source.Value
from @TokenValues source
    inner join [Content].[Token] token on token.MembershipId = @MembershipId
        and token.[Key] = source.[Key]
        and token.IsDeleted = 0
where not exists (
    select 1
    from [Content].[TokenValue] value
    where value.TokenId = token.Id and value.ContactId = source.ContactId
)

delete value
from [Content].[TokenValue] value
    inner join [Content].[Token] token on value.TokenId = token.Id
where token.MembershipId = @MembershipId
    and not exists (
        select 1
        from @TokenValues source
        where source.ContactId = value.ContactId and source.[Key] = token.[Key]
    )

commit transaction

select
     @added as Added
    ,@removed as Removed
    ,@preserved as Preserved
    ,@optedOut as OptedOut
    ,(select count(*) from [Content].[Token] where MembershipId = @MembershipId and IsDeleted = 0) as Tokens
    ,(
        select count(*)
        from [Content].[TokenValue] value
            inner join [Content].[Token] token on value.TokenId = token.Id
        where token.MembershipId = @MembershipId and token.IsDeleted = 0
    ) as TokenValues