create proc [ContentMessaging].[MoreEngagementMetricSelect] (
     @SchoolYearId uniqueidentifier
    ,@OrganizationId uniqueidentifier
    ,@GradeId uniqueidentifier = null
)
as
begin
    set nocount on

    create table #metric (
         Metric nvarchar(50) not null
        ,SchoolId uniqueidentifier not null
        ,GradeId uniqueidentifier null
        ,CompletedCount int not null
        ,TotalCount int not null
        ,Percentage decimal(5, 1) not null
    )
    declare @schoolYearStarts datetimeoffset = (
        select Starts
        from [Education].[SchoolYear-Active]
        where Id = @SchoolYearId
    )
    declare @districtOrganizationId uniqueidentifier = coalesce(
        (select OrganizationId from [Education].[District-Active] where OrganizationId = @OrganizationId),
        (
            select district.OrganizationId
            from [Education].[School-Active] school
                inner join [Education].[District-Active] district on district.Id = school.DistrictId
            where school.OrganizationId = @OrganizationId
        )
    )
    declare @moreDigital101Id uniqueidentifier = '88140641-b066-407f-b166-e50ccabe8252'
    declare @elaCategoryId uniqueidentifier = '96132ba4-821b-4218-bafb-bf06fea916b5'
    declare @mathCategoryId uniqueidentifier = '958e756d-7f09-413a-b65f-b9ea95cbe93e'
    declare @scienceCategoryId uniqueidentifier = '69a5f32d-4d5f-4683-93b3-234ed12758c4'
    declare @socialStudiesCategoryId uniqueidentifier = 'ac5c30dd-ab8a-4921-a857-15701e8ffef0'

    create table #schools (SchoolId uniqueidentifier primary key)
    create table #teacherRoster (
         SchoolId uniqueidentifier not null
        ,GradeId uniqueidentifier not null
        ,SchoolContactId uniqueidentifier not null
        ,ContactId uniqueidentifier not null
        ,UserId uniqueidentifier not null
        ,primary key (SchoolId, GradeId, SchoolContactId)
    )
    create table #studentRoster (
         StudentId uniqueidentifier not null
        ,SchoolId uniqueidentifier not null
        ,GradeId uniqueidentifier not null
        ,UserId uniqueidentifier null
        ,primary key (StudentId, SchoolId, GradeId)
    )
    create table #signedInUsers (UserId uniqueidentifier primary key)
    create table #completedDigital101 (ContactId uniqueidentifier primary key)
    create table #completedGame (StudentId uniqueidentifier primary key)
    create table #completedAssessment (
         StudentId uniqueidentifier not null
        ,Metric nvarchar(50) not null
        ,primary key (StudentId, Metric)
    )
    create table #recordingCompletion (
         SchoolContactId uniqueidentifier primary key
        ,HasAllFour bit not null
    )

    insert #schools (SchoolId)
    select school.Id
    from [Education].[School-Active] school
        inner join [Education].[District-Active] district on district.Id = school.DistrictId
    where district.OrganizationId = @districtOrganizationId

    insert #teacherRoster (SchoolId, GradeId, SchoolContactId, ContactId, UserId)
    select distinct
         classroom.SchoolId
        ,classroom.GradeId
        ,schoolContact.Id
        ,schoolContact.ContactId
        ,schoolContact.UserId
    from #schools scope
        inner join [Education].[Classroom-Active] classroom on classroom.SchoolId = scope.SchoolId
        inner join [Education].[ClassroomTeacher-Active] classroomTeacher on classroomTeacher.ClassroomId = classroom.Id
        inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.Id = classroomTeacher.SchoolContactId
        inner join [Education].[SchoolContactSchoolYear-Active] schoolContactSchoolYear on schoolContactSchoolYear.SchoolContactId = schoolContact.Id
            and schoolContactSchoolYear.SchoolYearId = @SchoolYearId
        inner join [Framework].[User-Active] userTable on userTable.Id = schoolContact.UserId
    where classroom.SchoolYearId = @SchoolYearId
        and classroom.SmallClassroom = 0
        and classroom.GradeId is not null
        and schoolContact.TestAccount = 0
        and (@GradeId is null or classroom.GradeId = @GradeId)

    insert #studentRoster (StudentId, SchoolId, GradeId, UserId)
    select distinct
         student.Id
        ,classroom.SchoolId
        ,classroom.GradeId
        ,student.UserId
    from #schools scope
        inner join [Education].[Classroom-Active] classroom on classroom.SchoolId = scope.SchoolId
        inner join [Education].[ClassroomStudent-Active] classroomStudent on classroomStudent.ClassroomId = classroom.Id
        inner join [Education].[Student-Active] student on student.Id = classroomStudent.StudentId
        inner join [Education].[StudentSchoolYear-Active] studentSchoolYear on studentSchoolYear.StudentId = student.Id
            and studentSchoolYear.SchoolYearId = @SchoolYearId
    where classroom.SchoolYearId = @SchoolYearId
        and classroom.SmallClassroom = 0
        and classroom.GradeId is not null
        and student.DeletedBySchool = 0
        and student.IsTestAccount = 0
        and (@GradeId is null or classroom.GradeId = @GradeId)

    insert #signedInUsers (UserId)
    select distinct session.UserId
    from (
        select UserId from #teacherRoster
        union
        select UserId from #studentRoster where UserId is not null
    ) roster
        inner join [Framework].[Session] session on session.UserId = roster.UserId
    where session.Started >= @schoolYearStarts
    group by session.UserId

    insert #completedDigital101 (ContactId)
    select distinct completed.ContactId
    from (select distinct ContactId from #teacherRoster) teacher
        inner join [Content].[CourseCompleted-Active] completed on completed.ContactId = teacher.ContactId
    where completed.CourseId = @moreDigital101Id

    insert #completedGame (StudentId)
    select distinct gameCompleted.StudentId
    from (select distinct StudentId from #studentRoster) student
        inner join [Education].[GameCompleted] gameCompleted on gameCompleted.StudentId = student.StudentId
    where gameCompleted.IsDeleted = 0
        and gameCompleted.Updated >= @schoolYearStarts

    insert #completedAssessment (StudentId, Metric)
    select distinct
         assessmentAssignment.StudentId
        ,case
            when assessment.CategoryId = @scienceCategoryId and assessment.Name like N'Part 1:%' then N'ScienceVocabulary'
            when assessment.CategoryId = @scienceCategoryId and assessment.Name like N'Part 2:%Transfer Challenge%' then N'ScienceCc'
            when assessment.CategoryId = @socialStudiesCategoryId and assessment.Name like N'Part 1:%' then N'SocialStudiesVocabulary'
            when assessment.CategoryId = @socialStudiesCategoryId and assessment.Name like N'Part 2:%Transfer Challenge%' then N'SocialStudiesCc'
         end
    from (select distinct StudentId from #studentRoster) student
        inner join [Education].[AssessmentAssignment-Active] assessmentAssignment on assessmentAssignment.StudentId = student.StudentId
        inner join [Education].[Assessment-Active] assessment on assessment.Id = assessmentAssignment.AssessmentId
    where assessmentAssignment.Completed >= @schoolYearStarts
        and assessment.CategoryId in (@scienceCategoryId, @socialStudiesCategoryId)
        and (
            assessment.Name like N'Part 1:%'
            or assessment.Name like N'Part 2:%Transfer Challenge%'
        )

    insert #recordingCompletion (SchoolContactId, HasAllFour)
    select
         recording.UploadedBy
        ,convert(bit, case when count(distinct case
            when recording.CategoryId in (@elaCategoryId, @mathCategoryId, @scienceCategoryId, @socialStudiesCategoryId)
                then recording.CategoryId
         end) = 4 then 1 else 0 end)
    from (select distinct SchoolContactId from #teacherRoster) teacher
        inner join [Education].[ClassRecording-Active] recording on recording.UploadedBy = teacher.SchoolContactId
    where recording.AudioFileId is not null
        and recording.Uploaded >= @schoolYearStarts
    group by recording.UploadedBy

    insert #metric (Metric, SchoolId, GradeId, CompletedCount, TotalCount, Percentage)
    select N'TeacherLogin', grouped.SchoolId, grouped.GradeId, grouped.CompletedCount, grouped.TotalCount,
        cast(100.0 * grouped.CompletedCount / nullif(grouped.TotalCount, 0) as decimal(5, 1))
    from (
        select teacher.SchoolId, teacher.GradeId,
            count(distinct case when signedIn.UserId is not null then teacher.SchoolContactId end) as CompletedCount,
            count(distinct teacher.SchoolContactId) as TotalCount
        from #teacherRoster teacher
            left join #signedInUsers signedIn on signedIn.UserId = teacher.UserId
        group by grouping sets ((teacher.SchoolId, teacher.GradeId), (teacher.SchoolId))
    ) grouped

    insert #metric (Metric, SchoolId, GradeId, CompletedCount, TotalCount, Percentage)
    select N'AllFourAudioRecording', grouped.SchoolId, grouped.GradeId, grouped.CompletedCount, grouped.TotalCount,
        cast(100.0 * grouped.CompletedCount / nullif(grouped.TotalCount, 0) as decimal(5, 1))
    from (
        select teacher.SchoolId, teacher.GradeId,
            count(distinct case when recorded.SchoolContactId is not null then teacher.SchoolContactId end) as CompletedCount,
            count(distinct teacher.SchoolContactId) as TotalCount
        from #teacherRoster teacher
            left join #recordingCompletion recorded on recorded.SchoolContactId = teacher.SchoolContactId
                and recorded.HasAllFour = 1
        group by grouping sets ((teacher.SchoolId, teacher.GradeId), (teacher.SchoolId))
    ) grouped

    insert #metric (Metric, SchoolId, GradeId, CompletedCount, TotalCount, Percentage)
    select N'TeacherDigital101', grouped.SchoolId, grouped.GradeId, grouped.CompletedCount, grouped.TotalCount,
        cast(100.0 * grouped.CompletedCount / nullif(grouped.TotalCount, 0) as decimal(5, 1))
    from (
        select teacher.SchoolId, teacher.GradeId,
            count(distinct case when completed.ContactId is not null then teacher.SchoolContactId end) as CompletedCount,
            count(distinct teacher.SchoolContactId) as TotalCount
        from #teacherRoster teacher
            left join #completedDigital101 completed on completed.ContactId = teacher.ContactId
        group by grouping sets ((teacher.SchoolId, teacher.GradeId), (teacher.SchoolId))
    ) grouped

    insert #metric (Metric, SchoolId, GradeId, CompletedCount, TotalCount, Percentage)
    select N'AudioRecording', grouped.SchoolId, grouped.GradeId, grouped.CompletedCount, grouped.TotalCount,
        cast(100.0 * grouped.CompletedCount / nullif(grouped.TotalCount, 0) as decimal(5, 1))
    from (
        select teacher.SchoolId, teacher.GradeId,
            count(distinct case when recorded.SchoolContactId is not null then teacher.SchoolContactId end) as CompletedCount,
            count(distinct teacher.SchoolContactId) as TotalCount
        from #teacherRoster teacher
            left join #recordingCompletion recorded on recorded.SchoolContactId = teacher.SchoolContactId
        group by grouping sets ((teacher.SchoolId, teacher.GradeId), (teacher.SchoolId))
    ) grouped

    insert #metric (Metric, SchoolId, GradeId, CompletedCount, TotalCount, Percentage)
    select N'StudentLogin', grouped.SchoolId, grouped.GradeId, grouped.CompletedCount, grouped.TotalCount,
        cast(100.0 * grouped.CompletedCount / nullif(grouped.TotalCount, 0) as decimal(5, 1))
    from (
        select student.SchoolId, student.GradeId,
            count(distinct case when signedIn.UserId is not null then student.StudentId end) as CompletedCount,
            count(distinct student.StudentId) as TotalCount
        from #studentRoster student
            left join #signedInUsers signedIn on signedIn.UserId = student.UserId
        group by grouping sets ((student.SchoolId, student.GradeId), (student.SchoolId))
    ) grouped

    insert #metric (Metric, SchoolId, GradeId, CompletedCount, TotalCount, Percentage)
    select N'StudentGame', grouped.SchoolId, grouped.GradeId, grouped.CompletedCount, grouped.TotalCount,
        cast(100.0 * grouped.CompletedCount / nullif(grouped.TotalCount, 0) as decimal(5, 1))
    from (
        select student.SchoolId, student.GradeId,
            count(distinct case when completed.StudentId is not null then student.StudentId end) as CompletedCount,
            count(distinct student.StudentId) as TotalCount
        from #studentRoster student
            left join #completedGame completed on completed.StudentId = student.StudentId
        group by grouping sets ((student.SchoolId, student.GradeId), (student.SchoolId))
    ) grouped

    insert #metric (Metric, SchoolId, GradeId, CompletedCount, TotalCount, Percentage)
    select requested.Metric, grouped.SchoolId, grouped.GradeId, grouped.CompletedCount, grouped.TotalCount,
        cast(100.0 * grouped.CompletedCount / nullif(grouped.TotalCount, 0) as decimal(5, 1))
    from (values
         (N'ScienceVocabulary')
        ,(N'ScienceCc')
        ,(N'SocialStudiesVocabulary')
        ,(N'SocialStudiesCc')
    ) requested(Metric)
    cross apply (
        select student.SchoolId, student.GradeId,
            count(distinct case when completed.StudentId is not null then student.StudentId end) as CompletedCount,
            count(distinct student.StudentId) as TotalCount
        from #studentRoster student
            left join #completedAssessment completed on completed.StudentId = student.StudentId
                and completed.Metric = requested.Metric
        group by grouping sets ((student.SchoolId, student.GradeId), (student.SchoolId))
    ) grouped

    select Metric, SchoolId, GradeId, CompletedCount, TotalCount, Percentage
    from #metric
    order by Metric, SchoolId, GradeId
end