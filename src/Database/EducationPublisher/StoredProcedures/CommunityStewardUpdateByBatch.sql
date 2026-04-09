create proc [EducationPublisher].[CommunityStewardUpdateByBatch] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@CommunityId uniqueidentifier
    ,@DistrictContactId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Education].[CommunitySteward]
set
    Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,CommunityId = @CommunityId
    ,DistrictContactId = @DistrictContactId
where Id = @Id