merge into [Content].[SectionType] as Target
using ( values
     ('9850b015-3627-4ccc-808b-c00e8af289b8', 'Responsive', null, 'Crudspa.Content.Display.Client.Plugins.SectionType.Responsive, Crudspa.Content.Display.Client', 0)
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