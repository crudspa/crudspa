create proc [ContentDesign].[ItemUpsert] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@BasisId uniqueidentifier
    ,@BasisAmount nvarchar(10)
    ,@Grow nvarchar(10)
    ,@Shrink nvarchar(10)
    ,@AlignSelfId uniqueidentifier
    ,@MaxWidth nvarchar(10)
    ,@MinWidth nvarchar(10)
    ,@Width nvarchar(10)
) as

declare @now datetimeoffset = sysdatetimeoffset()

if not exists (select Id from [Content].[Item-Active] where Id = @Id)
begin
    insert [Content].[Item] (
        Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,BasisId
        ,BasisAmount
        ,Grow
        ,Shrink
        ,AlignSelfId
        ,MaxWidth
        ,MinWidth
        ,Width
    )
    values (
        @Id
        ,@Id
        ,@now
        ,@SessionId
        ,@BasisId
        ,@BasisAmount
        ,@Grow
        ,@Shrink
        ,@AlignSelfId
        ,@MaxWidth
        ,@MinWidth
        ,@Width
    )
end
else
begin
    update [Content].[Item]
    set
        Id = @Id
        ,Updated = @now
        ,UpdatedBy = @SessionId
        ,BasisId = @BasisId
        ,BasisAmount = @BasisAmount
        ,Grow = @Grow
        ,Shrink = @Shrink
        ,AlignSelfId = @AlignSelfId
        ,MaxWidth = @MaxWidth
        ,MinWidth = @MinWidth
        ,Width = @Width
    where Id = @Id
end