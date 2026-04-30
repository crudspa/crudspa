merge into [Content].[AnswerType] as Target
using ( values
     ('745ed948-2d54-47fb-94b2-d215eacd08a9', 'Boolean', 'Crudspa.Content.Design.Client.Plugins.AnswerType.BooleanAnswerDesign, Crudspa.Content.Design.Client', 'Crudspa.Content.Display.Client.Plugins.AnswerType.BooleanAnswerDisplay, Crudspa.Content.Display.Client')
    ,('d12292db-f701-4293-bb00-bc3fff720d35', 'Contact', 'Crudspa.Content.Design.Client.Plugins.AnswerType.ContactAnswerDesign, Crudspa.Content.Design.Client', 'Crudspa.Content.Display.Client.Plugins.AnswerType.ContactAnswerDisplay, Crudspa.Content.Display.Client')
    ,('a5eeeac9-d8e5-4041-acc6-d673a7fe805f', 'Date',    'Crudspa.Content.Design.Client.Plugins.AnswerType.DateAnswerDesign, Crudspa.Content.Design.Client',    'Crudspa.Content.Display.Client.Plugins.AnswerType.DateAnswerDisplay, Crudspa.Content.Display.Client')
    ,('e193885b-84eb-4bd5-88f5-22e7447e50ef', 'File',    'Crudspa.Content.Design.Client.Plugins.AnswerType.FileAnswerDesign, Crudspa.Content.Design.Client',    'Crudspa.Content.Display.Client.Plugins.AnswerType.FileAnswerDisplay, Crudspa.Content.Display.Client')
    ,('8e010d95-f00b-416a-b47f-8ebf326bcfec', 'Number',  'Crudspa.Content.Design.Client.Plugins.AnswerType.NumberAnswerDesign, Crudspa.Content.Design.Client',  'Crudspa.Content.Display.Client.Plugins.AnswerType.NumberAnswerDisplay, Crudspa.Content.Display.Client')
    ,('d9f45363-bd04-4c10-acf4-e283ddb0aef0', 'Options', 'Crudspa.Content.Design.Client.Plugins.AnswerType.OptionsAnswerDesign, Crudspa.Content.Design.Client', 'Crudspa.Content.Display.Client.Plugins.AnswerType.OptionsAnswerDisplay, Crudspa.Content.Display.Client')
    ,('fe4c3757-ea9d-44d7-a3d1-494b64eeb15f', 'Scale',   'Crudspa.Content.Design.Client.Plugins.AnswerType.ScaleAnswerDesign, Crudspa.Content.Design.Client',   'Crudspa.Content.Display.Client.Plugins.AnswerType.ScaleAnswerDisplay, Crudspa.Content.Display.Client')
    ,('93331ccb-5223-419b-a9ed-6263311ecd5b', 'Text',    'Crudspa.Content.Design.Client.Plugins.AnswerType.TextAnswerDesign, Crudspa.Content.Design.Client',    'Crudspa.Content.Display.Client.Plugins.AnswerType.TextAnswerDisplay, Crudspa.Content.Display.Client')
) as Source
    (Id, Name, DesignView, DisplayView)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.Name = Source.Name
    ,Target.DesignView = Source.DesignView
    ,Target.DisplayView = Source.DisplayView

when not matched by target then
insert (Id, Name, DesignView, DisplayView)
values (Id, Name, DesignView, DisplayView)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;