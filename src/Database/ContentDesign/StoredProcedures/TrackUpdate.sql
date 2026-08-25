create proc [ContentDesign].[TrackUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Title nvarchar(75)
    ,@StatusId uniqueidentifier
    ,@Description nvarchar(max)
    ,@RequiresAchievementId uniqueidentifier
    ,@GeneratesAchievementId uniqueidentifier
    ,@RequireSequentialCompletion bit
    ,@Licenses Framework.IdList readonly
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
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
    ,Title = @Title
    ,StatusId = @StatusId
    ,Description = @Description
    ,RequiresAchievementId = @RequiresAchievementId
    ,GeneratesAchievementId = @GeneratesAchievementId
    ,RequireSequentialCompletion = @RequireSequentialCompletion
from [Content].[Track] baseTable
    inner join [Content].[Track-Active] track on track.Id = baseTable.Id
    inner join [Framework].[Portal-Active] portal on track.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where baseTable.Id = @Id
    and organization.Id = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end


update trackLicense
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[TrackLicense] trackLicense
    left join @Licenses ids on ids.Id = trackLicense.LicenseId
where trackLicense.TrackId = @Id
    and trackLicense.IsDeleted = 0
    and ids.Id is null

insert [Content].[TrackLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,TrackId
    ,LicenseId
)
select
     newRow.JunctionId
    ,newRow.JunctionId
    ,@now
    ,@SessionId
    ,@Id
    ,ids.Id
from (select distinct Id from @Licenses) ids
    inner join [Framework].[License-Active] license on license.Id = ids.Id
        and license.OwnerId = @organizationId
    left join [Content].[TrackLicense-Active] existingJunction on existingJunction.TrackId = @Id
        and existingJunction.LicenseId = ids.Id
    cross apply (select newid() as JunctionId) newRow
where existingJunction.Id is null
commit transaction