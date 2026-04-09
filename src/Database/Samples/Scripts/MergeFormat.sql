merge into [Samples].[Format] as Target
using ( values
     ('c7e699ec-ef0d-1fb8-0ba9-0133d99a6904', 'Hardcover',         0)
    ,('9d21dda1-8bc5-0472-8243-a8c521a41781', 'Paperback',         1)
    ,('ad5cee91-2881-4c12-b809-99a48b9c3b02', 'Pocket Paperback',  2)
    ,('da923ac2-02cb-f5c0-d240-8b1af27f03b7', 'Large Print',       3)
    ,('5958cbb5-5a9b-f8bb-9228-403c7eea93cc', 'Collector Edition', 4)
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