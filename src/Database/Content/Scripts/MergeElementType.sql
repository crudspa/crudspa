merge into [Content].[ElementType] as Target
using ( values
     ('2b3b70d2-2fb6-47e3-835c-2573f40938e3', 'Activity',   '9f16a102-eb05-5fe8-aea7-05c5224e5f1a', 'Crudspa.Education.Publisher.Client.Plugins.ElementType.ActivityElementDesign, Crudspa.Education.Publisher.Client', 'Crudspa.Education.Common.Client.Plugins.ElementType.ActivityElementDisplay, Crudspa.Education.Common.Client', 'Crudspa.Education.Publisher.Server.Services.ElementRepositoryActivity, Crudspa.Education.Publisher.Server', 1, 1, 0)
    ,('6cb0ea78-4cfd-4353-8d00-b6907ebf961d', 'Audio',      '80d9433d-5255-411f-9e32-35b33c4e5ecf', 'Crudspa.Content.Design.Client.Plugins.ElementType.AudioElementDesign, Crudspa.Content.Design.Client',              'Crudspa.Content.Display.Client.Plugins.ElementType.AudioElementDisplay, Crudspa.Content.Display.Client',      'Crudspa.Content.Design.Server.Services.ElementRepositoryAudio, Crudspa.Content.Design.Server',              0, 1, 1)
    ,('378cc04f-db6c-4e32-8bd5-adcfb7e8f393', 'Button',     'ab28e1e9-ec15-5648-bad8-b68f59cb2429', 'Crudspa.Content.Design.Client.Plugins.ElementType.ButtonElementDesign, Crudspa.Content.Design.Client',             'Crudspa.Content.Display.Client.Plugins.ElementType.ButtonElementDisplay, Crudspa.Content.Display.Client',     'Crudspa.Content.Design.Server.Services.ElementRepositoryButton, Crudspa.Content.Design.Server',             0, 0, 2)
    ,('e83da559-bc63-49fe-b94e-f69432205efb', 'Image',      'e82390cd-1e3c-53c8-bf1b-2ea889f35dfd', 'Crudspa.Content.Design.Client.Plugins.ElementType.ImageElementDesign, Crudspa.Content.Design.Client',              'Crudspa.Content.Display.Client.Plugins.ElementType.ImageElementDisplay, Crudspa.Content.Display.Client',      'Crudspa.Content.Design.Server.Services.ElementRepositoryImage, Crudspa.Content.Design.Server',              0, 0, 3)
    ,('10c2c0db-2aab-43bd-abc9-ec1b422f3907', 'Multimedia', 'b78dd648-1b87-5514-b166-f64657363a47', 'Crudspa.Content.Design.Client.Plugins.ElementType.MultimediaElementDesign, Crudspa.Content.Design.Client',         'Crudspa.Content.Display.Client.Plugins.ElementType.MultimediaElementDisplay, Crudspa.Content.Display.Client', 'Crudspa.Content.Design.Server.Services.ElementRepositoryMultimedia, Crudspa.Content.Design.Server',         0, 0, 4)
    ,('db70aeeb-3d2e-42f0-bbb2-61a6baf3f6f8', 'Note',       '2760eeee-dfe0-5bc9-b800-41ae8019d7d1', 'Crudspa.Content.Design.Client.Plugins.ElementType.NoteElementDesign, Crudspa.Content.Design.Client',               'Crudspa.Content.Display.Client.Plugins.ElementType.NoteElementDisplay, Crudspa.Content.Display.Client',       'Crudspa.Content.Design.Server.Services.ElementRepositoryNote, Crudspa.Content.Design.Server',               0, 1, 5)
    ,('52ce0670-8bae-43b6-aca2-9f5ca0c03f41', 'PDF',        '4ecc5c14-5488-4ea7-80bf-ff2b389d24f5', 'Crudspa.Content.Design.Client.Plugins.ElementType.PdfElementDesign, Crudspa.Content.Design.Client',                'Crudspa.Content.Display.Client.Plugins.ElementType.PdfElementDisplay, Crudspa.Content.Display.Client',        'Crudspa.Content.Design.Server.Services.ElementRepositoryPdf, Crudspa.Content.Design.Server',                0, 0, 6)
    ,('46931711-0f53-4791-9256-b0437a6c1ee6', 'Text',       'd0b01720-40fd-5c57-838a-1dce77a9b6b8', 'Crudspa.Content.Design.Client.Plugins.ElementType.TextElementDesign, Crudspa.Content.Design.Client',               'Crudspa.Content.Display.Client.Plugins.ElementType.TextElementDisplay, Crudspa.Content.Display.Client',       'Crudspa.Content.Design.Server.Services.ElementRepositoryTextElement, Crudspa.Content.Design.Server',        0, 0, 7)
    ,('dcffec78-c3da-420f-9512-a7f9e09a0fbd', 'Video',      '62766add-ea0a-5de8-9a58-96798b3b4bb9', 'Crudspa.Content.Design.Client.Plugins.ElementType.VideoElementDesign, Crudspa.Content.Design.Client',              'Crudspa.Content.Display.Client.Plugins.ElementType.VideoElementDisplay, Crudspa.Content.Display.Client',      'Crudspa.Content.Design.Server.Services.ElementRepositoryVideo, Crudspa.Content.Design.Server',              0, 1, 8)
) as Source
    (Id, Name, IconId, EditorView, DisplayView, RepositoryClass, OnlyChild, SupportsInteraction, Ordinal)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.Name = Source.Name
    ,Target.IconId = Source.IconId
    ,Target.EditorView = Source.EditorView
    ,Target.DisplayView = Source.DisplayView
    ,Target.RepositoryClass = Source.RepositoryClass
    ,Target.OnlyChild = Source.OnlyChild
    ,Target.SupportsInteraction = Source.SupportsInteraction
    ,Target.Ordinal = Source.Ordinal

when not matched by target then
insert (Id, Name, IconId, EditorView, DisplayView, RepositoryClass, OnlyChild, SupportsInteraction, Ordinal)
values (Id, Name, IconId, EditorView, DisplayView, RepositoryClass, OnlyChild, SupportsInteraction, Ordinal)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;