create proc [EducationDistrict].[SchoolUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Key nvarchar(100)
    ,@CommunityId uniqueidentifier
    ,@Treatment bit
    ,@AddressId uniqueidentifier
) as

declare @districtId uniqueidentifier = (
    select top 1 district.Id
    from [Education].[District-Active] district
        inner join [Education].[DistrictContact-Active] districtContact on districtContact.DistrictId = district.Id
        inner join [Framework].[User-Active] userTable on districtContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,[Key] = @Key
    ,CommunityId = @CommunityId
    ,Treatment = @Treatment
    ,AddressId = @AddressId
from [Education].[School] baseTable
    inner join [Education].[School-Active] school on school.Id = baseTable.Id
    inner join [Education].[District-Active] district on school.DistrictId = district.Id
where baseTable.Id = @Id
    and district.Id = @districtId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction