create proc [EducationDistrict].[CommunityStewardInsertByBatch] (
     @SessionId uniqueidentifier
    ,@CommunityId uniqueidentifier
    ,@DistrictContactId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

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

commit transaction