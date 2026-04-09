create proc [EducationPublisher].[SchoolUpdateSelectionsByCommunity] (
     @SessionId uniqueidentifier
    ,@CommunityId uniqueidentifier
    ,@Schools Framework.IdList readonly
) as

declare @publisherId uniqueidentifier = (
    select top 1 publisher.Id
    from [Education].[Publisher-Active] publisher
        inner join [Education].[PublisherContact-Active] publisherContact on publisherContact.PublisherId = publisher.Id
        inner join [Framework].[User-Active] userTable on publisherContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()
declare @districtId uniqueidentifier = (
    select top 1 community.DistrictId
    from [Education].[Community-Active] community
        inner join [Education].[District-Active] district on community.DistrictId = district.Id
    where community.Id = @CommunityId
        and district.PublisherId = @publisherId
)

begin transaction

    if @districtId is null
    begin
        rollback transaction
        raiserror('Tenancy check failed', 16, 1)
        return
    end

    update [Education].[School]
    set Updated = @now
        ,UpdatedBy = @SessionId
        ,CommunityId = @CommunityId
    where Id in (select Id from @Schools)
        and (CommunityId is null
            or CommunityId != @CommunityId)
        and IsDeleted = 0
        and VersionOf = Id
        and DistrictId = @districtId

    update [Education].[School]
    set Updated = @now
        ,UpdatedBy = @SessionId
        ,CommunityId = null
    where CommunityId = @CommunityId
        and Id not in (select Id from @Schools)
        and IsDeleted = 0
        and VersionOf = Id
        and DistrictId = @districtId

commit transaction