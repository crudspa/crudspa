create proc [FrameworkCore].[SegmentKeyIsUnique] (
     @Id uniqueidentifier
    ,@Key nvarchar(100)
    ,@PortalId uniqueidentifier
    ,@ParentId uniqueidentifier
    ,@Unique bit output
) as

if @Id is null begin
    set @Id = newid()
end

if exists (
    select 1
    from [Framework].[Segment-Active] segment
    where Id != @Id
        and [Key] = @Key
        and (
            (@ParentId is null and segment.ParentId is null and segment.PortalId = @PortalId)
            or (@ParentId is not null and segment.ParentId = @ParentId)
        )
)
begin
    set @Unique = 0
end
else
begin
    set @Unique = 1
end