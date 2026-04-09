merge into [Samples].[Size] as Target
using ( values
     ('cc5d5ce0-573b-5393-5116-0b9de84c3a06', '6c56b974-f606-53ed-77f7-517ace1c7703', 'XS',  0)
    ,('4dc7f9c0-3e69-db46-534d-346fde9eab7c', '09dca023-98e9-0aab-c5ca-fadc47b51f45', 'XS',  0)
    ,('87240a5c-8a2c-6b3f-4172-39a3d0d2cbf3', '292cdee1-692f-9fa9-c0fa-e57ede0ea444', 'XS',  0)
    ,('e1ccd114-b398-f2e2-d4e0-3baa308b318e', '87a436d4-219d-64bb-d8ac-4872a724473f', 'XS',  0)
    ,('712ba84b-d729-1512-3478-65e22f90555b', 'db3dbdac-0f01-a9cb-9a2f-50be0f44608b', 'XS',  0)
    ,('1935c371-d8f3-e375-db47-7b634227252c', '26468024-3db4-41e9-7631-f5d55e316a6c', 'XS',  0)
    ,('16b9055a-6c5f-05d2-f7af-e73c6e15fd95', '003f602d-e04a-6799-87ba-e51184c9f43c', 'XS',  0)
    ,('9ef59d13-392c-7978-e2de-ec3abdc860b1', '8a556412-792d-b91c-6b9a-57b847267944', 'XS',  0)
    ,('4505e834-52cd-e114-382e-fbca38709dfd', '5e5de436-a5ed-79a3-6b28-2e0356493b5f', 'XS',  0)
    ,('a905e1d1-87cd-cd26-2f00-ffc7433c27ae', '0d2cfb3c-1930-d609-d7c6-d20dd0793b06', 'XS',  0)
    ,('c0d208a1-a202-b61b-bb0b-9c6a1287753c', '5e5de436-a5ed-79a3-6b28-2e0356493b5f', 'S',   1)
    ,('6c3ad6fc-fd17-fb00-12e1-b1eecea81cbc', '003f602d-e04a-6799-87ba-e51184c9f43c', 'S',   1)
    ,('f6407077-6f78-afed-fc8f-b5732ad5d0d1', '09dca023-98e9-0aab-c5ca-fadc47b51f45', 'S',   1)
    ,('6ac531d8-d5a5-bf92-193c-bbbce77fdab5', '87a436d4-219d-64bb-d8ac-4872a724473f', 'S',   1)
    ,('5ffead5e-2b56-3fd3-956c-c8c490ad4ad4', '292cdee1-692f-9fa9-c0fa-e57ede0ea444', 'S',   1)
    ,('124d3439-5fbe-3e4c-05de-658c9a712ea8', '8a556412-792d-b91c-6b9a-57b847267944', 'S',   1)
    ,('53294af7-a1ab-d4aa-4834-3751c2e84a74', '6c56b974-f606-53ed-77f7-517ace1c7703', 'S',   1)
    ,('10b4f5b2-11dd-c595-505e-1a0fbca64c2a', '0d2cfb3c-1930-d609-d7c6-d20dd0793b06', 'S',   1)
    ,('867ce5e7-cffa-21fa-4cb4-232e7da85011', '26468024-3db4-41e9-7631-f5d55e316a6c', 'S',   1)
    ,('595c2c25-3328-42ad-6f03-29fb8c761264', 'db3dbdac-0f01-a9cb-9a2f-50be0f44608b', 'S',   1)
    ,('aee06ffd-954d-aca9-b3d3-2ad7ebd76e0a', '0d2cfb3c-1930-d609-d7c6-d20dd0793b06', 'M',   2)
    ,('6648f879-faf3-47dc-9489-71f4d23c11f8', '5e5de436-a5ed-79a3-6b28-2e0356493b5f', 'M',   2)
    ,('24c974dd-80a9-8d99-e9e0-563969c85991', '8a556412-792d-b91c-6b9a-57b847267944', 'M',   2)
    ,('1a5334fa-ee63-1a32-d4ac-5ea2a204012e', '292cdee1-692f-9fa9-c0fa-e57ede0ea444', 'M',   2)
    ,('12d09506-b865-b906-6066-b33fb8120962', '003f602d-e04a-6799-87ba-e51184c9f43c', 'M',   2)
    ,('cf6a52d3-8250-e600-dd5a-b407bb4223c7', 'db3dbdac-0f01-a9cb-9a2f-50be0f44608b', 'M',   2)
    ,('4855aa98-41ff-2488-2185-b2239daf2b59', '87a436d4-219d-64bb-d8ac-4872a724473f', 'M',   2)
    ,('2b1b4b93-e402-05cf-652c-a8522228cd46', '26468024-3db4-41e9-7631-f5d55e316a6c', 'M',   2)
    ,('46ef2312-d477-31d7-bff6-fd78b33fc14c', '6c56b974-f606-53ed-77f7-517ace1c7703', 'M',   2)
    ,('8cab0642-cd17-d2a4-821e-e79f68f9f3ab', '09dca023-98e9-0aab-c5ca-fadc47b51f45', 'M',   2)
    ,('745747a8-22a3-17f7-7c80-de344444c2c6', '87a436d4-219d-64bb-d8ac-4872a724473f', 'L',   3)
    ,('42c81fd1-1f3e-b7fe-5710-df2972c1281c', '5e5de436-a5ed-79a3-6b28-2e0356493b5f', 'L',   3)
    ,('8d263986-1c63-ddf3-d4ae-e19a4620db49', '003f602d-e04a-6799-87ba-e51184c9f43c', 'L',   3)
    ,('3d7bd2fe-14ff-c392-02f6-936bbb3b9d19', '09dca023-98e9-0aab-c5ca-fadc47b51f45', 'L',   3)
    ,('6b6eea26-1902-b16c-37a8-9aae4a490825', '26468024-3db4-41e9-7631-f5d55e316a6c', 'L',   3)
    ,('4e130a3c-4f1b-79eb-78e0-566f8bbd2f79', '292cdee1-692f-9fa9-c0fa-e57ede0ea444', 'L',   3)
    ,('abe2a269-9901-ad8b-8c54-6bf75171616b', '6c56b974-f606-53ed-77f7-517ace1c7703', 'L',   3)
    ,('76b9c9c9-80c5-e792-5bfc-202be5b20120', '8a556412-792d-b91c-6b9a-57b847267944', 'L',   3)
    ,('38de530c-d16f-387d-96e2-0e743e9f841d', 'db3dbdac-0f01-a9cb-9a2f-50be0f44608b', 'L',   3)
    ,('215adb6c-7bf2-e3a6-cc99-3d503daa0281', '0d2cfb3c-1930-d609-d7c6-d20dd0793b06', 'L',   3)
    ,('887334b8-1f05-7726-d13c-552bb25590f4', '292cdee1-692f-9fa9-c0fa-e57ede0ea444', 'XL',  4)
    ,('5e0a74c8-c5f1-39ac-1659-778776c36212', '87a436d4-219d-64bb-d8ac-4872a724473f', 'XL',  4)
    ,('d86703e6-dc51-43e2-12da-57b658dc795f', '0d2cfb3c-1930-d609-d7c6-d20dd0793b06', 'XL',  4)
    ,('ee8ed79f-1433-964a-8521-9bacf88bb268', '003f602d-e04a-6799-87ba-e51184c9f43c', 'XL',  4)
    ,('d7b3cbc1-58f3-19fc-cd38-9c80cdd9033c', '09dca023-98e9-0aab-c5ca-fadc47b51f45', 'XL',  4)
    ,('0ac5e19d-5047-8f16-fab0-fb012bcf1b51', '26468024-3db4-41e9-7631-f5d55e316a6c', 'XL',  4)
    ,('11d951ec-e537-472c-8698-e0b29957d641', '6c56b974-f606-53ed-77f7-517ace1c7703', 'XL',  4)
    ,('3ef75578-3e9d-a11c-2dd3-e8a42ae8a98c', '8a556412-792d-b91c-6b9a-57b847267944', 'XL',  4)
    ,('8a6dce17-9a23-566d-ccbc-ea104411332d', '5e5de436-a5ed-79a3-6b28-2e0356493b5f', 'XL',  4)
    ,('b8b96680-ffc6-43f4-fa87-eb5beabd7122', 'db3dbdac-0f01-a9cb-9a2f-50be0f44608b', 'XL',  4)
    ,('8d7fc83a-f030-7a41-5883-f184be823b0f', '09dca023-98e9-0aab-c5ca-fadc47b51f45', '2XL', 5)
    ,('a8ea5073-289d-8af5-b83a-f7ae8a0f291a', 'db3dbdac-0f01-a9cb-9a2f-50be0f44608b', '2XL', 5)
    ,('bbaafc0f-ee63-4a6b-db46-b2f040a03c5e', '292cdee1-692f-9fa9-c0fa-e57ede0ea444', '2XL', 5)
    ,('5c267e0f-35a5-3576-a657-babbe9be8ee4', '5e5de436-a5ed-79a3-6b28-2e0356493b5f', '2XL', 5)
    ,('42879253-c242-f781-f2be-d1606638b445', '6c56b974-f606-53ed-77f7-517ace1c7703', '2XL', 5)
    ,('4777d169-c7fd-1280-bc46-0aee2157dfdd', '26468024-3db4-41e9-7631-f5d55e316a6c', '2XL', 5)
    ,('bcfadba4-dc81-e6fc-013a-4729989318b7', '0d2cfb3c-1930-d609-d7c6-d20dd0793b06', '2XL', 5)
    ,('33170ab2-cd26-79f7-6a39-700206b9e4a9', '8a556412-792d-b91c-6b9a-57b847267944', '2XL', 5)
    ,('4f163b72-616d-2cd3-e77b-3bd40ad3bb5c', '87a436d4-219d-64bb-d8ac-4872a724473f', '2XL', 5)
    ,('4f4c4c8f-9251-b119-6ac1-12fae3626cf0', '003f602d-e04a-6799-87ba-e51184c9f43c', '2XL', 5)
    ,('576bacd0-4caa-95fd-c9a2-2414fee6a11f', '87a436d4-219d-64bb-d8ac-4872a724473f', '3XL', 6)
    ,('9ad613ef-9165-8fad-6d09-27d245363d6c', '09dca023-98e9-0aab-c5ca-fadc47b51f45', '3XL', 6)
    ,('9c91d103-3250-f9f7-328a-3ffc1323967b', '26468024-3db4-41e9-7631-f5d55e316a6c', '3XL', 6)
    ,('e6d417a5-4726-df37-8e5f-3ba4de7a355a', '8a556412-792d-b91c-6b9a-57b847267944', '3XL', 6)
    ,('04d9142f-2681-4b9b-be0f-862a0a0936c4', 'db3dbdac-0f01-a9cb-9a2f-50be0f44608b', '3XL', 6)
    ,('aaaf4e95-15ee-a082-71dd-608b1ca2e422', '003f602d-e04a-6799-87ba-e51184c9f43c', '3XL', 6)
    ,('de2bdc28-ca9e-fb85-c4c4-59a3ae3bae1e', '0d2cfb3c-1930-d609-d7c6-d20dd0793b06', '3XL', 6)
    ,('20f4c115-2db1-b0bb-6ed3-da9d77b947e4', '5e5de436-a5ed-79a3-6b28-2e0356493b5f', '3XL', 6)
    ,('e120c5d4-2a14-1a37-fb9d-c0a4bb602b33', '6c56b974-f606-53ed-77f7-517ace1c7703', '3XL', 6)
    ,('8dedaff0-5462-333f-37ab-ae53e1a2d3e0', '292cdee1-692f-9fa9-c0fa-e57ede0ea444', '3XL', 6)
) as Source
    (Id, ColorId, Name, Ordinal)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.ColorId = Source.ColorId
    ,Target.Name = Source.Name
    ,Target.Ordinal = Source.Ordinal

when not matched by target then
insert (Id, ColorId, Name, Ordinal)
values (Id, ColorId, Name, Ordinal)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;