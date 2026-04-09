create proc [EducationDistrict].[CommunityStewardUpdateByBatch] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@CommunityId uniqueidentifier
    ,@DistrictContactId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update [Education].[CommunitySteward]
set
    Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,DistrictContactId = @DistrictContactId
where Id = @Id

commit transaction