create function [EducationDistrict].[MySchools] (@SessionId uniqueidentifier)
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

    -- District contacts can see all schools they are stewards for, or all schools in the district if they are an admin

    declare @districtId uniqueidentifier
    declare @districtContactId uniqueidentifier

    select top 1
        @districtId = DistrictId
        ,@districtContactId = Id
    from [Education].[DistrictContact-Active]
    where ContactId = @contactId

    -- Schools for which I am a steward
    insert into @Schools
    select
        school.Id
        ,organization.Name as Name
    from [Education].[School-Active] as school
        inner join [Framework].[Organization-Active] organization on school.OrganizationId = organization.Id
        inner join [Education].[Community-Active] community on school.CommunityId = community.Id
        inner join [Education].[CommunitySteward-Active] communitySteward on communitySteward.CommunityId = community.Id
    where school.DistrictId = @districtId
        and communitySteward.DistrictContactId = @districtContactId

    if exists(
        select userRole.Id
        from [Framework].[UserRole-Active] userRole
            inner join [Framework].[Role] roleTable on userRole.RoleId = roleTable.Id
        where userRole.UserId = @userId
            and (roleTable.Name = 'Admin' or roleTable.Name = 'Executive Director')
    ) begin

        -- All schools in my district
        insert into @Schools
        select
            school.Id
            ,organization.Name as Name
        from [Education].[School-Active] as school
            inner join [Framework].[Organization-Active] organization on school.OrganizationId = organization.Id
        where school.DistrictId = @districtId
            and school.Id not in (select Id from @Schools)
    end

    return
end