create view [Samples].[Composer-Active] as

select composer.Id as Id
    ,composer.OrganizationId as OrganizationId
from [Samples].[Composer] composer
where 1=1
    and composer.IsDeleted = 0
    and composer.VersionOf = composer.Id