;disable trigger [Framework].[PortalPaneTypeTrigger] on [Framework].[PortalPaneType];

declare @sessionId uniqueidentifier = '22f1a393-c003-4587-8f1d-02369d9c6c53'
declare @now datetimeoffset(7) = sysdatetimeoffset()

merge into [Framework].[PortalPaneType] as Target
using ( values
     ('134013bc-ff8b-1416-5c82-d3145a525462', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', '0ae15328-96be-4b07-bee8-a21f99903bdb')
    ,('24861576-417b-6b3b-93ef-9c527e4abc5f', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', '3a172e8d-d547-4924-9b62-51c511ddba76')
    ,('68504571-9da0-559a-7eb9-0bda18bd28ca', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', '4d4af830-edce-4df4-92ee-fc303d42472e')
    ,('9af07400-907f-bd13-a70a-0d709d38577c', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', '55d77a1c-97f7-424d-a107-ccf434e3fd43')
    ,('9b5c6e7f-9c97-5e82-0b05-319a73dd79dc', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', '5f6d5089-135e-43df-9a6d-1c495a82ee2b')
    ,('fda4e929-2dde-21d7-feb5-88449565b4f6', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', '607c498d-8ead-4126-8463-288bdc13b6bf')
    ,('b63ee74e-8cd2-9473-9603-40908e8f7ea7', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', '6a36256b-c0e2-4f65-ab2d-0a0a854ba057')
    ,('fae3a992-ec8a-7507-f026-0322637f52e6', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', '7368a462-eaaf-4879-b65e-4a903732e82d')
    ,('fc7d6d47-9298-73e8-8ab9-a626ceea60e9', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', '7c3bffad-e780-4575-acaf-50a7bcee52d6')
    ,('dd9d4ae0-73e2-682a-4d39-945623627c01', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', '7e60acaa-e3f8-4555-913c-b583b07c3af7')
    ,('6ea17ba3-f0b7-f30c-2f13-b76140834802', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', '7f92e001-499b-44cc-a9bc-4020ca306d2a')
    ,('0291fa23-e34c-8f34-4461-c6961d27c5b8', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', '95365eeb-00f9-4de7-a2e9-87f686c1e4df')
    ,('96f4fc69-995d-8a6c-37ee-7b213bb4b66d', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', '9adc25a9-1c58-4bfc-826a-0a632f23c605')
    ,('3a65081c-c514-0405-ac06-8368ceafaf09', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', '9ce57b6b-4bb0-49b9-a35e-98d596f8f3a1')
    ,('1b717fb4-2527-039a-5d44-868020b76af0', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', 'a1a750e6-07b0-4bfe-8bfc-8b9ce3dc8bc8')
    ,('719002ce-f458-1f83-6d84-f6e54d6a4d6f', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', 'a2b63c33-02f8-440d-a81e-0280261a3939')
    ,('0ab44911-9fa1-d0ee-21c1-02ff883fdcd6', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', 'ad39508d-d998-4621-9a7a-b439c7d3dbd7')
    ,('7a932d3a-cc41-3ee0-3d1e-53b6ea0be176', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', 'b7030cac-ca49-442a-86f1-cc3a87ac6900')
    ,('d0814a66-bc65-a0dd-4300-00971c796813', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', 'c04e5667-6824-4f42-9063-3b9a7b1809e9')
    ,('6d6ad596-4f14-82d0-0f2b-ab07b9525f83', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', 'cf04391f-4e91-4fd3-a35f-c3747b1572df')
    ,('8e8e8248-e5c3-559a-410b-8bbe513f9460', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', 'f04dc913-e280-42f6-8f04-4885eb5e76a5')
    ,('67345886-5fdb-c23a-c2af-db805e854ffe', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', 'f3fcdc97-3dcb-4909-ae5c-48afb579e2cf')
    ,('b82f4154-2f99-5b03-1477-32cfe3a568a9', '651a367c-a7dd-4fe8-be5a-b70ef275a8ec', 'f40ea215-88ed-4df0-8c17-34b5178035e3')
    ,('7bc91434-9027-4820-a9d6-9534071814af', '73410fd3-3681-46d3-800e-a08670e291cf', '19e797d8-3380-4767-8878-a1c2775d732d')
    ,('66e9a94f-34d1-4d5e-a1bd-4857863452ac', '73410fd3-3681-46d3-800e-a08670e291cf', '33a466af-e9e1-42a7-ba1c-d5a09e854afe')
    ,('276fe273-05cf-4981-a592-8f6026ad9ca6', '73410fd3-3681-46d3-800e-a08670e291cf', '5244a27e-d872-4842-853a-0edd15aa0fad')
    ,('5bce59cd-368c-4ec3-b8d8-82f98f4e6191', '73410fd3-3681-46d3-800e-a08670e291cf', '5f54c629-8c8c-4a1f-a2f3-7031e246b6d1')
    ,('d01aaac7-74d2-416e-8abe-13bb841db10c', '73410fd3-3681-46d3-800e-a08670e291cf', '76c202dc-aced-4718-a497-ac506fbff162')
    ,('b4989e0d-3fe4-4ece-8a62-04845af54904', '73410fd3-3681-46d3-800e-a08670e291cf', '799670a9-b527-4447-b646-6cfdef53570b')
    ,('9e3112e7-8e47-4ad8-bbfb-fab4d14ce982', '73410fd3-3681-46d3-800e-a08670e291cf', '96a0bafa-d81b-48b2-9578-2f57422c3855')
    ,('50a692b1-6bca-4402-8acf-fa805653e0ae', '73410fd3-3681-46d3-800e-a08670e291cf', 'abe69d1b-52a7-43ef-af84-78e2af36c3fa')
    ,('8f388dc3-917a-4807-96aa-f0cf764ded31', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '070a9ccf-6080-47f7-9c88-404f89d8a6ad')
    ,('79536e90-f831-4468-86c9-485faa9a2da1', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '0cc00d0a-67ce-4069-9fb3-4df3bf3f0cc7')
    ,('22899826-ab69-49f6-834e-73f18289d3aa', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '19e797d8-3380-4767-8878-a1c2775d732d')
    ,('fc3e8434-dc23-4b99-9fa8-c2d8b1521e10', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '212b4b11-6027-4f51-9a59-d36df3dc5d1f')
    ,('f4defe85-efe0-4db6-b7f9-e48eee1f7e1f', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '25b8efd0-fd75-4b6a-86cb-f33e06186484')
    ,('c758cb99-25e9-4c31-b110-63b80115d8cb', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '313d7abf-1428-4a9f-a805-69d5b14c2c62')
    ,('9dbf03a0-c64e-4a6f-b9a0-dc7ee5cb5c39', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '3288e5df-7957-44d8-8b0d-a482490b2456')
    ,('8c76e4a1-40fb-4efb-87a0-ad6c7597d373', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '33a466af-e9e1-42a7-ba1c-d5a09e854afe')
    ,('6e45c5ef-6bce-4142-983f-721ca6a7143d', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '41f507c6-4a4d-47ec-a274-c8f0cc04a98c')
    ,('de290131-5a53-4924-b1e5-1e5c72a6bf43', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '42b4a2e8-96e6-4d98-94e2-02be7ae1bc5d')
    ,('21eb4285-50c3-46fe-9290-75444ade9af9', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '4379ca08-fc0f-42f7-82fd-9ff066ee3701')
    ,('5a0cbeb8-32f7-470a-82ee-695310756ad3', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '492dcc9e-ea4f-44de-bf16-a700a67352db')
    ,('834f083d-ffa4-4f2c-92e6-782006732cb6', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '4f843b6d-adba-4eb1-9899-2b70516d9ffa')
    ,('dfccc67d-c20a-4c43-b885-2e9820a12b05', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '5244a27e-d872-4842-853a-0edd15aa0fad')
    ,('1bbfb90f-b3ef-4617-9993-9b19b64ba7c2', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '55d77a1c-97f7-424d-a107-ccf434e3fd43')
    ,('78908ed6-476d-44fc-8084-e34b20abcbf7', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '5f54c629-8c8c-4a1f-a2f3-7031e246b6d1')
    ,('d4e0a01f-e9c0-4a99-860d-227131f098eb', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '5f6d5089-135e-43df-9a6d-1c495a82ee2b')
    ,('93b4819f-3ea6-4f3f-9640-1866134b28f0', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '6f8caf81-d904-4ee6-9767-0a1e5bcea651')
    ,('58c108c1-11d7-4b58-b43e-fe46e59a3bdf', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '706215be-da1b-42b4-9457-63d77ef0607f')
    ,('2e337eae-57c8-4153-b005-f769d01ae109', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '7368a462-eaaf-4879-b65e-4a903732e82d')
    ,('cbceb77e-1b51-4c8d-ba72-7577cd8df593', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '76c202dc-aced-4718-a497-ac506fbff162')
    ,('4475fa2b-4f23-4b89-bb84-3800bbd7e085', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '799670a9-b527-4447-b646-6cfdef53570b')
    ,('a35540cc-9bc5-4ba0-ad67-4c94b0679257', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '7b0e0f9d-1a67-4ff6-b4ec-2f4d56f7c2e1')
    ,('6bc6c6a4-170e-41d7-8332-afbb64406d39', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '7b918736-4df5-4d7d-8db8-4ba1686a84c7')
    ,('08ea27de-4faf-4568-8439-0459ee286d11', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '7c3bffad-e780-4575-acaf-50a7bcee52d6')
    ,('ef15f54b-b2a4-4649-9a4f-157099a8fd6d', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '7f92e001-499b-44cc-a9bc-4020ca306d2a')
    ,('c9d22d17-d1de-4edc-8a94-3da4b5255180', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '86e4c5fc-97f2-4579-9de2-6920b0e82df9')
    ,('0a2e2210-2e4c-4a14-9c79-57e23283274d', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '95365eeb-00f9-4de7-a2e9-87f686c1e4df')
    ,('fb32cd2a-143f-49c5-8a6f-a486b2e0c670', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '96a0bafa-d81b-48b2-9578-2f57422c3855')
    ,('e7c8b55b-5fd0-4c1a-b05d-6948213b820f', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '9bf4c6b4-2780-431c-9deb-bbc138833967')
    ,('b4efab5e-a58f-471e-a767-e76ce9685402', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '9ce57b6b-4bb0-49b9-a35e-98d596f8f3a1')
    ,('16230068-726e-4c9d-ab41-fa4e52c531cd', 'aea2c861-459a-490c-b7c3-30e5156fec9f', '9eb8218a-ee51-4be7-afa3-e107cb408fee')
    ,('db132e49-46f4-47f7-9e82-1aefdfa87571', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'a2b63c33-02f8-440d-a81e-0280261a3939')
    ,('1229fb4e-aa44-42c9-83f9-490600392b5f', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'abe69d1b-52a7-43ef-af84-78e2af36c3fa')
    ,('0b2f156d-8361-4288-8977-15834cce43ae', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'ad39508d-d998-4621-9a7a-b439c7d3dbd7')
    ,('c8f90af5-060f-4a10-8cb6-52b449e9c1e0', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'aeb0a3d0-c0d4-4a87-8d31-46038a4d4741')
    ,('603da586-c224-4759-8777-808332d6cb71', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'bebdcb61-c627-456a-baf0-8de9d389337f')
    ,('ff7a619d-a866-481e-bbdb-702a5697bf01', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'c04e5667-6824-4f42-9063-3b9a7b1809e9')
    ,('a50db41a-d15e-412a-b21d-2c00c389b365', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'c0b77408-8ec0-4a1d-87f0-019e57a6da1c')
    ,('82a63d5c-f7eb-4176-a314-6649bcdca641', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'c14d936c-b023-47ac-9be5-96b372649ea1')
    ,('05935791-07b6-40c8-a5bf-75f8a02a13af', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'c5209476-2eee-4fce-bf11-dd50c996c347')
    ,('603d3231-551d-4869-ab86-0116f3cbf270', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'c7dcfca8-bc1b-4d72-9799-0a2ee13d7f13')
    ,('60395f72-9b96-4826-a878-f006b5a585c6', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'c96ab9bf-4074-444c-9702-64cacdc59171')
    ,('b4d3d616-ef32-4d68-a75d-165b24dfe93c', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'd26484f8-b017-42f2-b963-600795d342aa')
    ,('4aab81b9-956a-408f-99de-9cb4cdace976', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'd42fef67-13e0-4730-9fdc-74934a0652a7')
    ,('db2620d2-91cd-451d-bb33-a7ef3cc0a6f8', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'd6760dc1-6d4c-412e-a24c-c7bd8f6d6bca')
    ,('4aee02ee-4e21-4316-87ac-aa13057af19d', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'd82bbf64-6cec-4f62-8c28-d20539445a70')
    ,('f3a36cc8-51d9-47a7-b0bb-3d32c9dde3af', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'e96f9f9f-8c79-46a9-9b5a-07f721b9ba21')
    ,('de75219f-f254-4ad3-8a99-f76ac5d48865', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'eb567d84-79e4-4137-85f2-63353dc07673')
    ,('2f01e905-1f11-41e3-8268-4eef4fbb338d', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'efceccc8-1515-435e-9714-6e519fba111e')
    ,('19d65465-4b74-4526-8d9d-89ffda307954', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'f04dc913-e280-42f6-8f04-4885eb5e76a5')
    ,('5ee2ffd5-3c0f-4721-a512-16bbc039ad6b', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'f4e8247b-a01d-4f98-bf7c-3e5b483a65d7')
    ,('9c830c2c-2ec5-4924-a45c-43744b634aaa', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'f6d92688-f316-47b0-a4df-e1ca2a88553a')
    ,('438122ee-3643-48f0-8f27-fb2b4982ff70', 'aea2c861-459a-490c-b7c3-30e5156fec9f', 'f7ce9bf5-9771-4fce-bc03-512663c23561')
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

;enable trigger [Framework].[PortalPaneTypeTrigger] on [Framework].[PortalPaneType];