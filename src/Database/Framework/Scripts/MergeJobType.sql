merge into [Framework].[JobType] as Target
using ( values
     ('d17d15df-ba1d-4f29-ab74-c8c75e4ee111', 'Caption Images',  'Crudspa.Framework.Jobs.Client.Plugins.JobType.CaptionImagesDesign, Crudspa.Framework.Jobs.Client',  'Crudspa.Framework.Jobs.Server.Actions.CaptionImages, Crudspa.Framework.Jobs.Server')
    ,('bf7dfe75-2177-4103-806e-685555515942', 'Expire Sessions', 'Crudspa.Framework.Jobs.Client.Plugins.JobType.ExpireSessionsDesign, Crudspa.Framework.Jobs.Client', 'Crudspa.Framework.Jobs.Server.Actions.ExpireSessions, Crudspa.Framework.Jobs.Server')
    ,('3c03a379-d953-4823-8381-5d8772a42415', 'Optimize Audio',  'Crudspa.Framework.Jobs.Client.Plugins.JobType.OptimizeAudioDesign, Crudspa.Framework.Jobs.Client',  'Crudspa.Framework.Jobs.Server.Actions.OptimizeAudio, Crudspa.Framework.Jobs.Server')
    ,('e2adc40c-46c5-409a-9396-1ca0a1e12d0c', 'Optimize Images', 'Crudspa.Framework.Jobs.Client.Plugins.JobType.OptimizeImagesDesign, Crudspa.Framework.Jobs.Client', 'Crudspa.Framework.Jobs.Server.Actions.OptimizeImages, Crudspa.Framework.Jobs.Server')
    ,('84f36181-4bbf-413c-8732-58b61ae0ab5d', 'Optimize Video',  'Crudspa.Framework.Jobs.Client.Plugins.JobType.OptimizeVideoDesign, Crudspa.Framework.Jobs.Client',  'Crudspa.Framework.Jobs.Server.Actions.OptimizeVideo, Crudspa.Framework.Jobs.Server')
    ,('7707dafb-3c2f-4f2a-a526-d0e20a01de3c', 'Reset Files',     'Crudspa.Framework.Jobs.Client.Plugins.JobType.ResetFilesDesign, Crudspa.Framework.Jobs.Client',     'Crudspa.Framework.Jobs.Server.Actions.ResetFiles, Crudspa.Framework.Jobs.Server')
) as Source
    (Id, Name, EditorView, ActionClass)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.Name = Source.Name
    ,Target.EditorView = Source.EditorView
    ,Target.ActionClass = Source.ActionClass

when not matched by target then
insert (Id, Name, EditorView, ActionClass)
values (Id, Name, EditorView, ActionClass)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;