create proc [ContentDesign].[ButtonSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

set nocount on

select
     button.Id
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
from [Content].[Button-Active] button
    left join [Framework].[ImageFile-Active] image on button.ImageId = image.Id
where button.Id = @Id