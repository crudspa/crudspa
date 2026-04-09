create proc [EducationSchool].[StudentInsert] (
     @SessionId uniqueidentifier
    ,@FirstName nvarchar(75)
    ,@LastName nvarchar(75)
    ,@SecretCode nvarchar(75)
    ,@GradeId uniqueidentifier
    ,@AssessmentLevelGroupId uniqueidentifier
    ,@PreferredName nvarchar(75)
    ,@AvatarString nvarchar(2)
    ,@IdNumber nvarchar(35)
    ,@IsTestAccount bit
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

declare @portalId uniqueidentifier = 'c882bec5-cca6-4327-8f37-7729b2839b80' -- School

declare @schoolId uniqueidentifier = (
    select top 1 school.Id
    from [Education].[School-Active] school
        inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.SchoolId = school.Id
        inner join [Framework].[User-Active] userTable on schoolContact.UserId = userTable.Id
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @organizationId uniqueidentifier
declare @timeZoneId nvarchar(32)

select @organizationId = organization.Id
    ,@timeZoneId = organization.TimeZoneId
from [Education].[School-Active] school
    inner join [Framework].[Organization-Active] organization on school.OrganizationId = organization.Id
where school.Id = @schoolId

begin transaction

    declare @contactId uniqueidentifier = newid()

    insert [Framework].[Contact] (
        Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,FirstName
        ,LastName
        ,TimeZoneId
    )
    values (
        @contactId
        ,@contactId
        ,@now
        ,@SessionId
        ,@FirstName
        ,@LastName
        ,@timeZoneId
    )

    declare @userId uniqueidentifier = newid()

    insert [Framework].[User] (
        Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,ContactId
        ,PortalId
        ,OrganizationId
        ,Username
        ,ResetPassword
    )
    values (
        @userId
        ,@userId
        ,@now
        ,@SessionId
        ,@contactId
        ,@portalId
        ,@organizationId
        ,'StudentUser-' + lower(convert(nvarchar(36), @Id))
        ,0
    )

    declare @familyOrganizationId uniqueidentifier = newid()

    insert [Framework].[Organization] (
        Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,Name
        ,TimeZoneId
    )
    values (
        @familyOrganizationId
        ,@familyOrganizationId
        ,@now
        ,@SessionId
        ,@LastName + ' (family)'
        ,@timeZoneId
    )

    declare @familyId uniqueidentifier = newid()

    insert [Education].[Family] (
        Id
        ,UpdatedBy
        ,VersionOf
        ,OrganizationId
        ,SchoolId
    )
    values (
        @familyId
        ,@SessionId
        ,@familyId
        ,@familyOrganizationId
        ,@schoolId
    )

    insert [Education].[Student] (
        Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,ContactId
        ,UserId
        ,FamilyId
        ,SecretCode
        ,GradeId
        ,AssessmentLevelGroupId
        ,PreferredName
        ,AvatarString
        ,IdNumber
        ,CreatedBySchool
        ,IsTestAccount
        --defaults
        ,AssessmentTypeGroupId
        ,AudioGenderId
        ,ChallengeLevelId
        ,ConditionGroupId
        ,ContentGroupId
        ,GenderId
        ,GoalSettingGroupId
        ,MoreSample
        ,PersonalizationGroupId
        ,StatusId
        ,TextSample
    )
    values (
        @Id
        ,@Id
        ,@now
        ,@SessionId
        ,@contactId
        ,@userId
        ,@familyId
        ,@SecretCode
        ,@GradeId
        ,@AssessmentLevelGroupId
        ,@PreferredName
        ,@AvatarString
        ,@IdNumber
        ,1
        ,@IsTestAccount
        --defaults
        ,'945d1ca4-e458-4fc9-9656-5b31fb23459d'
        ,'5c9bce2e-fc32-4863-a143-00d10254f218'
        ,'e2fa4f47-3587-4ff3-8dcf-008e7d1431be'
        ,'3befe15f-f053-4f26-826f-583dead12346'
        ,'2f742d69-63f7-4a05-bd13-15bc528ea55e'
        ,'629f74b9-4681-4b78-a657-82178611a030'
        ,'7a20cf56-bbfe-408c-8208-f1636a7851cf'
        ,1
        ,'49c37bc2-15e3-4afc-bc7f-d0846d460807'
        ,'9b3a7c55-130f-4e71-9538-1925f2962a4c'
        ,1
    )

    declare @schoolYearId uniqueidentifier = (select top 1 Id from [Education].[SchoolYear] where Starts <= @now and Ends > @now order by Starts desc)

    if not exists (select 1 from [Education].[StudentSchoolYear-Active] where StudentId = @Id and SchoolYearId = @schoolYearId)
    begin

        declare @studentSchoolYearId uniqueidentifier = newid()

        insert [Education].[StudentSchoolYear] (
            Id
            ,VersionOf
            ,Updated
            ,UpdatedBy
            ,StudentId
            ,SchoolYearId
        )
        values (
            @studentSchoolYearId
            ,@studentSchoolYearId
            ,@now
            ,@SessionId
            ,@Id
            ,@schoolYearId
        )

    end

commit transaction