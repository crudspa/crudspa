create proc [ContentMessaging].[MoreRosterSelect] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@OrganizationId uniqueidentifier
    ,@PopulationKey nvarchar(75)
    ,@ActivationScopeId uniqueidentifier = null
) as

set nocount on

declare @audience nvarchar(50)
declare @scopeGradeId uniqueidentifier
declare @scopeOrganizationId uniqueidentifier
declare @accessOrganizationId uniqueidentifier = @OrganizationId
declare @lessonStart date

declare @populations table (
     [Key] nvarchar(75) primary key
    ,Audience nvarchar(50) not null
)

insert @populations ([Key], Audience)
values
     (N'more-all-teachers', N'AllTeachers')
    ,(N'more-treatment-teachers', N'TreatmentTeachers')
    ,(N'more-control-teachers', N'ControlTeachers')
    ,(N'more-all-teacher-leaders', N'TeacherLeaders')
    ,(N'more-control-teacher-leaders', N'ControlTeacherLeaders')
    ,(N'more-treatment-teacher-leaders', N'TreatmentTeacherLeaders')
    ,(N'more-all-principals', N'Principals')
    ,(N'more-control-principals', N'ControlPrincipals')
    ,(N'more-treatment-principals', N'TreatmentPrincipals')
    ,(N'more-all-district-leaders', N'DistrictLeaders')

select @audience = Audience
from @populations
where [Key] = @PopulationKey

if @audience is null
    throw 51000, 'Population key is not a MORE roster population.', 1

select
     @scopeGradeId = schedule.GradeId
    ,@scopeOrganizationId = scope.OrganizationId
    ,@accessOrganizationId = activation.OrganizationId
    ,@lessonStart = schedule.LessonStart
from [Content].[ActivationScope-Active] scope
    inner join [Content].[Activation-Active] activation on activation.Id = scope.ActivationId
    inner join [Content].[CampaignSchedule-Active] schedule on schedule.ActivationScopeId = scope.Id
where scope.Id = @ActivationScopeId
    and scope.OrganizationId = @OrganizationId

if @ActivationScopeId is not null and @scopeOrganizationId is null
    throw 51000, 'Population scope is invalid.', 1

if not exists (select 1 from [ContentMessaging].[SessionCanReadOrganization](@SessionId, @PortalId, @accessOrganizationId))
    throw 51000, 'Population access denied.', 1

declare @schoolYearId uniqueidentifier = (
    select top 1 Id
    from [Education].[SchoolYear-Active]
    where coalesce(@lessonStart, convert(date, sysdatetime())) between Starts and Ends
    order by Starts desc
)

if @audience <> N'DistrictLeaders' and @schoolYearId is null
    throw 51000, 'No School Year was found for the effective lesson start date.', 1

declare @districts table (
     DistrictId uniqueidentifier primary key
    ,DistrictName nvarchar(150) not null
    ,OrganizationId uniqueidentifier not null
)

insert @districts (DistrictId, DistrictName, OrganizationId)
select district.Id, organization.Name, district.OrganizationId
from [Education].[District-Active] district
    inner join [Framework].[Organization-Active] organization on organization.Id = district.OrganizationId
where district.OrganizationId = @OrganizationId

insert @districts (DistrictId, DistrictName, OrganizationId)
select distinct district.Id, districtOrganization.Name, district.OrganizationId
from [Education].[School-Active] school
    inner join [Education].[District-Active] district on district.Id = school.DistrictId
    inner join [Framework].[Organization-Active] districtOrganization on districtOrganization.Id = district.OrganizationId
where school.OrganizationId = @OrganizationId
    and not exists (select 1 from @districts existing where existing.DistrictId = district.Id)

declare @schools table (
     SchoolId uniqueidentifier primary key
    ,SchoolName nvarchar(150) not null
    ,DistrictId uniqueidentifier not null
    ,DistrictName nvarchar(150) not null
    ,Treatment bit not null
)

insert @schools (SchoolId, SchoolName, DistrictId, DistrictName, Treatment)
select
     school.Id
    ,organization.Name
    ,district.DistrictId
    ,district.DistrictName
    ,school.Treatment
from @districts district
    inner join [Education].[School-Active] school on school.DistrictId = district.DistrictId
    inner join [Framework].[Organization-Active] organization on organization.Id = school.OrganizationId
where school.OrganizationId = @OrganizationId
    or district.OrganizationId = @OrganizationId

declare @recipient table (
     ContactId uniqueidentifier not null
    ,FirstName nvarchar(75) null
    ,LastName nvarchar(75) null
    ,Email nvarchar(75) not null
    ,SchoolId uniqueidentifier null
    ,SchoolName nvarchar(150) null
    ,DistrictName nvarchar(150) null
    ,Title nvarchar(150) null
    ,RoleName nvarchar(75) not null
)

if @audience in (N'AllTeachers', N'TreatmentTeachers', N'ControlTeachers')
begin
    insert @recipient (ContactId, FirstName, LastName, Email, SchoolId, SchoolName, DistrictName, Title, RoleName)
    select distinct
         contact.Id
        ,contact.FirstName
        ,contact.LastName
        ,contactEmail.Email
        ,school.SchoolId
        ,school.SchoolName
        ,school.DistrictName
        ,title.Name
        ,case
            when @audience = N'TreatmentTeachers' then N'Treatment Teacher'
            when @audience = N'ControlTeachers' then N'Control Teacher'
            else N'Teacher'
         end
    from @schools school
        inner join [Education].[Classroom-Active] classroom on classroom.SchoolId = school.SchoolId
        inner join [Education].[ClassroomTeacher-Active] classroomTeacher on classroomTeacher.ClassroomId = classroom.Id
        inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.Id = classroomTeacher.SchoolContactId
        inner join [Education].[Title-Active] title on title.Id = schoolContact.TitleId
        inner join [Education].[SchoolContactSchoolYear-Active] schoolContactSchoolYear on schoolContactSchoolYear.SchoolContactId = schoolContact.Id
            and schoolContactSchoolYear.SchoolYearId = @schoolYearId
        inner join [Framework].[Contact-Active] contact on contact.Id = schoolContact.ContactId
        inner join [Framework].[ContactEmail-Active] contactEmail on contactEmail.ContactId = contact.Id
            and contactEmail.Ordinal = 0
    where classroom.SchoolYearId = @schoolYearId
        and classroom.SmallClassroom = 0
        and classroom.GradeId is not null
        and schoolContact.TestAccount = 0
        and (@scopeGradeId is null or classroom.GradeId = @scopeGradeId)
        and (
            @audience = N'AllTeachers'
            or (@audience = N'TreatmentTeachers' and school.Treatment = 1)
            or (@audience = N'ControlTeachers' and school.Treatment = 0)
        )
end

if @audience in (N'TeacherLeaders', N'ControlTeacherLeaders', N'TreatmentTeacherLeaders', N'Principals', N'ControlPrincipals', N'TreatmentPrincipals')
begin
    insert @recipient (ContactId, FirstName, LastName, Email, SchoolId, SchoolName, DistrictName, Title, RoleName)
    select distinct
         contact.Id
        ,contact.FirstName
        ,contact.LastName
        ,contactEmail.Email
        ,school.SchoolId
        ,school.SchoolName
        ,school.DistrictName
        ,title.Name
        ,role.Name
    from @schools school
        inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.SchoolId = school.SchoolId
        inner join [Education].[Title-Active] title on title.Id = schoolContact.TitleId
        inner join [Education].[SchoolContactSchoolYear-Active] schoolContactSchoolYear on schoolContactSchoolYear.SchoolContactId = schoolContact.Id
            and schoolContactSchoolYear.SchoolYearId = @schoolYearId
        inner join [Framework].[User-Active] userTable on userTable.Id = schoolContact.UserId
        inner join [Framework].[UserRole-Active] userRole on userRole.UserId = userTable.Id
        inner join [Framework].[Role-Active] role on role.Id = userRole.RoleId
        inner join [Framework].[Contact-Active] contact on contact.Id = schoolContact.ContactId
        inner join [Framework].[ContactEmail-Active] contactEmail on contactEmail.ContactId = contact.Id
            and contactEmail.Ordinal = 0
    where schoolContact.TestAccount = 0
        and (
            (@audience in (N'TeacherLeaders', N'ControlTeacherLeaders', N'TreatmentTeacherLeaders') and role.Name = N'MORE Teacher Leader')
            or (@audience in (N'Principals', N'ControlPrincipals', N'TreatmentPrincipals') and role.Name = N'Principal/Staff')
        )
        and (@audience not in (N'ControlTeacherLeaders', N'TreatmentTeacherLeaders', N'ControlPrincipals', N'TreatmentPrincipals')
            or (@audience in (N'ControlTeacherLeaders', N'ControlPrincipals') and school.Treatment = 0)
            or (@audience in (N'TreatmentTeacherLeaders', N'TreatmentPrincipals') and school.Treatment = 1))
        and (
            @scopeGradeId is null
            or exists (
                select 1
                from [Education].[Classroom-Active] classroom
                where classroom.SchoolId = school.SchoolId
                    and classroom.SchoolYearId = @schoolYearId
                    and classroom.SmallClassroom = 0
                    and classroom.GradeId = @scopeGradeId
            )
        )
end

if @audience = N'DistrictLeaders'
begin
    insert @recipient (ContactId, FirstName, LastName, Email, SchoolId, SchoolName, DistrictName, Title, RoleName)
    select distinct
         contact.Id
        ,contact.FirstName
        ,contact.LastName
        ,contactEmail.Email
        ,null
        ,null
        ,district.DistrictName
        ,districtContact.Title
        ,role.Name
    from @districts district
        inner join [Education].[DistrictContact-Active] districtContact on districtContact.DistrictId = district.DistrictId
        inner join [Framework].[User-Active] userTable on userTable.Id = districtContact.UserId
        inner join [Framework].[UserRole-Active] userRole on userRole.UserId = userTable.Id
        inner join [Framework].[Role-Active] role on role.Id = userRole.RoleId
        inner join [Framework].[Contact-Active] contact on contact.Id = districtContact.ContactId
        inner join [Framework].[ContactEmail-Active] contactEmail on contactEmail.ContactId = contact.Id
            and contactEmail.Ordinal = 0
    where role.Name in (N'Admin', N'Community Coordinator', N'District Leader', N'Executive Director')
end

declare @moreTeacherLeaders table (
     SchoolId uniqueidentifier not null
    ,ContactId uniqueidentifier not null
    ,FullName nvarchar(151) not null
    ,primary key (SchoolId, ContactId)
)

insert @moreTeacherLeaders (SchoolId, ContactId, FullName)
select distinct
     school.SchoolId
    ,contact.Id
    ,trim(concat(contact.FirstName, N' ', contact.LastName))
from @schools school
    inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.SchoolId = school.SchoolId
    inner join [Education].[SchoolContactSchoolYear-Active] schoolContactSchoolYear on schoolContactSchoolYear.SchoolContactId = schoolContact.Id
        and schoolContactSchoolYear.SchoolYearId = @schoolYearId
    inner join [Framework].[UserRole-Active] userRole on userRole.UserId = schoolContact.UserId
    inner join [Framework].[Role-Active] role on role.Id = userRole.RoleId
        and role.Name = N'MORE Teacher Leader'
    inner join [Framework].[Contact-Active] contact on contact.Id = schoolContact.ContactId
where schoolContact.TestAccount = 0
    and trim(concat(contact.FirstName, N' ', contact.LastName)) <> N''

declare @metricToken table (
     Metric nvarchar(50) not null
    ,SchoolId uniqueidentifier not null
    ,GradeId uniqueidentifier null
    ,CompletedCount int not null
    ,TotalCount int not null
    ,Percentage decimal(5, 1) not null
)

if @schoolYearId is not null
begin
insert @metricToken (Metric, SchoolId, GradeId, CompletedCount, TotalCount, Percentage)
exec [ContentMessaging].[MoreEngagementMetricSelect]
     @SchoolYearId = @schoolYearId
    ,@OrganizationId = @accessOrganizationId
    ,@GradeId = @scopeGradeId
end

;with ContactBase as (
    select
         ContactId
        ,min(FirstName) as FirstName
        ,min(LastName) as LastName
        ,min(Email) as Email
    from @recipient
    group by ContactId
)
select
     contact.ContactId
    ,contact.FirstName
    ,contact.LastName
    ,contact.Email
    ,coalesce(schools.Names, N'') as SchoolNames
    ,coalesce(districts.Names, N'') as DistrictNames
    ,coalesce(roles.Names, N'') as RoleNames
    ,coalesce(titles.Names, roles.Names, N'') as Title
    ,coalesce(schools.Names, N'') as SchoolName
    ,coalesce(districts.Names, N'') as DistrictName
    ,coalesce(teacherLeaders.Names, N'') as MORETeacherLeader
    ,coalesce(format(@lessonStart, N'MMMM d, yyyy', N'en-US'), N'') as LessonStartDate
    ,schoolMetrics.TeacherLoginPercentage as SchoolTeacherLoginPercentage
    ,schoolMetrics.TeacherDigital101Percentage as SchoolMOREDigital101Percentage
    ,schoolMetrics.StudentLoginPercentage as SchoolStudentSigninPercentage
    ,schoolMetrics.StudentGamePercentage as SchoolStudentOneGamePercentage
    ,schoolMetrics.ScienceVocabularyPercentage as SchoolScienceVocabularyPercentage
    ,schoolMetrics.ScienceCcPercentage as SchoolScienceCCPercentage
    ,schoolMetrics.SocialStudiesVocabularyPercentage as SchoolSSVocabularyPercentage
    ,schoolMetrics.SocialStudiesCcPercentage as SchoolSSCCPercentage
    ,schoolMetrics.AudioRecordingPercentage as SchoolOneTeacherAudioPercentage
    ,schoolMetrics.AllFourAudioRecordingPercentage as SchoolAllFourTeacherAudioPercentage
    ,districtMetrics.TeacherLoginPercentage as DistrictTeacherLoginPercentage
    ,districtMetrics.TeacherDigital101Percentage as DistrictMOREDigital101Percentage
    ,districtMetrics.StudentLoginPercentage as DistrictStudentSigninPercentage
    ,districtMetrics.StudentGamePercentage as DistrictStudentOneGamePercentage
    ,districtMetrics.ScienceVocabularyPercentage as DistrictScienceVocabularyPercentage
    ,districtMetrics.ScienceCcPercentage as DistrictScienceCCPercentage
    ,districtMetrics.SocialStudiesVocabularyPercentage as DistrictSSVocabularyPercentage
    ,districtMetrics.SocialStudiesCcPercentage as DistrictSSCCPercentage
    ,districtMetrics.AudioRecordingPercentage as DistrictOneTeacherAudioPercentage
    ,districtMetrics.AllFourAudioRecordingPercentage as DistrictAllFourTeacherAudioPercentage
from ContactBase contact
outer apply (
    select string_agg(school.SchoolName, N', ') within group (order by school.SchoolName) as Names
    from (
        select distinct SchoolName
        from @recipient
        where ContactId = contact.ContactId and SchoolName is not null
    ) school
) schools
outer apply (
    select string_agg(district.DistrictName, N', ') within group (order by district.DistrictName) as Names
    from (
        select distinct DistrictName
        from @recipient
        where ContactId = contact.ContactId and DistrictName is not null
    ) district
) districts
outer apply (
    select string_agg(role.RoleName, N', ') within group (order by role.RoleName) as Names
    from (
        select distinct RoleName
        from @recipient
        where ContactId = contact.ContactId
    ) role
) roles
outer apply (
    select string_agg(title.Title, N', ') within group (order by title.Title) as Names
    from (
        select distinct Title
        from @recipient
        where ContactId = contact.ContactId and Title is not null
    ) title
) titles
outer apply (
    select string_agg(teacherLeader.FullName, N', ') within group (order by teacherLeader.FullName) as Names
    from (
        select distinct leader.FullName
        from @moreTeacherLeaders leader
        where exists (
            select 1
            from @recipient represented
            where represented.ContactId = contact.ContactId
                and (represented.SchoolId is null or represented.SchoolId = leader.SchoolId)
        )
    ) teacherLeader
) teacherLeaders
outer apply (
    select
         case when sum(case when metric.Metric = N'TeacherLogin' then metric.TotalCount else 0 end) > 0
            then format(cast(100.0 * sum(case when metric.Metric = N'TeacherLogin' then metric.CompletedCount else 0 end) / sum(case when metric.Metric = N'TeacherLogin' then metric.TotalCount else 0 end) as decimal(5, 1)), N'0.#') + N'%' else N'' end as TeacherLoginPercentage
        ,case when sum(case when metric.Metric = N'TeacherDigital101' then metric.TotalCount else 0 end) > 0
            then format(cast(100.0 * sum(case when metric.Metric = N'TeacherDigital101' then metric.CompletedCount else 0 end) / sum(case when metric.Metric = N'TeacherDigital101' then metric.TotalCount else 0 end) as decimal(5, 1)), N'0.#') + N'%' else N'' end as TeacherDigital101Percentage
        ,case when sum(case when metric.Metric = N'StudentLogin' then metric.TotalCount else 0 end) > 0
            then format(cast(100.0 * sum(case when metric.Metric = N'StudentLogin' then metric.CompletedCount else 0 end) / sum(case when metric.Metric = N'StudentLogin' then metric.TotalCount else 0 end) as decimal(5, 1)), N'0.#') + N'%' else N'' end as StudentLoginPercentage
        ,case when sum(case when metric.Metric = N'StudentGame' then metric.TotalCount else 0 end) > 0
            then format(cast(100.0 * sum(case when metric.Metric = N'StudentGame' then metric.CompletedCount else 0 end) / sum(case when metric.Metric = N'StudentGame' then metric.TotalCount else 0 end) as decimal(5, 1)), N'0.#') + N'%' else N'' end as StudentGamePercentage
        ,case when sum(case when metric.Metric = N'ScienceVocabulary' then metric.TotalCount else 0 end) > 0
            then format(cast(100.0 * sum(case when metric.Metric = N'ScienceVocabulary' then metric.CompletedCount else 0 end) / sum(case when metric.Metric = N'ScienceVocabulary' then metric.TotalCount else 0 end) as decimal(5, 1)), N'0.#') + N'%' else N'' end as ScienceVocabularyPercentage
        ,case when sum(case when metric.Metric = N'ScienceCc' then metric.TotalCount else 0 end) > 0
            then format(cast(100.0 * sum(case when metric.Metric = N'ScienceCc' then metric.CompletedCount else 0 end) / sum(case when metric.Metric = N'ScienceCc' then metric.TotalCount else 0 end) as decimal(5, 1)), N'0.#') + N'%' else N'' end as ScienceCcPercentage
        ,case when sum(case when metric.Metric = N'SocialStudiesVocabulary' then metric.TotalCount else 0 end) > 0
            then format(cast(100.0 * sum(case when metric.Metric = N'SocialStudiesVocabulary' then metric.CompletedCount else 0 end) / sum(case when metric.Metric = N'SocialStudiesVocabulary' then metric.TotalCount else 0 end) as decimal(5, 1)), N'0.#') + N'%' else N'' end as SocialStudiesVocabularyPercentage
        ,case when sum(case when metric.Metric = N'SocialStudiesCc' then metric.TotalCount else 0 end) > 0
            then format(cast(100.0 * sum(case when metric.Metric = N'SocialStudiesCc' then metric.CompletedCount else 0 end) / sum(case when metric.Metric = N'SocialStudiesCc' then metric.TotalCount else 0 end) as decimal(5, 1)), N'0.#') + N'%' else N'' end as SocialStudiesCcPercentage
        ,case when sum(case when metric.Metric = N'AudioRecording' then metric.TotalCount else 0 end) > 0
            then format(cast(100.0 * sum(case when metric.Metric = N'AudioRecording' then metric.CompletedCount else 0 end) / sum(case when metric.Metric = N'AudioRecording' then metric.TotalCount else 0 end) as decimal(5, 1)), N'0.#') + N'%' else N'' end as AudioRecordingPercentage
        ,case when sum(case when metric.Metric = N'AllFourAudioRecording' then metric.TotalCount else 0 end) > 0
            then format(cast(100.0 * sum(case when metric.Metric = N'AllFourAudioRecording' then metric.CompletedCount else 0 end) / sum(case when metric.Metric = N'AllFourAudioRecording' then metric.TotalCount else 0 end) as decimal(5, 1)), N'0.#') + N'%' else N'' end as AllFourAudioRecordingPercentage
    from @metricToken metric
    where metric.GradeId is null
        and exists (
            select 1
            from @recipient represented
            where represented.ContactId = contact.ContactId
                and (represented.SchoolId is null or represented.SchoolId = metric.SchoolId)
        )
) schoolMetrics
outer apply (
    select
         case when sum(case when metric.Metric = N'TeacherLogin' then metric.TotalCount else 0 end) > 0
            then format(cast(100.0 * sum(case when metric.Metric = N'TeacherLogin' then metric.CompletedCount else 0 end) / sum(case when metric.Metric = N'TeacherLogin' then metric.TotalCount else 0 end) as decimal(5, 1)), N'0.#') + N'%' else N'' end as TeacherLoginPercentage
        ,case when sum(case when metric.Metric = N'TeacherDigital101' then metric.TotalCount else 0 end) > 0
            then format(cast(100.0 * sum(case when metric.Metric = N'TeacherDigital101' then metric.CompletedCount else 0 end) / sum(case when metric.Metric = N'TeacherDigital101' then metric.TotalCount else 0 end) as decimal(5, 1)), N'0.#') + N'%' else N'' end as TeacherDigital101Percentage
        ,case when sum(case when metric.Metric = N'StudentLogin' then metric.TotalCount else 0 end) > 0
            then format(cast(100.0 * sum(case when metric.Metric = N'StudentLogin' then metric.CompletedCount else 0 end) / sum(case when metric.Metric = N'StudentLogin' then metric.TotalCount else 0 end) as decimal(5, 1)), N'0.#') + N'%' else N'' end as StudentLoginPercentage
        ,case when sum(case when metric.Metric = N'StudentGame' then metric.TotalCount else 0 end) > 0
            then format(cast(100.0 * sum(case when metric.Metric = N'StudentGame' then metric.CompletedCount else 0 end) / sum(case when metric.Metric = N'StudentGame' then metric.TotalCount else 0 end) as decimal(5, 1)), N'0.#') + N'%' else N'' end as StudentGamePercentage
        ,case when sum(case when metric.Metric = N'ScienceVocabulary' then metric.TotalCount else 0 end) > 0
            then format(cast(100.0 * sum(case when metric.Metric = N'ScienceVocabulary' then metric.CompletedCount else 0 end) / sum(case when metric.Metric = N'ScienceVocabulary' then metric.TotalCount else 0 end) as decimal(5, 1)), N'0.#') + N'%' else N'' end as ScienceVocabularyPercentage
        ,case when sum(case when metric.Metric = N'ScienceCc' then metric.TotalCount else 0 end) > 0
            then format(cast(100.0 * sum(case when metric.Metric = N'ScienceCc' then metric.CompletedCount else 0 end) / sum(case when metric.Metric = N'ScienceCc' then metric.TotalCount else 0 end) as decimal(5, 1)), N'0.#') + N'%' else N'' end as ScienceCcPercentage
        ,case when sum(case when metric.Metric = N'SocialStudiesVocabulary' then metric.TotalCount else 0 end) > 0
            then format(cast(100.0 * sum(case when metric.Metric = N'SocialStudiesVocabulary' then metric.CompletedCount else 0 end) / sum(case when metric.Metric = N'SocialStudiesVocabulary' then metric.TotalCount else 0 end) as decimal(5, 1)), N'0.#') + N'%' else N'' end as SocialStudiesVocabularyPercentage
        ,case when sum(case when metric.Metric = N'SocialStudiesCc' then metric.TotalCount else 0 end) > 0
            then format(cast(100.0 * sum(case when metric.Metric = N'SocialStudiesCc' then metric.CompletedCount else 0 end) / sum(case when metric.Metric = N'SocialStudiesCc' then metric.TotalCount else 0 end) as decimal(5, 1)), N'0.#') + N'%' else N'' end as SocialStudiesCcPercentage
        ,case when sum(case when metric.Metric = N'AudioRecording' then metric.TotalCount else 0 end) > 0
            then format(cast(100.0 * sum(case when metric.Metric = N'AudioRecording' then metric.CompletedCount else 0 end) / sum(case when metric.Metric = N'AudioRecording' then metric.TotalCount else 0 end) as decimal(5, 1)), N'0.#') + N'%' else N'' end as AudioRecordingPercentage
        ,case when sum(case when metric.Metric = N'AllFourAudioRecording' then metric.TotalCount else 0 end) > 0
            then format(cast(100.0 * sum(case when metric.Metric = N'AllFourAudioRecording' then metric.CompletedCount else 0 end) / sum(case when metric.Metric = N'AllFourAudioRecording' then metric.TotalCount else 0 end) as decimal(5, 1)), N'0.#') + N'%' else N'' end as AllFourAudioRecordingPercentage
    from @metricToken metric
    where metric.GradeId is null
) districtMetrics
order by contact.LastName, contact.FirstName, contact.ContactId
option (recompile)