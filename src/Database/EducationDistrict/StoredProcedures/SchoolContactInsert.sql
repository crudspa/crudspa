create proc [EducationDistrict].[SchoolContactInsert] (
     @SessionId uniqueidentifier
    ,@SchoolId uniqueidentifier
    ,@TitleId uniqueidentifier
    ,@TestAccount bit
    ,@ContactId uniqueidentifier
    ,@UserId uniqueidentifier
    ,@Id uniqueidentifier output
) as

declare @districtId uniqueidentifier = (
    select top 1 district.Id
    from [Education].[District-Active] district
        inner join [Education].[DistrictContact-Active] districtContact on districtContact.DistrictId = district.Id
        inner join [Framework].[User-Active] userTable on districtContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Education].[SchoolContact] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,SchoolId
    ,TitleId
    ,TestAccount
    ,ContactId
    ,UserId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@SchoolId
    ,@TitleId
    ,@TestAccount
    ,@ContactId
    ,@UserId
)

if not exists (
    select 1
    from [Education].[SchoolContact-Active] schoolContact
        inner join [Education].[School-Active] school on schoolContact.SchoolId = school.Id
        inner join [Education].[District-Active] district on school.DistrictId = district.Id
    where schoolContact.Id = @Id
        and district.Id = @districtId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction