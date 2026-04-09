create proc [ContentDesign].[ContainerUpsert] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@DirectionId uniqueidentifier
    ,@WrapId uniqueidentifier
    ,@JustifyContentId uniqueidentifier
    ,@AlignItemsId uniqueidentifier
    ,@AlignContentId uniqueidentifier
    ,@Gap nvarchar(10)
) as

declare @now datetimeoffset = sysdatetimeoffset()

if not exists (select Id from [Content].[Container-Active] where Id = @Id)
begin
    insert [Content].[Container] (
        Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,DirectionId
        ,WrapId
        ,JustifyContentId
        ,AlignItemsId
        ,AlignContentId
        ,Gap
    )
    values (
        @Id
        ,@Id
        ,@now
        ,@SessionId
        ,@DirectionId
        ,@WrapId
        ,@JustifyContentId
        ,@AlignItemsId
        ,@AlignContentId
        ,@Gap
    )
end
else
begin
    update [Content].[Container]
    set
        Id = @Id
        ,Updated = @now
        ,UpdatedBy = @SessionId
        ,DirectionId = @DirectionId
        ,WrapId = @WrapId
        ,JustifyContentId = @JustifyContentId
        ,AlignItemsId = @AlignItemsId
        ,AlignContentId = @AlignContentId
        ,Gap = @Gap
    where Id = @Id
end