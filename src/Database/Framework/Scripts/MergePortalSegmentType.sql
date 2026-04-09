;disable trigger [Framework].[PortalSegmentTypeTrigger] on [Framework].[PortalSegmentType];

declare @sessionId uniqueidentifier = '22f1a393-c003-4587-8f1d-02369d9c6c53'
declare @now datetimeoffset(7) = sysdatetimeoffset()

merge into [Framework].[PortalSegmentType] as Target
using ( values
     ('79bad903-3f9e-4d9f-8fa0-150539800e58', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', '35f404f9-c08b-4c71-88c9-794b60741332')
    ,('a00f8a42-561b-458e-81ce-225f81ebf2a3', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', 'e86d3ee2-22df-4cb1-bb66-ea417d34edeb')
    ,('f9e18af7-d6fb-48e0-b06e-e28d08e69684', '73410fd3-3681-46d3-800e-a08670e291cf', '35f404f9-c08b-4c71-88c9-794b60741332')
    ,('d8bf21be-60fe-4079-a680-5c0f3585ee74', '73410fd3-3681-46d3-800e-a08670e291cf', 'e86d3ee2-22df-4cb1-bb66-ea417d34edeb')
    ,('80e5a22e-27dc-433b-89d4-343d2feebae6', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '35f404f9-c08b-4c71-88c9-794b60741332')
    ,('602032ef-7cb9-43af-82de-4b546b007fb8', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'e86d3ee2-22df-4cb1-bb66-ea417d34edeb')
) as Source
    (Id, PortalId, TypeId)
on Target.Id = Source.Id

when matched and Target.Id = Target.VersionOf then
update set
     Target.IsDeleted = 0
    ,Target.Updated = @now
    ,Target.UpdatedBy = @sessionId
    ,Target.PortalId = Source.PortalId
    ,Target.TypeId = Source.TypeId

when not matched by target then
insert (Id, VersionOf, Updated, UpdatedBy, PortalId, TypeId)
values (Id, Id, @now, @sessionId, PortalId, TypeId)

when not matched by source and Target.IsDeleted = 0 and Target.Id = Target.VersionOf then
update set
     Target.IsDeleted = 1
    ,Target.Updated = @now
    ,Target.UpdatedBy = @sessionId

;enable trigger [Framework].[PortalSegmentTypeTrigger] on [Framework].[PortalSegmentType];