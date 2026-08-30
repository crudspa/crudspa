create proc [FrameworkAuth].[AuthComplete] (
     @TransactionId uniqueidentifier
    ,@Provider nvarchar(75)
    ,@Issuer nvarchar(500)
    ,@Subject nvarchar(255)
    ,@Tenant nvarchar(255)
    ,@ProviderRole nvarchar(50)
    ,@ProviderAudience nvarchar(25)
    ,@IdentityKeyHash binary(32)
    ,@HandoffId uniqueidentifier
    ,@CodeHash binary(32)
) as

set nocount on
set xact_abort on

declare @now datetimeoffset = sysdatetimeoffset()
declare @code int = 1
declare @transactionProvider nvarchar(75)
declare @transactionAudience nvarchar(25)
declare @effectiveAudience nvarchar(25)
declare @returnPath nvarchar(500)
declare @externalIdentityId uniqueidentifier
declare @authPolicyId uniqueidentifier
declare @portalId uniqueidentifier
declare @userId uniqueidentifier
declare @organizationId uniqueidentifier
declare @policyOrganizationId uniqueidentifier

begin transaction

select
     @transactionProvider = tx.Provider
    ,@transactionAudience = tx.Audience
    ,@returnPath = tx.ReturnPath
from [Framework].[AuthTransaction] tx with (updlock, holdlock)
where tx.Id = @TransactionId
    and tx.Consumed is null
    and tx.Expires > @now

if @transactionProvider is not null
begin
    update [Framework].[AuthTransaction]
    set Consumed = @now
    where Id = @TransactionId

    set @code = case
        when @transactionProvider collate Latin1_General_100_BIN2 = @Provider collate Latin1_General_100_BIN2 then 0
        else 1
    end
end

if @code = 0
begin
    set @effectiveAudience = case when @transactionAudience = N'auto' then @ProviderAudience else @transactionAudience end

    if @effectiveAudience is null
        or len(@effectiveAudience) > 25
        or (@transactionAudience <> N'auto' and @transactionAudience <> @ProviderAudience)
        set @code = 2
end

declare @links table (
     PortalId uniqueidentifier not null
    ,UserId uniqueidentifier not null
    ,OrganizationId uniqueidentifier not null
)

if @code = 0
begin
    select @externalIdentityId = externalIdentity.Id
    from [Framework].[ExternalIdentity-Active] externalIdentity
    where externalIdentity.KeyHash = @IdentityKeyHash
        and externalIdentity.Provider collate Latin1_General_100_BIN2 = @Provider collate Latin1_General_100_BIN2
        and externalIdentity.Issuer collate Latin1_General_100_BIN2 = @Issuer collate Latin1_General_100_BIN2
        and externalIdentity.Subject collate Latin1_General_100_BIN2 = @Subject collate Latin1_General_100_BIN2
        and externalIdentity.Tenant collate Latin1_General_100_BIN2 = @Tenant collate Latin1_General_100_BIN2
        and externalIdentity.Enabled = 1

    if @externalIdentityId is null
        set @code = 3
end

if @code = 0
begin
    update [Framework].[ExternalIdentity]
    set ProviderRole = @ProviderRole
        ,LastSeen = @now
    where Id = @externalIdentityId

    insert @links (PortalId, UserId, OrganizationId)
    select
         portal.Id
        ,[user].Id
        ,[user].OrganizationId
    from [Framework].[ExternalIdentityLink-Active] link
        inner join [Framework].[User-Active] [user] on link.UserId = [user].Id
        inner join [Framework].[Portal-Active] portal on [user].PortalId = portal.Id
    where link.ExternalIdentityId = @externalIdentityId
        and portal.[Key] collate Latin1_General_100_BIN2 = @effectiveAudience collate Latin1_General_100_BIN2

    set @code = case
        when not exists (select 1 from @links) then 4
        when (select count(1) from @links) > 1 then 5
        else 0
    end
end

if @code = 0
begin
    select
         @portalId = PortalId
        ,@userId = UserId
        ,@organizationId = OrganizationId
    from @links

    set @policyOrganizationId = @organizationId

    if @effectiveAudience = N'school'
        select @policyOrganizationId = district.OrganizationId
        from [Education].[School-Active] school
            inner join [Education].[District-Active] district on school.DistrictId = district.Id
        where school.OrganizationId = @organizationId

    if @effectiveAudience = N'student'
        select @policyOrganizationId = district.OrganizationId
        from [Education].[Student-Active] student
            inner join [Education].[Family-Active] family on family.Id = student.FamilyId
            inner join [Education].[School-Active] school on family.SchoolId = school.Id
            inner join [Education].[District-Active] district on school.DistrictId = district.Id
        where student.UserId = @userId

    declare @policies table (Id uniqueidentifier not null primary key)

    insert @policies
    select policy.Id
    from [Framework].[AuthPolicy-Active] policy
        inner join [Framework].[AuthConnection-Active] connection on policy.AuthConnectionId = connection.Id
            and policy.OrganizationId = connection.OrganizationId
    where policy.OrganizationId = @policyOrganizationId
        and policy.Audience collate Latin1_General_100_BIN2 = @effectiveAudience collate Latin1_General_100_BIN2
        and policy.Enabled = 1
        and connection.Enabled = 1
        and connection.Provider collate Latin1_General_100_BIN2 = @Provider collate Latin1_General_100_BIN2
        and connection.Tenant collate Latin1_General_100_BIN2 = @Tenant collate Latin1_General_100_BIN2

    set @code = case
        when not exists (select 1 from @policies) then 6
        when (select count(1) from @policies) > 1 then 7
        else 0
    end

    select @authPolicyId = Id from @policies
end

if @code = 0
begin
    insert [Framework].[AuthHandoff] (
         Id
        ,AuthTransactionId
        ,CodeHash
        ,PortalId
        ,UserId
        ,ExternalIdentityId
        ,Created
        ,Expires
        ,AuthPolicyId
    )
    values (
         @HandoffId
        ,@TransactionId
        ,@CodeHash
        ,@portalId
        ,@userId
        ,@externalIdentityId
        ,@now
        ,dateadd(second, 60, @now)
        ,@authPolicyId
    )
end

insert [Framework].[AuthEvent] (
     Id
    ,Created
    ,CorrelationId
    ,Type
    ,Outcome
    ,Provider
    ,Tenant
    ,Audience
    ,PortalId
    ,ExternalIdentityId
    ,Reason
)
values (
     newid()
    ,@now
    ,@TransactionId
    ,N'auth-completed'
    ,case when @code = 0 then N'succeeded' else N'rejected' end
    ,@Provider
    ,@Tenant
    ,coalesce(@effectiveAudience, @transactionAudience, @ProviderAudience)
    ,@portalId
    ,@externalIdentityId
    ,case @code
        when 0 then null
        when 1 then N'invalid-transaction'
        when 2 then N'audience-mismatch'
        when 3 then N'identity-not-found'
        when 4 then N'link-not-found'
        when 5 then N'link-ambiguous'
        when 6 then N'policy-not-found'
        when 7 then N'policy-ambiguous'
    end
)

commit transaction

select
     @code as Code
    ,@portalId as PortalId
    ,@userId as UserId
    ,@returnPath as ReturnPath
    ,@effectiveAudience as Audience