create proc [EducationDistrict].[DistrictContactInsert] (
     @SessionId uniqueidentifier
    ,@Title nvarchar(50)
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

insert [Education].[DistrictContact] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,Title
    ,ContactId
    ,UserId
    ,DistrictId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@Title
    ,@ContactId
    ,@UserId
    ,@districtId
)

if not exists (
    select 1
    from [Education].[DistrictContact-Active] districtContact
        inner join [Education].[District-Active] district on districtContact.DistrictId = district.Id
    where districtContact.Id = @Id
        and district.Id = @districtId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction