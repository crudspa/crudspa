create proc [FrameworkCore].[UsaPostalUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@RecipientName nvarchar(75)
    ,@BusinessName nvarchar(75)
    ,@StreetAddress nvarchar(150)
    ,@City nvarchar(50)
    ,@StateId uniqueidentifier
    ,@PostalCode nvarchar(10)
) as

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
where Id = @Id

commit transaction