create proc [ContentDisplay].[SurveyIsAccessible] (
     @LicenseIds [Framework].[IdList] readonly
    ,@SurveyId uniqueidentifier = null
    ,@SurveyReplyId uniqueidentifier = null
    ,@IsAccessible bit output
) as

if (@SurveyId is null and @SurveyReplyId is not null)
begin
    select @SurveyId = SurveyId
    from [Content].[SurveyReply-Active]
    where Id = @SurveyReplyId
end

set @IsAccessible = case
    when @SurveyId is null then 0
    when not exists (
        select 1
        from [Content].[SurveyLicense-Active] surveyLicense
            inner join [Framework].[License-Active] license on license.Id = surveyLicense.LicenseId
        where surveyLicense.SurveyId = @SurveyId
    ) then 1
    when exists (
        select 1
        from [Content].[SurveyLicense-Active] surveyLicense
            inner join [Framework].[License-Active] license on license.Id = surveyLicense.LicenseId
            inner join @LicenseIds sessionLicense on sessionLicense.Id = license.Id
        where surveyLicense.SurveyId = @SurveyId
    ) then 1
    else 0
end