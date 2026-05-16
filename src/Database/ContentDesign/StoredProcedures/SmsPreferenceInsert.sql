create proc [ContentDesign].[SmsPreferenceInsert] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@ContactId uniqueidentifier
    ,@ContactPhoneId uniqueidentifier
    ,@Number nvarchar(20)
    ,@Status int
    ,@Source int
    ,@StatusChanged datetimeoffset(7)
    ,@Notes nvarchar(max)
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[SmsPreference] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,PortalId
    ,ContactId
    ,ContactPhoneId
    ,Number
    ,Status
    ,Source
    ,StatusChanged
    ,Notes
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@PortalId
    ,@ContactId
    ,@ContactPhoneId
    ,@Number
    ,@Status
    ,@Source
    ,@StatusChanged
    ,@Notes
)

commit transaction