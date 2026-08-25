create proc [EducationPublisher].[AssessmentSelectNames] (
     @SessionId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     assessment.Id
    ,assessment.Name as Name
from [Education].[Assessment-Active] assessment
where assessment.OwnerId = @organizationId
order by assessment.Name