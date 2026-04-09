merge into [Content].[BinderType] as Target
using ( values
     ('3aaef3a5-a2ca-455a-b24f-50d0c466c510', 'Back and Next', null, 'Crudspa.Content.Display.Client.Plugins.BinderType.BackAndNextDisplay, Crudspa.Content.Display.Client', 0)
) as Source
    (Id, Name, DesignView, DisplayView, Ordinal)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.Name = Source.Name
    ,Target.DesignView = Source.DesignView
    ,Target.DisplayView = Source.DisplayView
    ,Target.Ordinal = Source.Ordinal

when not matched by target then
insert (Id, Name, DesignView, DisplayView, Ordinal)
values (Id, Name, DesignView, DisplayView, Ordinal)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;