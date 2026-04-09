create proc [FrameworkCore].[UsaPostalInsert] (
     @SessionId uniqueidentifier
    ,@RecipientName nvarchar(75)
    ,@BusinessName nvarchar(75)
    ,@StreetAddress nvarchar(150)
    ,@City nvarchar(50)
    ,@StateId uniqueidentifier
    ,@PostalCode nvarchar(10)
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

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
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@RecipientName
    ,@BusinessName
    ,@StreetAddress
    ,@City
    ,@StateId
    ,@PostalCode
)

commit transaction