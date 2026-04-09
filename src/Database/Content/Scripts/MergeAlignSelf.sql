merge into [Content].[AlignSelf] as Target
using ( values
     ('06931a95-b761-4c92-b721-e168f911f4b4', 'Auto',     0)
    ,('25aa2b77-064b-4d03-b86b-18b4ac161f5a', 'Baseline', 1)
    ,('4a946989-85d6-4388-935d-8fffe4892feb', 'Center',   2)
    ,('8a8e0b8c-a09c-427a-a0a6-5fddcefe7948', 'End',      3)
    ,('e5684aed-c7c3-45fa-aa0a-4dfb9087cbc4', 'Start',    4)
    ,('1a9c4be2-4373-4eef-ac2e-32b2c71d2aa2', 'Stretch',  5)
) as Source
    (Id, Name, Ordinal)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.Name = Source.Name
    ,Target.Ordinal = Source.Ordinal

when not matched by target then
insert (Id, Name, Ordinal)
values (Id, Name, Ordinal)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;