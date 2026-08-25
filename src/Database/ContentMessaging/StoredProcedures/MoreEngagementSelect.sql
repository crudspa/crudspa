create proc [ContentMessaging].[MoreEngagementSelect] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@OrganizationId uniqueidentifier
    ,@PopulationKey nvarchar(75)
    ,@ActivationScopeId uniqueidentifier = null
) as

set nocount on

declare @schoolYearId uniqueidentifier
declare @metric nvarchar(50)
declare @secondaryMetric nvarchar(50)
declare @bucket nvarchar(20)
declare @fullyEngagedAt decimal(5, 1)
declare @recipientRoleName nvarchar(50)
declare @treatment bit = case when @PopulationKey like N'more-treatment-%' then 1 when @PopulationKey like N'more-control-%' then 0 end
declare @basePopulationKey nvarchar(75) = case
    when @PopulationKey like N'more-treatment-%' then N'more-' + substring(@PopulationKey, len(N'more-treatment-') + 1, 75)
    when @PopulationKey like N'more-control-%' then N'more-' + substring(@PopulationKey, len(N'more-control-') + 1, 75)
    else @PopulationKey
end
declare @scopeGradeId uniqueidentifier
declare @scopeOrganizationId uniqueidentifier
declare @accessOrganizationId uniqueidentifier = @OrganizationId
declare @lessonStart date

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

select top 1 @schoolYearId = Id
from [Education].[SchoolYear-Active]
where coalesce(@lessonStart, convert(date, sysdatetime())) between Starts and Ends
order by Starts desc

if @schoolYearId is null
    throw 51000, 'No School Year was found for the effective lesson start date.', 1

declare @populations table (
     [Key] nvarchar(75) primary key
    ,Metric nvarchar(50) not null
    ,SecondaryMetric nvarchar(50) null
    ,Bucket nvarchar(20) not null
    ,FullyEngagedAt decimal(5, 1) not null
    ,RecipientRoleName nvarchar(50) not null
)

insert @populations ([Key], Metric, Bucket, FullyEngagedAt, RecipientRoleName)
values
     (N'more-teacher-engagement-1-little', N'TeacherLogin', N'Little', 100.0, N'MORE Teacher Leader')
    ,(N'more-teacher-engagement-1-some', N'TeacherLogin', N'Some', 100.0, N'MORE Teacher Leader')
    ,(N'more-teacher-engagement-1-fully', N'TeacherLogin', N'Fully', 100.0, N'MORE Teacher Leader')
    ,(N'more-teacher-engagement-2-little', N'TeacherDigital101', N'Little', 100.0, N'MORE Teacher Leader')
    ,(N'more-teacher-engagement-2-some', N'TeacherDigital101', N'Some', 100.0, N'MORE Teacher Leader')
    ,(N'more-teacher-engagement-2-fully', N'TeacherDigital101', N'Fully', 100.0, N'MORE Teacher Leader')
    ,(N'more-student-engagement-1-little', N'StudentLogin', N'Little', 80.0, N'MORE Teacher Leader')
    ,(N'more-student-engagement-1-some', N'StudentLogin', N'Some', 80.0, N'MORE Teacher Leader')
    ,(N'more-student-engagement-1-fully', N'StudentLogin', N'Fully', 80.0, N'MORE Teacher Leader')
    ,(N'more-student-engagement-2-little', N'StudentGame', N'Little', 80.0, N'MORE Teacher Leader')
    ,(N'more-student-engagement-2-some', N'StudentGame', N'Some', 80.0, N'MORE Teacher Leader')
    ,(N'more-student-engagement-2-fully', N'StudentGame', N'Fully', 80.0, N'MORE Teacher Leader')
    ,(N'more-assessment-science-vocabulary-little', N'ScienceVocabulary', N'Little', 80.0, N'MORE Teacher Leader')
    ,(N'more-assessment-science-vocabulary-some', N'ScienceVocabulary', N'Some', 80.0, N'MORE Teacher Leader')
    ,(N'more-assessment-science-vocabulary-fully', N'ScienceVocabulary', N'Fully', 80.0, N'MORE Teacher Leader')
    ,(N'more-assessment-science-cc-little', N'ScienceCc', N'Little', 80.0, N'MORE Teacher Leader')
    ,(N'more-assessment-science-cc-some', N'ScienceCc', N'Some', 80.0, N'MORE Teacher Leader')
    ,(N'more-assessment-science-cc-fully', N'ScienceCc', N'Fully', 80.0, N'MORE Teacher Leader')
    ,(N'more-assessment-social-studies-vocabulary-little', N'SocialStudiesVocabulary', N'Little', 80.0, N'MORE Teacher Leader')
    ,(N'more-assessment-social-studies-vocabulary-some', N'SocialStudiesVocabulary', N'Some', 80.0, N'MORE Teacher Leader')
    ,(N'more-assessment-social-studies-vocabulary-fully', N'SocialStudiesVocabulary', N'Fully', 80.0, N'MORE Teacher Leader')
    ,(N'more-assessment-social-studies-cc-little', N'SocialStudiesCc', N'Little', 80.0, N'MORE Teacher Leader')
    ,(N'more-assessment-social-studies-cc-some', N'SocialStudiesCc', N'Some', 80.0, N'MORE Teacher Leader')
    ,(N'more-assessment-social-studies-cc-fully', N'SocialStudiesCc', N'Fully', 80.0, N'MORE Teacher Leader')
    ,(N'more-audio-recording-little', N'AudioRecording', N'Little', 80.0, N'MORE Teacher Leader')
    ,(N'more-audio-recording-some', N'AudioRecording', N'Some', 80.0, N'MORE Teacher Leader')
    ,(N'more-audio-recording-fully', N'AudioRecording', N'Fully', 80.0, N'MORE Teacher Leader')
    ,(N'more-audio-recording-all-four-little', N'AllFourAudioRecording', N'Little', 80.0, N'MORE Teacher Leader')
    ,(N'more-audio-recording-all-four-some', N'AllFourAudioRecording', N'Some', 80.0, N'MORE Teacher Leader')
    ,(N'more-audio-recording-all-four-fully', N'AllFourAudioRecording', N'Fully', 80.0, N'MORE Teacher Leader')
    ,(N'more-teacher-engagement-1-little-principals', N'TeacherLogin', N'Little', 100.0, N'Principal/Staff')
    ,(N'more-teacher-engagement-1-some-principals', N'TeacherLogin', N'Some', 100.0, N'Principal/Staff')
    ,(N'more-teacher-engagement-1-fully-principals', N'TeacherLogin', N'Fully', 100.0, N'Principal/Staff')
    ,(N'more-teacher-engagement-2-little-principals', N'TeacherDigital101', N'Little', 100.0, N'Principal/Staff')
    ,(N'more-teacher-engagement-2-some-principals', N'TeacherDigital101', N'Some', 100.0, N'Principal/Staff')
    ,(N'more-teacher-engagement-2-fully-principals', N'TeacherDigital101', N'Fully', 100.0, N'Principal/Staff')
    ,(N'more-student-engagement-1-little-principals', N'StudentLogin', N'Little', 80.0, N'Principal/Staff')
    ,(N'more-student-engagement-1-some-principals', N'StudentLogin', N'Some', 80.0, N'Principal/Staff')
    ,(N'more-student-engagement-1-fully-principals', N'StudentLogin', N'Fully', 80.0, N'Principal/Staff')
    ,(N'more-student-engagement-2-little-principals', N'StudentGame', N'Little', 80.0, N'Principal/Staff')
    ,(N'more-student-engagement-2-some-principals', N'StudentGame', N'Some', 80.0, N'Principal/Staff')
    ,(N'more-student-engagement-2-fully-principals', N'StudentGame', N'Fully', 80.0, N'Principal/Staff')
    ,(N'more-assessment-science-vocabulary-little-principals', N'ScienceVocabulary', N'Little', 80.0, N'Principal/Staff')
    ,(N'more-assessment-science-vocabulary-some-principals', N'ScienceVocabulary', N'Some', 80.0, N'Principal/Staff')
    ,(N'more-assessment-science-vocabulary-fully-principals', N'ScienceVocabulary', N'Fully', 80.0, N'Principal/Staff')
    ,(N'more-assessment-science-cc-little-principals', N'ScienceCc', N'Little', 80.0, N'Principal/Staff')
    ,(N'more-assessment-science-cc-some-principals', N'ScienceCc', N'Some', 80.0, N'Principal/Staff')
    ,(N'more-assessment-science-cc-fully-principals', N'ScienceCc', N'Fully', 80.0, N'Principal/Staff')
    ,(N'more-assessment-social-studies-vocabulary-little-principals', N'SocialStudiesVocabulary', N'Little', 80.0, N'Principal/Staff')
    ,(N'more-assessment-social-studies-vocabulary-some-principals', N'SocialStudiesVocabulary', N'Some', 80.0, N'Principal/Staff')
    ,(N'more-assessment-social-studies-vocabulary-fully-principals', N'SocialStudiesVocabulary', N'Fully', 80.0, N'Principal/Staff')
    ,(N'more-assessment-social-studies-cc-little-principals', N'SocialStudiesCc', N'Little', 80.0, N'Principal/Staff')
    ,(N'more-assessment-social-studies-cc-some-principals', N'SocialStudiesCc', N'Some', 80.0, N'Principal/Staff')
    ,(N'more-assessment-social-studies-cc-fully-principals', N'SocialStudiesCc', N'Fully', 80.0, N'Principal/Staff')
    ,(N'more-audio-recording-little-principals', N'AudioRecording', N'Little', 80.0, N'Principal/Staff')
    ,(N'more-audio-recording-some-principals', N'AudioRecording', N'Some', 80.0, N'Principal/Staff')
    ,(N'more-audio-recording-fully-principals', N'AudioRecording', N'Fully', 80.0, N'Principal/Staff')
    ,(N'more-audio-recording-all-four-little-principals', N'AllFourAudioRecording', N'Little', 80.0, N'Principal/Staff')
    ,(N'more-audio-recording-all-four-some-principals', N'AllFourAudioRecording', N'Some', 80.0, N'Principal/Staff')
    ,(N'more-audio-recording-all-four-fully-principals', N'AllFourAudioRecording', N'Fully', 80.0, N'Principal/Staff')

insert @populations ([Key], Metric, SecondaryMetric, Bucket, FullyEngagedAt, RecipientRoleName)
values
     (N'more-teacher-login-high-digital-101-some', N'TeacherLogin', N'TeacherDigital101', N'Composite', 100.0, N'MORE Teacher Leader')
    ,(N'more-teacher-login-high-digital-101-some-principals', N'TeacherLogin', N'TeacherDigital101', N'Composite', 100.0, N'Principal/Staff')

select
     @metric = Metric
    ,@secondaryMetric = SecondaryMetric
    ,@bucket = Bucket
    ,@fullyEngagedAt = FullyEngagedAt
    ,@recipientRoleName = RecipientRoleName
from @populations
where [Key] = @basePopulationKey

if @metric is null
    throw 51000, 'Population key is not a MORE engagement population.', 1

declare @schools table (
     SchoolId uniqueidentifier primary key
    ,Treatment bit not null
    ,SchoolName nvarchar(150) not null
    ,DistrictName nvarchar(150) not null
)
declare @engagement table (
     Metric nvarchar(50) not null
    ,SchoolId uniqueidentifier not null
    ,GradeId uniqueidentifier not null
    ,Percentage decimal(5, 1) not null
    ,primary key (Metric, SchoolId, GradeId)
)
declare @metricToken table (
     Metric nvarchar(50) not null
    ,SchoolId uniqueidentifier not null
    ,GradeId uniqueidentifier null
    ,CompletedCount int not null
    ,TotalCount int not null
    ,Percentage decimal(5, 1) not null
)

insert @schools (SchoolId, Treatment, SchoolName, DistrictName)
select school.Id, school.Treatment, schoolOrganization.Name, districtOrganization.Name
from [Education].[School-Active] school
    inner join [Education].[District-Active] district on school.DistrictId = district.Id
    inner join [Framework].[Organization-Active] schoolOrganization on schoolOrganization.Id = school.OrganizationId
    inner join [Framework].[Organization-Active] districtOrganization on districtOrganization.Id = district.OrganizationId
where school.OrganizationId = @OrganizationId
    or district.OrganizationId = @OrganizationId
    or (@ActivationScopeId is not null and school.OrganizationId = @scopeOrganizationId)

insert @metricToken (Metric, SchoolId, GradeId, CompletedCount, TotalCount, Percentage)
exec [ContentMessaging].[MoreEngagementMetricSelect]
     @SchoolYearId = @schoolYearId
    ,@OrganizationId = @accessOrganizationId
    ,@GradeId = @scopeGradeId

insert @engagement (Metric, SchoolId, GradeId, Percentage)
select Metric, SchoolId, GradeId, Percentage
from @metricToken
where Metric in (@metric, @secondaryMetric)
    and GradeId is not null
declare @leaders table (
     SchoolId uniqueidentifier not null
    ,ContactId uniqueidentifier not null
    ,FirstName nvarchar(50) null
    ,LastName nvarchar(50) null
    ,Email nvarchar(255) null
    ,SchoolName nvarchar(150) not null
    ,DistrictName nvarchar(150) not null
    ,Title nvarchar(50) null
    ,primary key (SchoolId, ContactId)
)
declare @leaderUsers table (UserId uniqueidentifier primary key)

insert @leaderUsers (UserId)
select userRole.UserId
from [Framework].[UserRole-Active] userRole
    inner join [Framework].[Role-Active] role on role.Id = userRole.RoleId
where role.Name = @recipientRoleName
group by userRole.UserId

insert @leaders (SchoolId, ContactId, FirstName, LastName, Email, SchoolName, DistrictName, Title)
select distinct
     schoolContact.SchoolId
    ,schoolContact.ContactId
    ,contact.FirstName
    ,contact.LastName
    ,contactEmail.Email
    ,scope.SchoolName
    ,scope.DistrictName
    ,title.Name
from @schools scope
    inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.SchoolId = scope.SchoolId
    inner join [Education].[Title-Active] title on title.Id = schoolContact.TitleId
    inner join @leaderUsers leaderUser on leaderUser.UserId = schoolContact.UserId
    inner join [Education].[SchoolContactSchoolYear-Active] schoolContactSchoolYear on schoolContactSchoolYear.SchoolContactId = schoolContact.Id
        and schoolContactSchoolYear.SchoolYearId = @schoolYearId
    inner join [Framework].[User-Active] userTable on userTable.Id = schoolContact.UserId
    inner join [Framework].[Contact-Active] contact on contact.Id = schoolContact.ContactId
    left join [Framework].[ContactEmail-Active] contactEmail on contactEmail.ContactId = contact.Id
        and contactEmail.Ordinal = 0
where schoolContact.TestAccount = 0
option (recompile)

declare @moreTeacherLeaders table (
     SchoolId uniqueidentifier not null
    ,ContactId uniqueidentifier not null
    ,FullName nvarchar(151) not null
    ,primary key (SchoolId, ContactId)
)

insert @moreTeacherLeaders (SchoolId, ContactId, FullName)
select distinct
     scope.SchoolId
    ,contact.Id
    ,trim(concat(contact.FirstName, N' ', contact.LastName))
from @schools scope
    inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.SchoolId = scope.SchoolId
    inner join [Education].[SchoolContactSchoolYear-Active] schoolContactSchoolYear on schoolContactSchoolYear.SchoolContactId = schoolContact.Id
        and schoolContactSchoolYear.SchoolYearId = @schoolYearId
    inner join [Framework].[UserRole-Active] userRole on userRole.UserId = schoolContact.UserId
    inner join [Framework].[Role-Active] role on role.Id = userRole.RoleId
        and role.Name = N'MORE Teacher Leader'
    inner join [Framework].[Contact-Active] contact on contact.Id = schoolContact.ContactId
where schoolContact.TestAccount = 0
    and trim(concat(contact.FirstName, N' ', contact.LastName)) <> N''

;with MatchingGrades as (
    select
         engagement.SchoolId
        ,engagement.GradeId
        ,convert(nvarchar(max), convert(nvarchar(20), engagement.Percentage) + N'%') as EngagementText
    from @engagement engagement
    where @secondaryMetric is null
        and engagement.Metric = @metric
        and (@treatment is null or exists (select 1 from @schools school where school.SchoolId = engagement.SchoolId and school.Treatment = @treatment))
        and (
            (@bucket = N'Little' and engagement.Percentage < 10.0)
            or (@bucket = N'Some' and engagement.Percentage >= 10.0 and engagement.Percentage < @fullyEngagedAt)
            or (@bucket = N'Fully' and engagement.Percentage >= @fullyEngagedAt)
        )
    union all
    select
         primaryMetric.SchoolId
        ,primaryMetric.GradeId
        ,convert(nvarchar(max), concat(
             N'Teacher sign-ins ', convert(nvarchar(20), primaryMetric.Percentage), N'%'
            ,N'; Digital 101 ', convert(nvarchar(20), secondaryMetric.Percentage), N'%'
         ))
    from @engagement primaryMetric
        inner join @engagement secondaryMetric on secondaryMetric.SchoolId = primaryMetric.SchoolId
            and secondaryMetric.GradeId = primaryMetric.GradeId
            and secondaryMetric.Metric = @secondaryMetric
    where @secondaryMetric is not null
        and primaryMetric.Metric = @metric
        and primaryMetric.Percentage > 80.0
        and secondaryMetric.Percentage >= 10.0
        and secondaryMetric.Percentage < 100.0
        and (@treatment is null or exists (select 1 from @schools school where school.SchoolId = primaryMetric.SchoolId and school.Treatment = @treatment))
),
RecipientEngagement as (
select
     leader.ContactId
    ,leader.FirstName
    ,leader.LastName
    ,leader.Email
    ,coalesce(schools.Names, N'') as SchoolNames
    ,string_agg(grade.Name, N', ') within group (order by grade.Name) as GradeNames
    ,string_agg(match.EngagementText, N', ') within group (order by grade.Name) as Engagements
    ,coalesce(titles.Names, @recipientRoleName, N'') as Title
    ,coalesce(schools.Names, N'') as SchoolName
    ,coalesce(districts.Names, N'') as DistrictName
    ,coalesce(districts.Names, N'') as DistrictNames
    ,coalesce(teacherLeaders.Names, N'') as MORETeacherLeader
from MatchingGrades match
    inner join [Education].[Grade-Active] grade on grade.Id = match.GradeId
    inner join @leaders leader on leader.SchoolId = match.SchoolId
    outer apply (
        select string_agg(school.SchoolName, N', ') within group (order by school.SchoolName) as Names
        from (
            select distinct matchingLeader.SchoolName
            from @leaders matchingLeader
            where matchingLeader.ContactId = leader.ContactId
                and exists (select 1 from MatchingGrades matching where matching.SchoolId = matchingLeader.SchoolId)
        ) school
    ) schools
    outer apply (
        select string_agg(district.DistrictName, N', ') within group (order by district.DistrictName) as Names
        from (
            select distinct matchingLeader.DistrictName
            from @leaders matchingLeader
            where matchingLeader.ContactId = leader.ContactId
                and exists (select 1 from MatchingGrades matching where matching.SchoolId = matchingLeader.SchoolId)
        ) district
    ) districts
    outer apply (
        select string_agg(title.Title, N', ') within group (order by title.Title) as Names
        from (
            select distinct matchingLeader.Title
            from @leaders matchingLeader
            where matchingLeader.ContactId = leader.ContactId
                and matchingLeader.Title is not null
                and exists (select 1 from MatchingGrades matching where matching.SchoolId = matchingLeader.SchoolId)
        ) title
    ) titles
    outer apply (
        select string_agg(teacherLeader.FullName, N', ') within group (order by teacherLeader.FullName) as Names
        from (
            select distinct moreTeacherLeader.FullName
            from @moreTeacherLeaders moreTeacherLeader
            where exists (
                select 1
                from @leaders represented
                where represented.ContactId = leader.ContactId
                    and represented.SchoolId = moreTeacherLeader.SchoolId
                    and exists (select 1 from MatchingGrades matching where matching.SchoolId = represented.SchoolId)
            )
        ) teacherLeader
    ) teacherLeaders
group by leader.ContactId, leader.FirstName, leader.LastName, leader.Email,
    schools.Names, districts.Names, titles.Names, teacherLeaders.Names
)
select
     recipient.ContactId
    ,recipient.FirstName
    ,recipient.LastName
    ,recipient.Email
    ,recipient.SchoolNames
    ,recipient.GradeNames
    ,recipient.Engagements
    ,recipient.Title
    ,recipient.SchoolName
    ,recipient.DistrictName
    ,recipient.DistrictNames
    ,recipient.MORETeacherLeader
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
from RecipientEngagement recipient
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
            from @leaders represented
            where represented.ContactId = recipient.ContactId
                and represented.SchoolId = metric.SchoolId
                and exists (select 1 from MatchingGrades matching where matching.SchoolId = represented.SchoolId)
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
order by recipient.LastName, recipient.FirstName, recipient.ContactId
option (recompile)