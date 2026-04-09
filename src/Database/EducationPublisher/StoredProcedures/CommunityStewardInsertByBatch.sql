create proc [EducationPublisher].[CommunityStewardInsertByBatch] (
     @SessionId uniqueidentifier
    ,@CommunityId uniqueidentifier
    ,@DistrictContactId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Education].[CommunitySteward] (
    Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,CommunityId
    ,DistrictContactId
)
values (
    @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@CommunityId
    ,@DistrictContactId
)