create proc [EducationPublisher].[ForumSelect] (
     @Id uniqueidentifier
) as

select
    forum.Id
    ,forum.Name
    ,forum.Description
    ,forum.BodyTemplateId
    ,forum.Pinned
    ,forum.DistrictId
    ,forum.SchoolId
    ,forum.InnovatorsOnly
    ,bodyTemplate.Name as BodyTemplateName
    ,districtOrganization.Name as DistrictName
    ,schoolOrganization.Name as SchoolName
    ,(select count(1) from [Education].[Post-Active] where ForumId = forum.Id) as PostCount
from [Education].[Forum-Active] forum
    inner join [Education].[BodyTemplate-Active] bodyTemplate on forum.BodyTemplateId = bodyTemplate.Id
    left join [Education].[District-Active] district on forum.DistrictId = district.Id
    left join [Education].[School-Active] school on forum.SchoolId = school.Id
    left join [Framework].[Organization-Active] districtOrganization on district.OrganizationId = districtOrganization.Id
    left join [Framework].[Organization-Active] schoolOrganization on school.OrganizationId = schoolOrganization.Id
where forum.Id = @Id