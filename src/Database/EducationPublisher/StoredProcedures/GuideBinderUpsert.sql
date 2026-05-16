create proc [EducationPublisher].[GuideBinderUpsert] (
     @SessionId uniqueidentifier
    ,@BinderId uniqueidentifier
    ,@GuideImageId uniqueidentifier
    ,@Id uniqueidentifier output
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

select @Id = Id
from [Education].[GuideBinder-Active]
where BinderId = @BinderId

if (@Id is null)
begin
    set @Id = newid()

    insert [Education].[GuideBinder] (
         Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,BinderId
        ,GuideImageId
    )
    values (
         @Id
        ,@Id
        ,@now
        ,@SessionId
        ,@BinderId
        ,@GuideImageId
    )
end
else
begin
    update [Education].[GuideBinder]
    set
         Id = @Id
        ,Updated = @now
        ,UpdatedBy = @SessionId
        ,GuideImageId = @GuideImageId
    where Id = @Id
end

commit transaction