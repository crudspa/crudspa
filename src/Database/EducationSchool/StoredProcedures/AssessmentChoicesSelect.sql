create proc [EducationSchool].[AssessmentChoicesSelect] (
     @ClassroomId uniqueidentifier,
    @AssessmentType nvarchar(25) = 'science'
) as

declare @schoolYearBeginDate datetime = (select top 1 Starts from [Education].[SchoolYear-Active] where getdate() between Starts and Ends)
declare @socialStudiesCategory uniqueidentifier = 'AC5C30DD-AB8A-4921-A857-15701E8FFEF0'
declare @scienceCategory uniqueidentifier = '69A5F32D-4D5F-4683-93B3-234ED12758C4'

declare @AssessmentIds table (
    AssessmentId uniqueidentifier,
    Optional bit
)

-- Get the Assessments that relate to the passed in type. This is hardcoded for now
if @AssessmentType = 'socialstudies'
begin
    insert into @AssessmentIds
    select Id, 0 from [Education].[Assessment-Active] where CategoryId = @socialStudiesCategory
end
else
begin
    insert into @AssessmentIds
    select Id, 0 from [Education].[Assessment-Active] where CategoryId = @scienceCategory
end

;with ReadCTE as (
    -- Combine Read choice and Read Text Entry answers
    select  student.Id as StudentId,
            student.IdNumber,
            assignment.AssessmentId as AssessmentId,
            assessments.Optional,
            part.Title as PartTitle,
            question.Text as QuestionText,
            part.Ordinal as PartOrdinal,
            convert(datetime2, selection.Made, 1) as ChoiceMade,
            selection.AssignmentId,
            choice.ReadQuestionId as QuestionId,
            'Read' as ChoiceType,
            choice.Text as ChoiceText,
            choice.IsCorrect,
            choice.Ordinal as ChoiceOrdinal,
            cast(question.HasCorrectChoice as nvarchar(3)) as HasCorrectChoice,
            cast(question.RequireTextInput as nvarchar(3)) as RequireTextInput,
            question.Ordinal as QuestionOrdinal,
            null as TextEntry,
            readQuestionType.Name as QuestionType,
            questionCategory.Name as QuestionCategory,
            row_number() over (partition by student.IdNumber, assignment.AssessmentId, part.Id, question.Id
                               order by selection.Made) as RowNum
    from [Education].[ReadChoiceSelection-Active] selection
        inner join [Education].[ReadChoice-Active] choice on choice.Id = selection.ChoiceId
        inner join [Education].[ReadQuestion-Active] question on question.Id = choice.ReadQuestionId
        inner join [Education].[ReadPart-Active] part on part.Id = question.ReadPartId
        inner join [Education].[AssessmentAssignment-Active] assignment on assignment.Id = selection.AssignmentId
        inner join [Education].[Student-Active] student on student.Id = assignment.StudentId
        inner join [Education].[ClassroomStudent-Active] classroomStudent on student.Id = classroomStudent.StudentId and classroomStudent.ClassroomId = @ClassroomId
        inner join [Education].[ReadQuestionType-Active] readQuestionType on question.TypeId = readQuestionType.Id
        inner join [Education].[ReadQuestionCategory-Active] questionCategory on question.CategoryId = questionCategory.Id
        inner join @AssessmentIds assessments on assessments.AssessmentId = assignment.AssessmentId
    where assignment.Started > @schoolYearBeginDate
        and question.IsPreview = 0

    union all

    select  student.Id as StudentId,
            student.IdNumber,
            assignment.AssessmentId as AssessmentId,
            assessments.Optional,
            part.Title as PartTitle,
            question.Text as QuestionText,
            part.Ordinal as PartOrdinal,
            convert(datetime2, textEntry.Updated, 1) as ChoiceMade,
            textEntry.AssignmentId,
            question.Id as QuestionId,
            'Read' as ChoiceType,
            null as ChoiceText,
            null as IsCorrect,
            null as ChoiceOrdinal,
            cast(question.HasCorrectChoice as nvarchar(3)) as HasCorrectChoice,
            cast(question.RequireTextInput as nvarchar(3)) as RequireTextInput,
            question.Ordinal as QuestionOrdinal,
            textEntry.Text as TextEntry,
            readQuestionType.Name as QuestionType,
            questionCategory.Name as QuestionCategory,
            row_number() over (partition by student.IdNumber, assignment.AssessmentId, part.Id, question.Id
                               order by textEntry.Updated) as RowNum
    from [Education].[ReadTextEntry] textEntry
        inner join [Education].[ReadQuestion-Active] question on question.Id = textEntry.QuestionId and question.RequireTextInput = 1
        inner join [Education].[ReadPart-Active] part on part.Id = question.ReadPartId
        inner join [Education].[AssessmentAssignment-Active] assignment on assignment.Id = textEntry.AssignmentId
        inner join [Education].[Student-Active] student on student.Id = assignment.StudentId
        inner join [Education].[ClassroomStudent-Active] classroomStudent on student.Id = classroomStudent.StudentId and classroomStudent.ClassroomId = @ClassroomId
        inner join [Education].[ReadQuestionType-Active] readQuestionType on question.TypeId = readQuestionType.Id
        inner join [Education].[ReadQuestionCategory-Active] questionCategory on question.CategoryId = questionCategory.Id
        inner join @AssessmentIds assessments on assessments.AssessmentId = assignment.AssessmentId
    where assignment.Started > @schoolYearBeginDate
        and question.IsPreview = 0
    ),

ListenCTE as (
    -- Listen choice answers
    select  student.Id as StudentId,
            student.IdNumber,
            assignment.AssessmentId as AssessmentId,
            assessments.Optional,
            part.Title as PartTitle,
            question.Text as QuestionText,
            part.Ordinal as PartOrdinal,
            convert(datetime2, selection.Made, 1) as ChoiceMade,
            selection.AssignmentId,
            choice.ListenQuestionId as QuestionId,
            'Listen' as ChoiceType,
            choice.Text as ChoiceText,
            choice.IsCorrect,
            choice.Ordinal as ChoiceOrdinal,
            cast(question.HasCorrectChoice as nvarchar(3)) as HasCorrectChoice,
            cast(question.RequireTextInput as nvarchar(3)) as RequireTextInput,
            question.Ordinal as QuestionOrdinal,
            null as TextEntry,
            null as QuestionType,
            questionCategory.Name as QuestionCategory,
            row_number() over (partition by student.IdNumber, assignment.AssessmentId, part.Id, question.Id
                               order by selection.Made) as RowNum
    from [Education].[ListenChoiceSelection-Active] selection
        inner join [Education].[ListenChoice-Active] choice on choice.Id = selection.ChoiceId
        inner join [Education].[ListenQuestion-Active] question on question.Id = choice.ListenQuestionId and question.RequireTextInput = 0
        inner join [Education].[ListenPart-Active] part on part.Id = question.ListenPartId
        inner join [Education].[AssessmentAssignment-Active] assignment on assignment.Id = selection.AssignmentId
        inner join [Education].[Student-Active] student on student.Id = assignment.StudentId
        inner join [Education].[ClassroomStudent-Active] classroomStudent on student.Id = classroomStudent.StudentId and classroomStudent.ClassroomId = @ClassroomId
        inner join [Education].[ReadQuestionCategory-Active] questionCategory on question.CategoryId = questionCategory.Id
        inner join @AssessmentIds assessments on assessments.AssessmentId = assignment.AssessmentId
    where assignment.Started > @schoolYearBeginDate
        and question.IsPreview = 0

    union all

    -- Listen Text Entry answers
    select  student.Id as StudentId,
            student.IdNumber,
            assignment.AssessmentId as AssessmentId,
            assessments.Optional,
            part.Title as PartTitle,
            question.Text as QuestionText,
            part.Ordinal as PartOrdinal,
            convert(datetime2, textEntry.Updated, 1) as ChoiceMade,
            textEntry.AssignmentId,
            question.Id as QuestionId,
            'Listen' as ChoiceType,
            null as ChoiceText,
            null as IsCorrect,
            null as ChoiceOrdinal,
            cast(question.HasCorrectChoice as nvarchar(3)) as HasCorrectChoice,
            cast(question.RequireTextInput as nvarchar(3)) as RequireTextInput,
            question.Ordinal as QuestionOrdinal,
            textEntry.Text as TextEntry,
            null as QuestionType,
            questionCategory.Name as QuestionCategory,
            row_number() over (partition by student.IdNumber, assignment.AssessmentId, part.Id, question.Id
                               order by textEntry.Updated) as RowNum
    from [Education].[ListenTextEntry] textEntry
        inner join [Education].[ListenQuestion-Active] question on question.Id = textEntry.QuestionId and question.RequireTextInput = 1
        inner join [Education].[ListenPart-Active] part on part.Id = question.ListenPartId
        inner join [Education].[AssessmentAssignment-Active] assignment on assignment.Id = textEntry.AssignmentId
        inner join [Education].[Student-Active] student on student.Id = assignment.StudentId
        inner join [Education].[ClassroomStudent-Active] classroomStudent on student.Id = classroomStudent.StudentId and classroomStudent.ClassroomId = @ClassroomId
        inner join [Education].[ReadQuestionCategory-Active] questionCategory on question.CategoryId = questionCategory.Id
        inner join @AssessmentIds assessments on assessments.AssessmentId = assignment.AssessmentId
    where assignment.Started > @schoolYearBeginDate
        and question.IsPreview = 0
),

VocabCTE as (
    -- Vocab choice answers
    select  student.Id as StudentId,
            student.IdNumber,
            assignment.AssessmentId as AssessmentId,
            assessments.Optional,
            part.Title as PartTitle,
            question.Word as QuestionText,
            part.Ordinal as PartOrdinal,
            convert(datetime2, selection.Made, 1) as ChoiceMade,
            selection.AssignmentId,
            choice.VocabQuestionId as QuestionId,
            'Vocab' as ChoiceType,
            choice.Word as ChoiceText,
            choice.IsCorrect,
            choice.Ordinal as ChoiceOrdinal,
            'N/A' as HasCorrectChoice,
            'N/A' as RequireTextInput,
            question.Ordinal as QuestionOrdinal,
            null as TextEntry,
            null as QuestionType,
            null as QuestionCategory,
            row_number() over (partition by student.IdNumber, assignment.AssessmentId, part.Id, question.Id
                               order by selection.Made) as RowNum

    from [Education].[VocabChoiceSelection-Active] selection
        inner join [Education].[VocabChoice-Active] choice on choice.Id = selection.ChoiceId
        inner join [Education].[VocabQuestion-Active] question on question.Id = choice.VocabQuestionId
        inner join [Education].[VocabPart-Active] part on part.Id = question.VocabPartId
        inner join [Education].[AssessmentAssignment-Active] assignment on assignment.Id = selection.AssignmentId
        inner join [Education].[Student-Active] student on student.Id = assignment.StudentId
        inner join [Education].[ClassroomStudent-Active] classroomStudent on student.Id = classroomStudent.StudentId and classroomStudent.ClassroomId = @ClassroomId
        inner join @AssessmentIds assessments on assessments.AssessmentId = assignment.AssessmentId
    where assignment.Started > @schoolYearBeginDate
        and question.IsPreview = 0
)

-- Select only the first record for Read and Listen, top two for Vocab
select *
from ReadCTE
where RowNum = 1

union all

select *
from ListenCTE
where RowNum = 1

union all

select *
from VocabCTE
where RowNum in (1, 2)