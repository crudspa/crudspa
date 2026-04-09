create proc [FrameworkCore].[ContactPhoneInsert] (
     @SessionId uniqueidentifier
    ,@ContactId uniqueidentifier
    ,@Phone nvarchar(10)
    ,@Extension nvarchar(10)
    ,@SupportsSms bit
    ,@Ordinal int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Framework].[ContactPhone] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ContactId
    ,Phone
    ,Extension
    ,SupportsSms
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ContactId
    ,@Phone
    ,@Extension
    ,@SupportsSms
    ,@Ordinal
)