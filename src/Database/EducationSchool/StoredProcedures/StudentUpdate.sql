create proc [EducationSchool].[StudentUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@FirstName nvarchar(75)
    ,@LastName nvarchar(75)
    ,@SecretCode nvarchar(75)
    ,@GradeId uniqueidentifier
    ,@AssessmentLevelGroupId uniqueidentifier
    ,@PreferredName nvarchar(75)
    ,@AvatarString nvarchar(2)
    ,@IdNumber nvarchar(35)
    ,@IsTestAccount bit
) as

declare @now datetimeoffset = sysdatetimeoffset()

begin transaction

    update [Framework].[Contact]
    set FirstName = @FirstName
        ,LastName = @LastName
    where Id in (select ContactId from [Education].[Student-Active] where Id = @Id)

    update [Education].[Student]
    set
        Updated = @now
        ,UpdatedBy = @SessionId
        ,SecretCode = @SecretCode
        ,GradeId = @GradeId
        ,AssessmentLevelGroupId = @AssessmentLevelGroupId
        ,PreferredName = @PreferredName
        ,AvatarString = @AvatarString
        ,IdNumber = @IdNumber
        ,IsTestAccount = @IsTestAccount
    where Id = @Id

commit transaction