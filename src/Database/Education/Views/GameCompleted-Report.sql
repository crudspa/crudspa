create view [Education].[GameCompleted-Report] as

select
     book.Title as BookTitle
    ,book.[Key] as BookKey
    ,convert(datetime2, completed.Updated, 1) as CompletedDate
    ,student.ResearchId as ResearchId
    ,student.Id as StudentId
    ,contact.FirstName as StudentFirstName
    ,contact.LastName as StudentLastName
    ,classroom.ClassroomId as ClassroomId
    ,classroom.SmallClassroom as SmallClassroom
    ,game.Id as GameId
    ,game.[Key] as GameKey
    ,organization.Name as DistrictName

from [Education].[GameCompleted] completed
    inner join [Education].[Game-Active] game on completed.GameId = game.Id
    inner join [Education].[Book-Active] book on book.Id = game.BookId
    inner join [Education].[Student-Active] student on student.Id = completed.StudentId
    inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
    inner join [Education].[Family-Active] family on student.FamilyId = family.Id
    inner join [Education].[School-Active] school on family.SchoolId = school.Id
    inner join [Education].[District-Active] district on school.DistrictId = district.Id
    inner join [Framework].[Organization-Active] organization on district.OrganizationId = organization.Id
    left join (select classroomStudent.ClassroomId, classroomStudent.StudentId, classroom.SmallClassroom
        from [Education].[ClassroomStudent-Active] classroomStudent
            inner join [Education].[Classroom-Active] classroom on classroomStudent.ClassroomId = classroom.Id
        where classroom.SchoolYearId = (select top 1 Id from [Education].[SchoolYear] where Starts <= sysdatetime() and Ends > sysdatetime() order by Starts desc)
    ) classroom on classroom.StudentId = student.Id
where completed.Updated > '2025-08-01'