create proc [FrameworkCore].[ContactPostalUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@RecipientName nvarchar(75)
    ,@BusinessName nvarchar(75)
    ,@StreetAddress nvarchar(150)
    ,@City nvarchar(50)
    ,@StateId uniqueidentifier
    ,@PostalCode nvarchar(10)
    ,@Ordinal int
) as

declare @postalId uniqueidentifier = (select top 1 PostalId from [Framework].[ContactPostal-Active] where Id = @Id)

declare @now datetimeoffset = sysdatetimeoffset()

begin transaction

update [Framework].[UsaPostal]
set  Updated = @now
    ,UpdatedBy = @SessionId
    ,RecipientName = @RecipientName
    ,BusinessName = @BusinessName
    ,StreetAddress = @StreetAddress
    ,City = @City
    ,StateId = @StateId
    ,PostalCode = @PostalCode
where Id = @postalId

update [Framework].[ContactPostal]
set  Updated = @now
    ,UpdatedBy = @SessionId
    ,Ordinal = @Ordinal
where Id = @Id

commit transaction