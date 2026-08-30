create proc [EducationRostering].[RosterMembershipApply] (
     @RosterRunId uniqueidentifier
    ,@sourceId uniqueidentifier
    ,@provider nvarchar(75)
    ,@tenant nvarchar(255)
    ,@updatedBy uniqueidentifier
    ,@now datetimeoffset
) as

set nocount on

create table #class (
     ExternalId nvarchar(255) not null primary key
    ,ClassroomId uniqueidentifier not null
)

insert #class
select staged.ExternalId, link.LocalId
from [Education].[RosterClass] staged
    inner join [EducationRostering].[RosterSelection](@RosterRunId, @sourceId) selected
        on selected.Kind = N'class'
        and selected.ExternalId = staged.ExternalId
    inner join [Education].[RosterLink-Active] link
        on link.RosterSourceId = @sourceId
        and link.Kind = N'class'
        and link.ExternalId = staged.ExternalId
where staged.RosterRunId = @RosterRunId
    and staged.Status = N'active'

create table #student (
     ExternalId nvarchar(255) not null primary key
    ,StudentId uniqueidentifier not null
    ,FamilyId uniqueidentifier not null
    ,FamilyOrganizationId uniqueidentifier not null
    ,UserId uniqueidentifier not null
    ,SchoolId uniqueidentifier not null
    ,GradeId uniqueidentifier not null
)

insert #student
select distinct role.PersonExternalId, student.Id, student.FamilyId, family.OrganizationId, student.UserId, family.SchoolId, student.GradeId
from [Education].[RosterRole] role
    inner join [Education].[RosterLink-Active] link
        on link.RosterSourceId = @sourceId and link.Kind = N'role' and link.ExternalId = role.ExternalId
    inner join [Education].[Student-Active] student on student.Id = link.LocalId
    inner join [Education].[Family-Active] family on family.Id = student.FamilyId
where role.RosterRunId = @RosterRunId
    and role.Role = N'student'

create table #schoolContact (
     ExternalId nvarchar(255) not null primary key
    ,SchoolContactId uniqueidentifier not null
    ,PersonExternalId nvarchar(255) not null
)

insert #schoolContact
select role.ExternalId, link.LocalId, role.PersonExternalId
from [Education].[RosterRole] role
    inner join [Education].[RosterLink-Active] link
        on link.RosterSourceId = @sourceId and link.Kind = N'role' and link.ExternalId = role.ExternalId
    inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.Id = link.LocalId
where role.RosterRunId = @RosterRunId
    and role.Role in (N'teacher', N'staff', N'principal', N'literacy-facilitator', N'contact')

create table #schoolUser (
     ExternalId nvarchar(255) not null primary key
    ,UserId uniqueidentifier not null
    ,OrganizationId uniqueidentifier not null
)

insert #schoolUser
select schoolContact.PersonExternalId, min(convert(uniqueidentifier, schoolContactEntity.UserId)), min(convert(uniqueidentifier, [user].OrganizationId))
from #schoolContact schoolContact
    inner join [Education].[SchoolContact-Active] schoolContactEntity on schoolContactEntity.Id = schoolContact.SchoolContactId
    inner join [Framework].[User-Active] [user] on [user].Id = schoolContactEntity.UserId
group by schoolContact.PersonExternalId

create table #districtContact (
     ExternalId nvarchar(255) not null primary key
    ,DistrictContactId uniqueidentifier not null
    ,UserId uniqueidentifier not null
    ,PersonExternalId nvarchar(255) not null
)

insert #districtContact
select role.ExternalId, link.LocalId, districtContact.UserId, role.PersonExternalId
from [Education].[RosterRole] role
    inner join [Education].[RosterLink-Active] link
        on link.RosterSourceId = @sourceId and link.Kind = N'role' and link.ExternalId = role.ExternalId
    inner join [Education].[DistrictContact-Active] districtContact on districtContact.Id = link.LocalId
where role.RosterRunId = @RosterRunId
    and role.Role = N'district-admin'

create table #enrollment (
     ExternalId nvarchar(255) not null primary key
    ,LocalId uniqueidentifier null
    ,[Role] nvarchar(25) not null
    ,ClassroomId uniqueidentifier not null
    ,MemberId uniqueidentifier not null
    ,SourceHash binary(32) not null
)

insert #enrollment
select staged.ExternalId, link.LocalId, staged.Role, class.ClassroomId
    ,case staged.Role when N'student' then student.StudentId else schoolContact.SchoolContactId end
    ,staged.SourceHash
from [Education].[RosterEnrollment] staged
    inner join #class class on class.ExternalId = staged.ClassExternalId
    left join #student student
        on staged.Role = N'student'
        and student.ExternalId = staged.PersonExternalId
    left join [Education].[RosterRole] schoolRole
        on staged.Role = N'teacher'
        and schoolRole.RosterRunId = @RosterRunId
        and schoolRole.PersonExternalId = staged.PersonExternalId
        and schoolRole.SchoolExternalId = staged.SchoolExternalId
        and schoolRole.Role = N'teacher'
    left join #schoolContact schoolContact on schoolContact.ExternalId = schoolRole.ExternalId
    left join [Education].[RosterLink-Active] link
        on link.RosterSourceId = @sourceId
        and link.Kind = N'enrollment'
        and link.ExternalId = staged.ExternalId
where staged.RosterRunId = @RosterRunId
    and staged.Status = N'active'
    and ((staged.Role = N'student' and student.StudentId is not null)
        or (staged.Role = N'teacher' and schoolContact.SchoolContactId is not null))

create table #membership (
     Role nvarchar(25) not null
    ,ClassroomId uniqueidentifier not null
    ,MemberId uniqueidentifier not null
    ,LocalId uniqueidentifier not null
    ,MatchCount int not null
    ,primary key (Role, ClassroomId, MemberId)
)

insert #membership
select N'student', enrollment.ClassroomId, enrollment.MemberId, min(membership.Id), count(*)
from #enrollment enrollment
    inner join [Education].[ClassroomStudent-Active] membership
        on enrollment.Role = N'student'
        and membership.ClassroomId = enrollment.ClassroomId
        and membership.StudentId = enrollment.MemberId
group by enrollment.ClassroomId, enrollment.MemberId

insert #membership
select N'teacher', enrollment.ClassroomId, enrollment.MemberId, min(membership.Id), count(*)
from #enrollment enrollment
    inner join [Education].[ClassroomTeacher-Active] membership
        on enrollment.Role = N'teacher'
        and membership.ClassroomId = enrollment.ClassroomId
        and membership.SchoolContactId = enrollment.MemberId
group by enrollment.ClassroomId, enrollment.MemberId

if exists (select 1 from #membership where MatchCount > 1)
    throw 50000, 'An enrollment matches more than one existing classroom membership.', 1

if exists (
    select 1
    from #enrollment enrollment
        inner join #membership membership
            on membership.Role = enrollment.Role
            and membership.ClassroomId = enrollment.ClassroomId
            and membership.MemberId = enrollment.MemberId
        inner join [Education].[RosterLink-Active] link
            on link.Kind = N'enrollment'
            and link.LocalId = membership.LocalId
    where enrollment.LocalId is null
)
    throw 50000, 'An existing classroom membership is already managed by another roster enrollment.', 1

if exists (
    select 1
    from #enrollment enrollment
    where enrollment.LocalId is not null
        and ((enrollment.Role = N'student' and exists (
                select 1 from [Education].[ClassroomTeacher-Active] where Id = enrollment.LocalId
            ))
            or (enrollment.Role = N'teacher' and exists (
                select 1 from [Education].[ClassroomStudent-Active] where Id = enrollment.LocalId
            )))
)
    throw 50000, 'A roster enrollment changed between student and teacher membership types.', 1

if exists (
    select 1
    from #enrollment enrollment
        inner join #membership membership
            on membership.Role = enrollment.Role
            and membership.ClassroomId = enrollment.ClassroomId
            and membership.MemberId = enrollment.MemberId
    where enrollment.LocalId is not null
        and enrollment.LocalId <> membership.LocalId
)
    throw 50000, 'A roster enrollment conflicts with an existing classroom membership.', 1

update enrollment
set LocalId = coalesce(enrollment.LocalId, membership.LocalId, newid())
from #enrollment enrollment
    left join #membership membership
        on membership.Role = enrollment.Role
        and membership.ClassroomId = enrollment.ClassroomId
        and membership.MemberId = enrollment.MemberId

update membership
set ClassroomId = enrollment.ClassroomId
    ,StudentId = enrollment.MemberId
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Education].[ClassroomStudent] membership
    inner join #enrollment enrollment on enrollment.Role = N'student' and enrollment.LocalId = membership.Id
where membership.VersionOf = membership.Id
    and membership.IsDeleted = 0
    and (membership.ClassroomId <> enrollment.ClassroomId or membership.StudentId <> enrollment.MemberId)

update membership
set ClassroomId = enrollment.ClassroomId
    ,SchoolContactId = enrollment.MemberId
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Education].[ClassroomTeacher] membership
    inner join #enrollment enrollment on enrollment.Role = N'teacher' and enrollment.LocalId = membership.Id
where membership.VersionOf = membership.Id
    and membership.IsDeleted = 0
    and (membership.ClassroomId <> enrollment.ClassroomId or membership.SchoolContactId <> enrollment.MemberId)

insert [Education].[ClassroomStudent] (Id, VersionOf, Updated, UpdatedBy, ClassroomId, StudentId)
select LocalId, LocalId, @now, @updatedBy, ClassroomId, MemberId
from #enrollment enrollment
where Role = N'student'
    and not exists (select 1 from [Education].[ClassroomStudent-Active] where Id = enrollment.LocalId)

insert [Education].[ClassroomTeacher] (Id, VersionOf, Updated, UpdatedBy, ClassroomId, SchoolContactId)
select LocalId, LocalId, @now, @updatedBy, ClassroomId, MemberId
from #enrollment enrollment
where Role = N'teacher'
    and not exists (select 1 from [Education].[ClassroomTeacher-Active] where Id = enrollment.LocalId)

insert [Education].[RosterLink] (Id, VersionOf, Updated, UpdatedBy, RosterSourceId, Kind, ExternalId, LocalId, SourceHash)
select value.Id, value.Id, @now, @updatedBy, @sourceId, N'enrollment', value.ExternalId, value.LocalId, value.SourceHash
from (
    select newid() Id, enrollment.ExternalId, enrollment.LocalId, enrollment.SourceHash
    from #enrollment enrollment
    where not exists (
        select 1 from [Education].[RosterLink-Active]
        where RosterSourceId = @sourceId and Kind = N'enrollment' and ExternalId = enrollment.ExternalId
    )
) value

create table #identity (
     ExternalId nvarchar(255) not null primary key
    ,IdentityId uniqueidentifier not null
    ,Issuer nvarchar(500) not null
    ,Subject nvarchar(255) not null
    ,ProviderRole nvarchar(50) null
    ,KeyHash binary(32) not null
)

insert #identity
select staged.ExternalId, coalesce(identityTable.Id, newid()), staged.AuthIssuer, staged.AuthSubject
    ,case role.Role when N'district-admin' then N'district_admin' else role.Role end
    ,hashbytes('SHA2_256', convert(varbinary(max), convert(varchar(max), concat(
        datalength(convert(varchar(max), lower(@provider))), N':', lower(@provider),
        datalength(convert(varchar(max), staged.AuthIssuer)), N':', staged.AuthIssuer,
        datalength(convert(varchar(max), staged.AuthSubject)), N':', staged.AuthSubject
    )) collate Latin1_General_100_BIN2_UTF8))
from [Education].[RosterPerson] staged
    inner join [EducationRostering].[RosterSelection](@RosterRunId, @sourceId) selected
        on selected.Kind = N'person'
        and selected.ExternalId = staged.ExternalId
    outer apply (
        select top 1 Role
        from [Education].[RosterRole]
        where RosterRunId = @RosterRunId
            and PersonExternalId = staged.ExternalId
        order by case Role when N'district-admin' then 1 when N'staff' then 2 when N'teacher' then 3 else 4 end
    ) role
    left join [Framework].[ExternalIdentity-Active] identityTable
        on identityTable.Provider collate Latin1_General_100_BIN2 = @provider collate Latin1_General_100_BIN2
        and identityTable.Issuer collate Latin1_General_100_BIN2 = staged.AuthIssuer collate Latin1_General_100_BIN2
        and identityTable.Subject collate Latin1_General_100_BIN2 = staged.AuthSubject collate Latin1_General_100_BIN2
where staged.RosterRunId = @RosterRunId
    and staged.Status = N'active'
    and staged.AuthIssuer is not null
    and staged.AuthSubject is not null

insert [Framework].[ExternalIdentity] (
    Id, Created, Provider, Issuer, Subject, Tenant, ProviderRole, Enabled, LastSeen, KeyHash
)
select IdentityId, @now, lower(@provider), Issuer, Subject, @tenant, ProviderRole, 1, @now, KeyHash
from #identity identityValue
where not exists (select 1 from [Framework].[ExternalIdentity-Active] where Id = identityValue.IdentityId)

update identityTable
set Tenant = @tenant
    ,ProviderRole = identityValue.ProviderRole
    ,Enabled = 1
    ,LastSeen = @now
    ,KeyHash = identityValue.KeyHash
from [Framework].[ExternalIdentity] identityTable
    inner join #identity identityValue on identityValue.IdentityId = identityTable.Id
where identityTable.IsDeleted = 0
    and (identityTable.Tenant <> @tenant
        or isnull(identityTable.ProviderRole, N'') <> isnull(identityValue.ProviderRole, N'')
        or identityTable.Enabled = 0
        or identityTable.KeyHash <> identityValue.KeyHash)

create table #identityUser (
     IdentityId uniqueidentifier not null
    ,UserId uniqueidentifier not null
    ,PortalId uniqueidentifier not null
    ,primary key (IdentityId, UserId)
)

insert #identityUser
select linked.IdentityId, linked.UserId, [user].PortalId
from (
    select identityValue.IdentityId, student.UserId
    from #identity identityValue inner join #student student on student.ExternalId = identityValue.ExternalId
    union
    select identityValue.IdentityId, schoolUser.UserId
    from #identity identityValue inner join #schoolUser schoolUser on schoolUser.ExternalId = identityValue.ExternalId
    union
    select identityValue.IdentityId, districtContact.UserId
    from #identity identityValue inner join #districtContact districtContact on districtContact.PersonExternalId = identityValue.ExternalId
) linked
    inner join [Framework].[User-Active] [user] on [user].Id = linked.UserId

update link
set IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Framework].[ExternalIdentityLink] link
    inner join [Framework].[User-Active] [user] on [user].Id = link.UserId
where link.IsDeleted = 0
    and exists (
        select 1
        from #identityUser expected
        where expected.IdentityId = link.ExternalIdentityId
    )
    and not exists (
        select 1
        from #identityUser expected
        where expected.IdentityId = link.ExternalIdentityId and expected.UserId = link.UserId
    )

insert [Framework].[ExternalIdentityLink] (
    Id, Updated, UpdatedBy, ExternalIdentityId, UserId, Method, Approved, ApprovedById
)
select newid(), @now, @updatedBy, linked.IdentityId, linked.UserId, N'roster', @now, null
from #identityUser linked
where not exists (
    select 1
    from [Framework].[ExternalIdentityLink-Active]
    where ExternalIdentityId = linked.IdentityId
        and UserId = linked.UserId
)