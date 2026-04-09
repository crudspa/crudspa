create proc [EducationSchool].[AssessmentAssignmentSelectWhere] (
     @SessionId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@Classrooms Framework.IdList readonly
    ,@Assessments Framework.IdList readonly
) as

set nocount on

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @offsetRows int = @firstRecord - 1

declare @classroomsCount int = (select count(1) from @Classrooms)
declare @assessmentsCount int = (select count(1) from @Assessments)
declare @mySchools Framework.IdList

declare @now datetimeoffset = sysdatetimeoffset()
declare @schoolYearId uniqueidentifier = (select top 1 Id from [Education].[SchoolYear] where Starts <= @now and Ends > @now order by Starts desc)

insert into @mySchools (Id)
select mySchools.Id
from [EducationSchool].[MySchools](@SessionId) mySchools

create table #EligibleStudents (
    StudentId uniqueidentifier not null primary key clustered
    ,ContactId uniqueidentifier not null
)

insert into #EligibleStudents (StudentId, ContactId)
select distinct
    student.Id
    ,student.ContactId
from [Education].[Student-Active] student
    inner join [Education].[Family-Active] family on student.FamilyId = family.Id
    inner join [Education].[StudentSchoolYear-Active] studentSchoolYear on studentSchoolYear.StudentId = student.Id
where studentSchoolYear.SchoolYearId = @schoolYearId
    and student.DeletedBySchool = 0
    and exists (select 1 from @mySchools mySchools where mySchools.Id = family.SchoolId)
    and (@classroomsCount = 0 or exists (
        select 1
        from [Education].[ClassroomStudent-Active] classroomStudent
            inner join [Education].[Classroom-Active] classroom on classroomStudent.ClassroomId = classroom.Id
        where classroomStudent.StudentId = student.Id
            and exists (select 1 from @Classrooms classrooms where classrooms.Id = classroom.Id)
    ))

create table #FilteredAssignments (
    Id uniqueidentifier not null primary key clustered
    ,AssessmentId uniqueidentifier not null
    ,StudentId uniqueidentifier not null
    ,StartAfter datetimeoffset not null
    ,EndBefore datetimeoffset not null
    ,Started datetimeoffset null
    ,Completed datetimeoffset null
    ,AssessmentName nvarchar(100) not null
    ,StudentFirstName nvarchar(75) null
    ,StudentLastName nvarchar(75) null
)

insert into #FilteredAssignments (
    Id
    ,AssessmentId
    ,StudentId
    ,StartAfter
    ,EndBefore
    ,Started
    ,Completed
    ,AssessmentName
    ,StudentFirstName
    ,StudentLastName
)
select
    assessmentAssignment.Id
    ,assessmentAssignment.AssessmentId
    ,assessmentAssignment.StudentId
    ,assessmentAssignment.StartAfter
    ,assessmentAssignment.EndBefore
    ,assessmentAssignment.Started
    ,assessmentAssignment.Completed
    ,assessment.Name
    ,contact.FirstName
    ,contact.LastName
from [Education].[AssessmentAssignment-Active] assessmentAssignment
    inner join #EligibleStudents eligibleStudents on eligibleStudents.StudentId = assessmentAssignment.StudentId
    inner join [Education].[Assessment-Active] assessment on assessmentAssignment.AssessmentId = assessment.Id
    inner join [Framework].[Contact-Active] contact on contact.Id = eligibleStudents.ContactId
where assessment.AvailableStart <= @now
    and assessment.AvailableEnd >= @now
    and (@assessmentsCount = 0 or exists (select 1 from @Assessments assessments where assessments.Id = assessment.Id))
    and (
        @SearchText is null
        or assessment.Name like '%' + @SearchText + '%'
        or contact.FirstName like '%' + @SearchText + '%'
        or contact.LastName like '%' + @SearchText + '%'
    )

declare @totalCount int = (select count(1) from #FilteredAssignments)

if (@SortField = 'End') begin
    create index IX_FilteredAssignments_Sort on #FilteredAssignments (EndBefore)
end else if (@SortField = 'First Name') begin
    create index IX_FilteredAssignments_Sort on #FilteredAssignments (StudentFirstName)
end else if (@SortField = 'Last Name') begin
    create index IX_FilteredAssignments_Sort on #FilteredAssignments (StudentLastName)
end else begin
    create index IX_FilteredAssignments_Sort on #FilteredAssignments (StartAfter)
end

create table #PagedAssignments (
    RowNumber int not null
    ,TotalCount int not null
    ,Id uniqueidentifier not null primary key clustered
    ,AssessmentId uniqueidentifier not null
    ,StudentId uniqueidentifier not null
    ,StartAfter datetimeoffset not null
    ,EndBefore datetimeoffset not null
    ,Started datetimeoffset null
    ,Completed datetimeoffset null
    ,AssessmentName nvarchar(100) not null
    ,StudentFirstName nvarchar(75) null
    ,StudentLastName nvarchar(75) null
)

if (@SortField = 'End') begin
    if (@SortAscending = 1) begin
        insert into #PagedAssignments
        select
            row_number() over(order by filtered.EndBefore asc) + @offsetRows as RowNumber
            ,@totalCount as TotalCount
            ,filtered.Id
            ,filtered.AssessmentId
            ,filtered.StudentId
            ,filtered.StartAfter
            ,filtered.EndBefore
            ,filtered.Started
            ,filtered.Completed
            ,filtered.AssessmentName
            ,filtered.StudentFirstName
            ,filtered.StudentLastName
        from #FilteredAssignments filtered
        order by filtered.EndBefore asc
        offset @offsetRows rows fetch next @PageSize rows only
    end else begin
        insert into #PagedAssignments
        select
            row_number() over(order by filtered.EndBefore desc) + @offsetRows as RowNumber
            ,@totalCount as TotalCount
            ,filtered.Id
            ,filtered.AssessmentId
            ,filtered.StudentId
            ,filtered.StartAfter
            ,filtered.EndBefore
            ,filtered.Started
            ,filtered.Completed
            ,filtered.AssessmentName
            ,filtered.StudentFirstName
            ,filtered.StudentLastName
        from #FilteredAssignments filtered
        order by filtered.EndBefore desc
        offset @offsetRows rows fetch next @PageSize rows only
    end
end else if (@SortField = 'First Name') begin
    if (@SortAscending = 1) begin
        insert into #PagedAssignments
        select
            row_number() over(order by filtered.StudentFirstName asc) + @offsetRows as RowNumber
            ,@totalCount as TotalCount
            ,filtered.Id
            ,filtered.AssessmentId
            ,filtered.StudentId
            ,filtered.StartAfter
            ,filtered.EndBefore
            ,filtered.Started
            ,filtered.Completed
            ,filtered.AssessmentName
            ,filtered.StudentFirstName
            ,filtered.StudentLastName
        from #FilteredAssignments filtered
        order by filtered.StudentFirstName asc
        offset @offsetRows rows fetch next @PageSize rows only
    end else begin
        insert into #PagedAssignments
        select
            row_number() over(order by filtered.StudentFirstName desc) + @offsetRows as RowNumber
            ,@totalCount as TotalCount
            ,filtered.Id
            ,filtered.AssessmentId
            ,filtered.StudentId
            ,filtered.StartAfter
            ,filtered.EndBefore
            ,filtered.Started
            ,filtered.Completed
            ,filtered.AssessmentName
            ,filtered.StudentFirstName
            ,filtered.StudentLastName
        from #FilteredAssignments filtered
        order by filtered.StudentFirstName desc
        offset @offsetRows rows fetch next @PageSize rows only
    end
end else if (@SortField = 'Last Name') begin
    if (@SortAscending = 1) begin
        insert into #PagedAssignments
        select
            row_number() over(order by filtered.StudentLastName asc) + @offsetRows as RowNumber
            ,@totalCount as TotalCount
            ,filtered.Id
            ,filtered.AssessmentId
            ,filtered.StudentId
            ,filtered.StartAfter
            ,filtered.EndBefore
            ,filtered.Started
            ,filtered.Completed
            ,filtered.AssessmentName
            ,filtered.StudentFirstName
            ,filtered.StudentLastName
        from #FilteredAssignments filtered
        order by filtered.StudentLastName asc
        offset @offsetRows rows fetch next @PageSize rows only
    end else begin
        insert into #PagedAssignments
        select
            row_number() over(order by filtered.StudentLastName desc) + @offsetRows as RowNumber
            ,@totalCount as TotalCount
            ,filtered.Id
            ,filtered.AssessmentId
            ,filtered.StudentId
            ,filtered.StartAfter
            ,filtered.EndBefore
            ,filtered.Started
            ,filtered.Completed
            ,filtered.AssessmentName
            ,filtered.StudentFirstName
            ,filtered.StudentLastName
        from #FilteredAssignments filtered
        order by filtered.StudentLastName desc
        offset @offsetRows rows fetch next @PageSize rows only
    end
end else begin
    if (@SortAscending = 1) begin
        insert into #PagedAssignments
        select
            row_number() over(order by filtered.StartAfter asc) + @offsetRows as RowNumber
            ,@totalCount as TotalCount
            ,filtered.Id
            ,filtered.AssessmentId
            ,filtered.StudentId
            ,filtered.StartAfter
            ,filtered.EndBefore
            ,filtered.Started
            ,filtered.Completed
            ,filtered.AssessmentName
            ,filtered.StudentFirstName
            ,filtered.StudentLastName
        from #FilteredAssignments filtered
        order by filtered.StartAfter asc
        offset @offsetRows rows fetch next @PageSize rows only
    end else begin
        insert into #PagedAssignments
        select
            row_number() over(order by filtered.StartAfter desc) + @offsetRows as RowNumber
            ,@totalCount as TotalCount
            ,filtered.Id
            ,filtered.AssessmentId
            ,filtered.StudentId
            ,filtered.StartAfter
            ,filtered.EndBefore
            ,filtered.Started
            ,filtered.Completed
            ,filtered.AssessmentName
            ,filtered.StudentFirstName
            ,filtered.StudentLastName
        from #FilteredAssignments filtered
        order by filtered.StartAfter desc
        offset @offsetRows rows fetch next @PageSize rows only
    end
end

;with AssessmentIdsCte as (
    select distinct
        paged.AssessmentId
    from #PagedAssignments paged
)
,ListenPartCountCte as (
    select
        listenPart.AssessmentId
        ,count(1) as ListenPartCount
    from [Education].[ListenPart-Active] listenPart
        inner join AssessmentIdsCte assessmentIds on listenPart.AssessmentId = assessmentIds.AssessmentId
    group by listenPart.AssessmentId
)
,ListenQuestionCountCte as (
    select
        listenPart.AssessmentId
        ,count(1) as ListenQuestionCount
    from [Education].[ListenPart-Active] listenPart
        inner join AssessmentIdsCte assessmentIds on listenPart.AssessmentId = assessmentIds.AssessmentId
        inner join [Education].[ListenQuestion-Active] listenQuestion on listenQuestion.ListenPartId = listenPart.Id
    group by listenPart.AssessmentId
)
,ListenChoiceSelectionCountCte as (
    select
        listenChoiceSelection.AssignmentId
        ,count(distinct listenChoice.ListenQuestionId) as ListenChoiceSelectionCount
    from [Education].[ListenChoiceSelection-Active] listenChoiceSelection
        inner join #PagedAssignments paged on paged.Id = listenChoiceSelection.AssignmentId
        inner join [Education].[ListenChoice-Active] listenChoice on listenChoiceSelection.ChoiceId = listenChoice.Id
    group by listenChoiceSelection.AssignmentId
)
,ListenPartCompletedCountCte as (
    select
        listenPartCompleted.AssignmentId
        ,count(1) as ListenPartCompletedCount
    from [Education].[ListenPartCompleted-Active] listenPartCompleted
        inner join #PagedAssignments paged on paged.Id = listenPartCompleted.AssignmentId
    group by listenPartCompleted.AssignmentId
)
,ListenTextEntryCountCte as (
    select
        listenTextEntry.AssignmentId
        ,count(1) as ListenTextEntryCount
    from [Education].[ListenTextEntry-Active] listenTextEntry
        inner join #PagedAssignments paged on paged.Id = listenTextEntry.AssignmentId
    group by listenTextEntry.AssignmentId
)
,ReadPartCountCte as (
    select
        readPart.AssessmentId
        ,count(1) as ReadPartCount
    from [Education].[ReadPart-Active] readPart
        inner join AssessmentIdsCte assessmentIds on readPart.AssessmentId = assessmentIds.AssessmentId
    group by readPart.AssessmentId
)
,ReadQuestionCountCte as (
    select
        readPart.AssessmentId
        ,count(1) as ReadQuestionCount
    from [Education].[ReadPart-Active] readPart
        inner join AssessmentIdsCte assessmentIds on readPart.AssessmentId = assessmentIds.AssessmentId
        inner join [Education].[ReadQuestion-Active] readQuestion on readQuestion.ReadPartId = readPart.Id
    group by readPart.AssessmentId
)
,ReadChoiceSelectionCountCte as (
    select
        readChoiceSelection.AssignmentId
        ,count(distinct readChoice.ReadQuestionId) as ReadChoiceSelectionCount
    from [Education].[ReadChoiceSelection-Active] readChoiceSelection
        inner join #PagedAssignments paged on paged.Id = readChoiceSelection.AssignmentId
        inner join [Education].[ReadChoice-Active] readChoice on readChoiceSelection.ChoiceId = readChoice.Id
    group by readChoiceSelection.AssignmentId
)
,ReadPartCompletedCountCte as (
    select
        readPartCompleted.AssignmentId
        ,count(1) as ReadPartCompletedCount
    from [Education].[ReadPartCompleted-Active] readPartCompleted
        inner join #PagedAssignments paged on paged.Id = readPartCompleted.AssignmentId
    group by readPartCompleted.AssignmentId
)
,ReadTextEntryCountCte as (
    select
        readTextEntry.AssignmentId
        ,count(1) as ReadTextEntryCount
    from [Education].[ReadTextEntry-Active] readTextEntry
        inner join #PagedAssignments paged on paged.Id = readTextEntry.AssignmentId
    group by readTextEntry.AssignmentId
)
,VocabPartCountCte as (
    select
        vocabPart.AssessmentId
        ,count(1) as VocabPartCount
    from [Education].[VocabPart-Active] vocabPart
        inner join AssessmentIdsCte assessmentIds on vocabPart.AssessmentId = assessmentIds.AssessmentId
    group by vocabPart.AssessmentId
)
,VocabQuestionCountCte as (
    select
        vocabPart.AssessmentId
        ,count(1) as VocabQuestionCount
    from [Education].[VocabPart-Active] vocabPart
        inner join AssessmentIdsCte assessmentIds on vocabPart.AssessmentId = assessmentIds.AssessmentId
        inner join [Education].[VocabQuestion-Active] vocabQuestion on vocabQuestion.VocabPartId = vocabPart.Id
    group by vocabPart.AssessmentId
)
,VocabChoiceSelectionCountCte as (
    select
        vocabChoiceSelection.AssignmentId
        ,count(distinct vocabChoice.VocabQuestionId) as VocabChoiceSelectionCount
    from [Education].[VocabChoiceSelection-Active] vocabChoiceSelection
        inner join #PagedAssignments paged on paged.Id = vocabChoiceSelection.AssignmentId
        inner join [Education].[VocabChoice-Active] vocabChoice on vocabChoiceSelection.ChoiceId = vocabChoice.Id
    group by vocabChoiceSelection.AssignmentId
)
,VocabPartCompletedCountCte as (
    select
        vocabPartCompleted.AssignmentId
        ,count(1) as VocabPartCompletedCount
    from [Education].[VocabPartCompleted-Active] vocabPartCompleted
        inner join #PagedAssignments paged on paged.Id = vocabPartCompleted.AssignmentId
    group by vocabPartCompleted.AssignmentId
)

select
    paged.RowNumber
    ,paged.TotalCount
    ,paged.Id as Id
    ,paged.AssessmentId as AssessmentId
    ,paged.StudentId as StudentId
    ,paged.StartAfter as StartAfter
    ,paged.EndBefore as EndBefore
    ,paged.Started as Started
    ,paged.Completed as Completed
    ,paged.AssessmentName as AssessmentName
    ,paged.StudentFirstName as StudentFirstName
    ,paged.StudentLastName as StudentLastName
    ,isnull(listenPartCount.ListenPartCount, 0) as ListenPartCount
    ,isnull(listenQuestionCount.ListenQuestionCount, 0) as ListenQuestionCount
    ,isnull(listenChoiceSelectionCount.ListenChoiceSelectionCount, 0) as ListenChoiceSelectionCount
    ,isnull(listenPartCompletedCount.ListenPartCompletedCount, 0) as ListenPartCompletedCount
    ,isnull(listenTextEntryCount.ListenTextEntryCount, 0) as ListenTextEntryCount
    ,isnull(readPartCount.ReadPartCount, 0) as ReadPartCount
    ,isnull(readQuestionCount.ReadQuestionCount, 0) as ReadQuestionCount
    ,isnull(readChoiceSelectionCount.ReadChoiceSelectionCount, 0) as ReadChoiceSelectionCount
    ,isnull(readPartCompletedCount.ReadPartCompletedCount, 0) as ReadPartCompletedCount
    ,isnull(readTextEntryCount.ReadTextEntryCount, 0) as ReadTextEntryCount
    ,isnull(vocabPartCount.VocabPartCount, 0) as VocabPartCount
    ,isnull(vocabQuestionCount.VocabQuestionCount, 0) as VocabQuestionCount
    ,isnull(vocabChoiceSelectionCount.VocabChoiceSelectionCount, 0) as VocabChoiceSelectionCount
    ,isnull(vocabPartCompletedCount.VocabPartCompletedCount, 0) as VocabPartCompletedCount
from #PagedAssignments paged
    left join ListenPartCountCte listenPartCount on listenPartCount.AssessmentId = paged.AssessmentId
    left join ListenQuestionCountCte listenQuestionCount on listenQuestionCount.AssessmentId = paged.AssessmentId
    left join ListenChoiceSelectionCountCte listenChoiceSelectionCount on listenChoiceSelectionCount.AssignmentId = paged.Id
    left join ListenPartCompletedCountCte listenPartCompletedCount on listenPartCompletedCount.AssignmentId = paged.Id
    left join ListenTextEntryCountCte listenTextEntryCount on listenTextEntryCount.AssignmentId = paged.Id
    left join ReadPartCountCte readPartCount on readPartCount.AssessmentId = paged.AssessmentId
    left join ReadQuestionCountCte readQuestionCount on readQuestionCount.AssessmentId = paged.AssessmentId
    left join ReadChoiceSelectionCountCte readChoiceSelectionCount on readChoiceSelectionCount.AssignmentId = paged.Id
    left join ReadPartCompletedCountCte readPartCompletedCount on readPartCompletedCount.AssignmentId = paged.Id
    left join ReadTextEntryCountCte readTextEntryCount on readTextEntryCount.AssignmentId = paged.Id
    left join VocabPartCountCte vocabPartCount on vocabPartCount.AssessmentId = paged.AssessmentId
    left join VocabQuestionCountCte vocabQuestionCount on vocabQuestionCount.AssessmentId = paged.AssessmentId
    left join VocabChoiceSelectionCountCte vocabChoiceSelectionCount on vocabChoiceSelectionCount.AssignmentId = paged.Id
    left join VocabPartCompletedCountCte vocabPartCompletedCount on vocabPartCompletedCount.AssignmentId = paged.Id
order by paged.RowNumber asc