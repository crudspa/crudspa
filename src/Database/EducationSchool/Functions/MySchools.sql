create function [EducationSchool].[MySchools] (@SessionId uniqueidentifier)
returns @Schools table (
     [Id] uniqueidentifier
    ,[Name] nvarchar(75)
)
as
begin

    declare @contactId uniqueidentifier
    declare @userId uniqueidentifier

    select top 1
        @contactId = userTable.ContactId
        ,@userId = session.UserId
    from [Framework].[Session-Active] session
        inner join [Framework].[User-Active] userTable on session.UserId = userTable.Id
    where session.Id = @SessionId

    -- School contacts can only see their own school

    if exists (select Id from [Education].[SchoolContact-Active] where ContactId = @contactId) begin

        insert into @Schools
        select
            school.Id
            ,organization.Name as Name
        from [Education].[School-Active] as school
            inner join [Framework].[Organization-Active] organization on school.OrganizationId = organization.Id
            inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.SchoolId = school.Id
        where schoolContact.ContactId = @contactId

    end

    return
end