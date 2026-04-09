merge into [Framework].[Permission] as Target
using ( values
     ('e1935697-f5c4-467a-9dea-f90470172a94', 'Achievements')
    ,('b8b24c2e-b256-48cc-a5ae-43bcd2f78d80', 'Blogs')
    ,('1d8b8c58-74db-4115-8c58-4c9ea42f5beb', 'Books')
    ,('f8066e13-ffad-4ee5-be65-78b3dfd9c6fb', 'Jobs')
    ,('ced746f6-ea1e-4905-b636-0bcaf2c4a1d6', 'Memberships')
    ,('26194feb-a467-47ff-9b46-038d69244763', 'Movies')
    ,('233c0886-6525-4f29-b21f-1a953a69fd01', 'Pages')
    ,('55b87662-2b7d-4838-b3eb-ec15053d9ee5', 'Portals')
    ,('c661300e-8c67-4613-b5ce-a6be80acac04', 'Segments')
    ,('ac2d3d54-14f2-4f0b-ba16-bb3931883bb9', 'Settings | Account')
    ,('8615fc8d-2c8b-46c3-a586-8cc7319b889c', 'Settings | Contacts')
    ,('fe636e18-3654-40ab-b1f4-8e687318c405', 'Settings | Organization')
    ,('1093e1a5-2914-464a-be08-a8d7cbf780b5', 'Shirts')
    ,('8759c072-0f45-4e3d-a630-1412e12cde76', 'Styles')
    ,('4458062f-6894-4561-aeb1-43bef71dc825', 'Templates')
    ,('0fbd1165-18be-44bc-8620-f0dc4d32d587', 'Tracks')
) as Source
    (Id, Name)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.Name = Source.Name

when not matched by target then
insert (Id, Name)
values (Id, Name)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;