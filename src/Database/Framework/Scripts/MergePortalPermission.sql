;disable trigger [Framework].[PortalPermissionTrigger] on [Framework].[PortalPermission];

declare @sessionId uniqueidentifier = '22f1a393-c003-4587-8f1d-02369d9c6c53'
declare @now datetimeoffset(7) = sysdatetimeoffset()

merge into [Framework].[PortalPermission] as Target
using ( values
     ('c5f60dd6-7c1b-46d6-809a-5d8f56654ba6', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', '1093e1a5-2914-464a-be08-a8d7cbf780b5')
    ,('32785a4f-9389-46dc-80c4-3684f6ca0e78', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', '1d8b8c58-74db-4115-8c58-4c9ea42f5beb')
    ,('acdc0901-a5df-4d4d-85ee-9ead9a65e84d', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', '26194feb-a467-47ff-9b46-038d69244763')
    ,('48b23b45-0518-46d2-9e8b-5d2e5f8df449', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', '4458062f-6894-4561-aeb1-43bef71dc825')
    ,('7c06fcba-78bf-4fe2-84a8-727592eed598', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', '55b87662-2b7d-4838-b3eb-ec15053d9ee5')
    ,('4441fa69-749e-4f45-a3da-b00d2c277a7e', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', '8615fc8d-2c8b-46c3-a586-8cc7319b889c')
    ,('fd633da1-7994-4b3f-9693-6bb3d5485c33', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', 'ac2d3d54-14f2-4f0b-ba16-bb3931883bb9')
    ,('11987188-f696-4070-b85d-1132a3f7f8bc', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', 'c661300e-8c67-4613-b5ce-a6be80acac04')
    ,('498d06bd-d404-41d5-a599-8dcf0cc1f842', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', 'f8066e13-ffad-4ee5-be65-78b3dfd9c6fb')
    ,('b2b232f7-ee6f-43a9-b1e9-29460cb74cd8', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', 'fe636e18-3654-40ab-b1f4-8e687318c405')
    ,('5a978b7a-e6ec-469c-a8ee-effa85478597', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '0fbd1165-18be-44bc-8620-f0dc4d32d587')
    ,('488f3101-4b4e-4fdf-a09d-6e985fde377e', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '55b87662-2b7d-4838-b3eb-ec15053d9ee5')
    ,('8d7f7250-5327-4b41-b7ae-6d13a6d910bf', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '8615fc8d-2c8b-46c3-a586-8cc7319b889c')
    ,('703c8d50-7120-4b07-8359-698b115ba181', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '8759c072-0f45-4e3d-a630-1412e12cde76')
    ,('ec699229-2334-45ed-8d00-20678843b950', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'ac2d3d54-14f2-4f0b-ba16-bb3931883bb9')
    ,('2e3ca518-415d-415b-8b0e-f3e2306583f2', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'b8b24c2e-b256-48cc-a5ae-43bcd2f78d80')
    ,('47904c93-4fab-45ec-8ec2-a27e14d70cd1', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'c661300e-8c67-4613-b5ce-a6be80acac04')
    ,('acd5af09-b52a-4846-80d3-93f64444ee52', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'ced746f6-ea1e-4905-b636-0bcaf2c4a1d6')
    ,('820e6b5c-09a2-43b7-8477-70ad18dffbfa', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'e1935697-f5c4-467a-9dea-f90470172a94')
    ,('172440bf-4081-47b5-b49a-2b39d035b248', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'f8066e13-ffad-4ee5-be65-78b3dfd9c6fb')
    ,('13e24a88-af71-4f0f-bbbb-8e1ff9db4d31', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'fe636e18-3654-40ab-b1f4-8e687318c405')
) as Source
    (Id, PortalId, PermissionId)
on Target.Id = Source.Id

when matched and Target.Id = Target.VersionOf then
update set
     Target.IsDeleted = 0
    ,Target.Updated = @now
    ,Target.UpdatedBy = @sessionId
    ,Target.PortalId = Source.PortalId
    ,Target.PermissionId = Source.PermissionId

when not matched by target then
insert (Id, VersionOf, Updated, UpdatedBy, PortalId, PermissionId)
values (Id, Id, @now, @sessionId, PortalId, PermissionId)

when not matched by source and Target.IsDeleted = 0 and Target.Id = Target.VersionOf then
update set
     Target.IsDeleted = 1
    ,Target.Updated = @now
    ,Target.UpdatedBy = @sessionId

;enable trigger [Framework].[PortalPermissionTrigger] on [Framework].[PortalPermission];