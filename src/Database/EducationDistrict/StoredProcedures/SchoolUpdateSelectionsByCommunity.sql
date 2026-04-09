create proc [EducationDistrict].[SchoolUpdateSelectionsByCommunity] (
     @SessionId uniqueidentifier
    ,@CommunityId uniqueidentifier
    ,@Schools Framework.IdList readonly
) as


declare @now datetimeoffset = sysdatetimeoffset()
declare @districtId uniqueidentifier = (select top 1 DistrictId from [Education].[Community-Active] where Id = @CommunityId)

begin transaction

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