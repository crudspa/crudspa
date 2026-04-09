merge into [Framework].[NavigationType] as Target
using ( values
     ('f4392140-4918-4afe-8fde-1bad1ab0a822', 'Horizontal', 'Crudspa.Framework.Core.Client.Plugins.NavigationType.HorizontalNavigation, Crudspa.Framework.Core.Client')
    ,('ec44a236-ea1e-420a-b5f7-cd38613c563c', 'Stack',      'Crudspa.Framework.Core.Client.Plugins.NavigationType.StackNavigation, Crudspa.Framework.Core.Client')
    ,('0a6041e0-5ba7-4664-9dc0-e9b05f3b7b09', 'Vertical',   'Crudspa.Framework.Core.Client.Plugins.NavigationType.VerticalNavigation, Crudspa.Framework.Core.Client')
) as Source
    (Id, Name, DisplayView)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.Name = Source.Name
    ,Target.DisplayView = Source.DisplayView

when not matched by target then
insert (Id, Name, DisplayView)
values (Id, Name, DisplayView)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;