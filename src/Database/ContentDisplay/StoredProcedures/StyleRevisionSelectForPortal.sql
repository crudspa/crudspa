create proc [ContentDisplay].[StyleRevisionSelectForPortal] (
     @PortalId uniqueidentifier
) as

select top 1
    contentPortal.StyleRevision
from [Content].[ContentPortal-Active] contentPortal
where contentPortal.PortalId = @PortalId