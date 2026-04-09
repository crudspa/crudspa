merge into [Content].[PageType] as Target
using ( values
     ('edc52141-f8e2-4d79-8ce3-5c1925f26b68', 'Stacked Sections', null, 'Crudspa.Content.Display.Client.Plugins.PageType.StackedSections, Crudspa.Content.Display.Client', 0)
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