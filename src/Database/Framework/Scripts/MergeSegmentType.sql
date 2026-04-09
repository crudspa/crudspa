merge into [Framework].[SegmentType] as Target
using ( values
     ('35f404f9-c08b-4c71-88c9-794b60741332', 'Single Pane',  'Crudspa.Framework.Core.Client.Plugins.SegmentType.SinglePaneDesign',  'Crudspa.Framework.Core.Client.Plugins.SegmentType.SinglePaneDisplay',  0)
    ,('e86d3ee2-22df-4cb1-bb66-ea417d34edeb', 'Tabbed Panes', 'Crudspa.Framework.Core.Client.Plugins.SegmentType.TabbedPanesDesign', 'Crudspa.Framework.Core.Client.Plugins.SegmentType.TabbedPanesDisplay', 1)
) as Source
    (Id, Name, EditorView, DisplayView, Ordinal)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.Name = Source.Name
    ,Target.EditorView = Source.EditorView
    ,Target.DisplayView = Source.DisplayView
    ,Target.Ordinal = Source.Ordinal

when not matched by target then
insert (Id, Name, EditorView, DisplayView, Ordinal)
values (Id, Name, EditorView, DisplayView, Ordinal)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;