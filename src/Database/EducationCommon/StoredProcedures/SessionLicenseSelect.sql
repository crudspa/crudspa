create proc [EducationCommon].[SessionLicenseSelect] (
    @SessionId uniqueidentifier
) as
begin
    set nocount on;

    select sessionLicense.LicenseId
    from [EducationCommon].[SessionLicenses](@SessionId) sessionLicense
    order by sessionLicense.LicenseId;
end