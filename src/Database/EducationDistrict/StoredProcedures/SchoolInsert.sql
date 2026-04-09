create proc [EducationDistrict].[SchoolInsert] (
     @SessionId uniqueidentifier
    ,@Key nvarchar(100)
    ,@CommunityId uniqueidentifier
    ,@Treatment bit
    ,@OrganizationId uniqueidentifier
    ,@AddressId uniqueidentifier
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

insert [Education].[School] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,[Key]
    ,CommunityId
    ,Treatment
    ,OrganizationId
    ,AddressId
    ,DistrictId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@Key
    ,@CommunityId
    ,@Treatment
    ,@OrganizationId
    ,@AddressId
    ,@districtId
)

if not exists (
    select 1
    from [Education].[School-Active] school
        inner join [Education].[District-Active] district on school.DistrictId = district.Id
    where school.Id = @Id
        and district.Id = @districtId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction