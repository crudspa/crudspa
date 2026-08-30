create proc [EducationRostering].[RosterSourceDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@OrganizationId uniqueidentifier
) as

update [Education].[RosterSource]
set Updated = sysdatetimeoffset()
    ,UpdatedBy = @SessionId
    ,IsDeleted = 1
where Id = @Id
    and OrganizationId = @OrganizationId
    and VersionOf = Id
    and IsDeleted = 0

if @@rowcount = 0
    raiserror('Roster source not found', 16, 1)