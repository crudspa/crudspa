create proc [EducationSchool].[StudentSelectWhere] (
     @SessionId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@Grades Framework.IdList readonly
    ,@AssessmentLevelGroups Framework.IdList readonly
    ,@SchoolYears Framework.IdList readonly
    ,@NotInClassroom bit
    ,@IncludeTestAccounts bit
) as

declare @now datetimeoffset = sysdatetimeoffset()

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1
declare @gradesCount int = (select count(1) from @Grades)
declare @assessmentLevelGroupsCount int = (select count(1) from @AssessmentLevelGroups)
declare @schoolYearsCount int = (select count(1) from @SchoolYears)

;with StudentCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'First Name' and @SortAscending = 1)
                    then contact.FirstName
                end asc,
                case when (@SortField = 'First Name' and @SortAscending = 0)
                    then contact.FirstName
                end desc,
                case when (@SortField = 'Last Name' and @SortAscending = 1)
                    then contact.LastName
                end asc,
                case when (@SortField = 'Last Name' and @SortAscending = 0)
                    then contact.LastName
                end desc,
                case when (@SortField = 'Preferred Name' and @SortAscending = 1)
                    then student.PreferredName
                end asc,
                case when (@SortField = 'Preferred Name' and @SortAscending = 0)
                    then student.PreferredName
                end desc,
                case when (@SortField = 'Id Number' and @SortAscending = 1)
                    then student.IdNumber
                end asc,
                case when (@SortField = 'Id Number' and @SortAscending = 0)
                    then student.IdNumber
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,student.Id
    from [Education].[Student-Active] student
        inner join [Education].[AssessmentLevel-Active] assessmentLevelGroup on student.AssessmentLevelGroupId = assessmentLevelGroup.Id
        inner join [Education].[Family-Active] family on student.FamilyId = family.Id
        inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
    where 1 = 1
        and (@SearchText is null
            or contact.FirstName like '%' + @SearchText + '%'
            or contact.LastName like '%' + @SearchText + '%'
            or student.SecretCode like '%' + @SearchText + '%'
            or student.PreferredName like '%' + @SearchText + '%'
            or student.IdNumber like '%' + @SearchText + '%'
        )
        and (@gradesCount = 0 or student.GradeId in (select Id from @Grades))
        and (@assessmentLevelGroupsCount = 0 or student.AssessmentLevelGroupId in (select Id from @AssessmentLevelGroups))
        and (@schoolYearsCount = 0
            or student.Id in (
                select StudentId
                from [Education].[StudentSchoolYear-Active]
                where SchoolYearId in (select Id from @SchoolYears)
            )
        )
        and family.SchoolId in (select Id from [EducationSchool].[MySchools] (@SessionId))
        and student.DeletedBySchool = 0
        and (@IncludeTestAccounts = 1 or student.IsTestAccount = 0)
        and (@NotInClassroom = 0
            or (
                select count(1)
                from [Education].[ClassroomStudent-Active] classroomStudent
                    inner join [Education].[Classroom-Active] classroom on classroomStudent.ClassroomId = classroom.Id
                where classroomStudent.StudentId = student.Id
                    and (@schoolYearsCount = 0 or classroom.SchoolYearId in (select Id from @SchoolYears))
                ) = 0)
        )


select
    cte.RowNumber
    ,cte.TotalCount
    ,student.Id
    ,contact.FirstName as FirstName
    ,contact.LastName as LastName
    ,student.SecretCode
    ,student.GradeId
    ,student.AssessmentLevelGroupId
    ,student.PreferredName
    ,student.AvatarString
    ,student.IdNumber
    ,student.IsTestAccount
    ,family.SchoolId as FamilySchoolId
    ,organization.Name as SchoolName
    ,grade.Name as GradeName
from StudentCte cte
    inner join [Education].[Student-Active] student on cte.Id = student.Id
    inner join [Education].[Grade-Active] grade on student.GradeId = grade.Id
    inner join [Education].[AssessmentLevel-Active] assessmentLevelGroup on student.AssessmentLevelGroupId = assessmentLevelGroup.Id
    inner join [Education].[Family-Active] family on student.FamilyId = family.Id
    inner join [Education].[School-Active] school on school.Id = family.SchoolId
    inner join [Framework].[Organization-Active] organization on school.OrganizationId = organization.Id
    inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc