create view [Education].[District-Active] as

select district.Id as Id
    ,district.OrganizationId as OrganizationId
    ,district.PublisherId as PublisherId
    ,district.AddressId as AddressId
    ,district.StudentIdNumberLabel as StudentIdNumberLabel
    ,district.AssessmentExplainer as AssessmentExplainer
from [Education].[District] district
where 1=1
    and district.IsDeleted = 0
    and district.VersionOf = district.Id