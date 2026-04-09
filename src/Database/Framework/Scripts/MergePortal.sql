merge into [Framework].[Portal] as Target
using ( values
     ('651a367c-a7dd-4fe8-be5a-b70ef275a8ec', 'catalog',  'Catalog',  '32ff24f1-6c27-4ec4-aec0-ca2b8d6ac192', '0a6041e0-5ba7-4664-9dc0-e9b05f3b7b09', 1, 1, 1)
    ,('aea2c861-459a-490c-b7c3-30e5156fec9f', 'composer', 'Composer', '7eaa4a2d-5a80-4c2a-8fc0-5fa5b70d55c1', '0a6041e0-5ba7-4664-9dc0-e9b05f3b7b09', 1, 1, 0)
    ,('73410fd3-3681-46d3-800e-a08670e291cf', 'consumer', 'Consumer', '7eaa4a2d-5a80-4c2a-8fc0-5fa5b70d55c1', 'f4392140-4918-4afe-8fde-1bad1ab0a822', 1, 0, 0)
) as Source
    (Id, [Key], Title, OwnerId, NavigationTypeId, SessionsPersist, AllowSignIn, RequireSignIn)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.[Key] = Source.[Key]
    ,Target.Title = Source.Title
    ,Target.OwnerId = Source.OwnerId
    ,Target.NavigationTypeId = Source.NavigationTypeId
    ,Target.SessionsPersist = Source.SessionsPersist
    ,Target.AllowSignIn = Source.AllowSignIn
    ,Target.RequireSignIn = Source.RequireSignIn

when not matched by target then
insert (Id, [Key], Title, OwnerId, NavigationTypeId, SessionsPersist, AllowSignIn, RequireSignIn)
values (Id, [Key], Title, OwnerId, NavigationTypeId, SessionsPersist, AllowSignIn, RequireSignIn)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;