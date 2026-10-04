create proc [ContentDisplay].[PublicationResourceTypeSelectOrderables] (
     @SessionId uniqueidentifier
) as

set nocount on

select
     publicationResourceType.Id
    ,publicationResourceType.Name as Name
    ,publicationResourceType.Ordinal
from [Content].[PublicationResourceType-Active] publicationResourceType
order by publicationResourceType.Ordinal