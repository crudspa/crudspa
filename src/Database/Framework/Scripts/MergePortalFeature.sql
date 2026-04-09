merge into [Framework].[PortalFeature] as Target
using ( values
     ('2b0eadf7-669f-3ae4-faed-37e622d3b27c', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', 'segments',     'Segments',     '9b1e06ee-d42b-5120-a377-8d76ca28632c', 'c661300e-8c67-4613-b5ce-a6be80acac04')
    ,('88bccd2d-0a67-4476-a952-d7b2be2c6497', '73410fd3-3681-46d3-800e-a08670e291cf', 'achievements', 'Achievements', '964d7620-6d47-4147-b096-72cd32647e0f', 'e1935697-f5c4-467a-9dea-f90470172a94')
    ,('970e4a80-b17d-4dd7-af75-d73ee406f8dd', '73410fd3-3681-46d3-800e-a08670e291cf', 'blogs',        'Blogs',        'd0b01720-40fd-5c57-838a-1dce77a9b6b8', 'b8b24c2e-b256-48cc-a5ae-43bcd2f78d80')
    ,('cea41994-49e6-49b9-a06e-25e88ce05abe', '73410fd3-3681-46d3-800e-a08670e291cf', 'details',      'Details',      '188245f0-4ac3-4ad8-a044-a27dceee4eaf', '55b87662-2b7d-4838-b3eb-ec15053d9ee5')
    ,('d490ce5d-1f00-43bd-9f1b-983772fe31bd', '73410fd3-3681-46d3-800e-a08670e291cf', 'fonts',        'Fonts',        '23bbb231-e121-4e5c-8482-500efbdcb234', '8759c072-0f45-4e3d-a630-1412e12cde76')
    ,('0001cf12-f863-4088-9493-9912cb64d747', '73410fd3-3681-46d3-800e-a08670e291cf', 'segments',     'Segments',     '9b1e06ee-d42b-5120-a377-8d76ca28632c', 'c661300e-8c67-4613-b5ce-a6be80acac04')
    ,('01a76abc-0710-402f-a0fa-668a05b32e1c', '73410fd3-3681-46d3-800e-a08670e291cf', 'styles',       'Styles',       'cfa17a64-9a0d-4722-a839-c895007ae812', '8759c072-0f45-4e3d-a630-1412e12cde76')
    ,('88c1c74a-61c9-41bd-8318-13e1438f33db', '73410fd3-3681-46d3-800e-a08670e291cf', 'tracks',       'Tracks',       'ce78b660-6929-5a74-b7be-bafa3b4912ab', '0fbd1165-18be-44bc-8620-f0dc4d32d587')
    ,('f9969941-b1d6-40ec-9138-40aee2c05b4f', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'achievements', 'Achievements', '964d7620-6d47-4147-b096-72cd32647e0f', 'e1935697-f5c4-467a-9dea-f90470172a94')
    ,('c96bb571-d9b1-4589-95f6-6a9076bf17bd', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'blogs',        'Blogs',        'd0b01720-40fd-5c57-838a-1dce77a9b6b8', 'b8b24c2e-b256-48cc-a5ae-43bcd2f78d80')
    ,('d748c6ec-60ac-4ee6-aeb3-d05bcab15b50', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'details',      'Details',      '188245f0-4ac3-4ad8-a044-a27dceee4eaf', '55b87662-2b7d-4838-b3eb-ec15053d9ee5')
    ,('7901943f-b352-48f2-9d04-94d76e88d03f', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'fonts',        'Fonts',        '23bbb231-e121-4e5c-8482-500efbdcb234', '8759c072-0f45-4e3d-a630-1412e12cde76')
    ,('d1164b79-e5d3-4848-8cf7-abf2bfd957db', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'memberships',  'Memberships',  '9598629a-dfa2-4877-9d21-e47b073d67e0', 'ced746f6-ea1e-4905-b636-0bcaf2c4a1d6')
    ,('4a59cbde-e382-4612-b893-e3264fba3c2d', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'segments',     'Segments',     '9b1e06ee-d42b-5120-a377-8d76ca28632c', 'c661300e-8c67-4613-b5ce-a6be80acac04')
    ,('7d12670f-91d3-4172-932c-b09e4e4d9ee8', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'styles',       'Styles',       'cfa17a64-9a0d-4722-a839-c895007ae812', '8759c072-0f45-4e3d-a630-1412e12cde76')
    ,('4b11c7b5-e259-40fe-97a3-77ee7ec2ce70', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'tracks',       'Tracks',       'ce78b660-6929-5a74-b7be-bafa3b4912ab', '0fbd1165-18be-44bc-8620-f0dc4d32d587')
) as Source
    (Id, PortalId, [Key], Title, IconId, PermissionId)
on Target.Id = Source.Id

when matched then
update set
     Target.PortalId = Source.PortalId
    ,Target.[Key] = Source.[Key]
    ,Target.Title = Source.Title
    ,Target.IconId = Source.IconId
    ,Target.PermissionId = Source.PermissionId

when not matched by target then
insert (Id, PortalId, [Key], Title, IconId, PermissionId)
values (Id, PortalId, [Key], Title, IconId, PermissionId)

when not matched by source then delete
;