create proc [ContentDesign].[BinderInsert] (
     @SessionId uniqueidentifier
    ,@TypeId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Content].[Binder] (
    Id
    ,VersionOf
    ,UpdatedBy
    ,TypeId
)
values (
    @Id
    ,@Id
    ,@SessionId
    ,@TypeId
)