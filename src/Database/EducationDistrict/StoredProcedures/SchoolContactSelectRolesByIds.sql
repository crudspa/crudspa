create proc [EducationDistrict].[SchoolContactSelectRolesByIds] (
     @SchoolContactIds Framework.IdList readonly
) as

select userRole.Id
    ,role.Name
    ,schoolContact.Id as ParentId
from [Education].[SchoolContact-Active] schoolContact
    inner join [Framework].[User-Active] userTable on schoolContact.UserId = userTable.Id
    inner join [Framework].[UserRole-Active] userRole on userRole.UserId = userTable.Id
    inner join [Framework].[Role-Active] role on userRole.RoleId = role.Id
where schoolContact.Id in (select Id from @SchoolContactIds)