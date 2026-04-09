create proc [EducationPublisher].[DistrictContactSelectRolesByIds] (
     @DistrictContactIds Framework.IdList readonly
) as

select userRole.Id
    ,role.Name
    ,districtContact.Id as ParentId
from [Education].[DistrictContact-Active] districtContact
    inner join [Framework].[User-Active] userTable on districtContact.UserId = userTable.Id
    inner join [Framework].[UserRole-Active] userRole on userRole.UserId = userTable.Id
    inner join [Framework].[Role-Active] role on userRole.RoleId = role.Id
where districtContact.Id in (select Id from @DistrictContactIds)