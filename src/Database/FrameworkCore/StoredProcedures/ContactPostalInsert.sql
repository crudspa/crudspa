create proc [FrameworkCore].[ContactPostalInsert] (
     @SessionId uniqueidentifier
    ,@ContactId uniqueidentifier
    ,@RecipientName nvarchar(75)
    ,@BusinessName nvarchar(75)
    ,@StreetAddress nvarchar(150)
    ,@City nvarchar(50)
    ,@StateId uniqueidentifier
    ,@PostalCode nvarchar(10)
    ,@Ordinal int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

declare @postalId uniqueidentifier = newid()

begin transaction

insert [Framework].[UsaPostal] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,RecipientName
    ,BusinessName
    ,StreetAddress
    ,City
    ,StateId
    ,PostalCode
)
values (
     @postalId
    ,@postalId
    ,@now
    ,@SessionId
    ,@RecipientName
    ,@BusinessName
    ,@StreetAddress
    ,@City
    ,@StateId
    ,@PostalCode
)

insert [Framework].[ContactPostal] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ContactId
    ,PostalId
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ContactId
    ,@postalId
    ,@Ordinal
)

commit transaction