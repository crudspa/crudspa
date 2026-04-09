merge into [Content].[RuleType] as Target
using ( values
     ('43e8a13e-081b-4307-8807-50501ecd6bad', 'Color',      'Crudspa.Content.Design.Client.Plugins.RuleType.ColorDesign, Crudspa.Content.Design.Client',     'Crudspa.Content.Design.Client.Plugins.RuleType.ColorDisplay, Crudspa.Content.Design.Client')
    ,('fd46817d-4f10-45aa-b35f-f1cecc53a329', 'Color Pair', 'Crudspa.Content.Design.Client.Plugins.RuleType.ColorPairDesign, Crudspa.Content.Design.Client', 'Crudspa.Content.Design.Client.Plugins.RuleType.ColorPairDisplay, Crudspa.Content.Design.Client')
    ,('249e8ff8-8157-4e12-af8f-e72fad73dd9c', 'Font',       'Crudspa.Content.Design.Client.Plugins.RuleType.FontDesign, Crudspa.Content.Design.Client',      'Crudspa.Content.Design.Client.Plugins.RuleType.FontDisplay, Crudspa.Content.Design.Client')
    ,('067be8fd-dee9-4c39-8f69-df802371c606', 'Margin',     'Crudspa.Content.Design.Client.Plugins.RuleType.MarginDesign, Crudspa.Content.Design.Client',    'Crudspa.Content.Design.Client.Plugins.RuleType.MarginDisplay, Crudspa.Content.Design.Client')
    ,('a32611df-7b8d-4b8c-b62f-e596589ef992', 'Padding',    'Crudspa.Content.Design.Client.Plugins.RuleType.PaddingDesign, Crudspa.Content.Design.Client',   'Crudspa.Content.Design.Client.Plugins.RuleType.PaddingDisplay, Crudspa.Content.Design.Client')
    ,('5ab192a1-7bc7-4d3a-bdcc-884ad31d1e04', 'Roundness',  'Crudspa.Content.Design.Client.Plugins.RuleType.RoundnessDesign, Crudspa.Content.Design.Client', 'Crudspa.Content.Design.Client.Plugins.RuleType.RoundnessDisplay, Crudspa.Content.Design.Client')
    ,('945c7ae7-0f88-4047-9f70-83a52512b897', 'Zoom',       'Crudspa.Content.Design.Client.Plugins.RuleType.ZoomDesign, Crudspa.Content.Design.Client',      'Crudspa.Content.Design.Client.Plugins.RuleType.ZoomDisplay, Crudspa.Content.Design.Client')
) as Source
    (Id, Name, EditorView, DisplayView)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.Name = Source.Name
    ,Target.EditorView = Source.EditorView
    ,Target.DisplayView = Source.DisplayView

when not matched by target then
insert (Id, Name, EditorView, DisplayView)
values (Id, Name, EditorView, DisplayView)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;