merge into [Framework].[UsaState] as Target
using ( values
     ('32b8d802-456a-413b-9399-7cebd44a2173', 'AA', 'Armed Forces (The Americas)')
    ,('37453188-2048-4bbe-87df-b6cef2ff3084', 'AK', 'Alaska')
    ,('1cbb9f36-d3ac-495d-bea0-7bb8373b8395', 'AL', 'Alabama')
    ,('fab24a02-c085-4310-9a0c-e9185e061884', 'AR', 'Arkansas')
    ,('fe4504ff-4e2e-4bc0-beef-b114054849c2', 'AZ', 'Arizona')
    ,('8fafeeb8-a064-4aab-876c-da40006378ca', 'CA', 'California')
    ,('c5ce50ad-51c8-4df8-86a0-d041c38ae42c', 'CO', 'Colorado')
    ,('6e4ebad8-f5e8-4225-a3ad-594c4380b847', 'CT', 'Connecticut')
    ,('2fc43d66-1482-4af1-a532-8366b10d72cd', 'DC', 'District of Columbia')
    ,('4407e20a-d03a-41c1-997a-c96bfd912981', 'DE', 'Delaware')
    ,('52963050-ef53-48d0-9483-b650d9b49e38', 'FL', 'Florida')
    ,('6b0cac14-3345-4e3c-bbac-ad1908b22651', 'GA', 'Georgia')
    ,('7b7d5043-3717-48fa-aee3-8637593dcbbf', 'HI', 'Hawaii')
    ,('aa29952c-497f-4720-9bfb-b781cf88f799', 'IA', 'Iowa')
    ,('9e8ea8ab-e287-41b9-a8f5-64d104842432', 'ID', 'Idaho')
    ,('a2f7d09c-51e1-4e8f-b1d9-389010f8fa1f', 'IL', 'Illinois')
    ,('a14d812f-3261-4a53-a0ec-c63dbab6e89e', 'IN', 'Indiana')
    ,('8064f4bc-8671-4db2-ba7f-bd5bc0958ce4', 'KS', 'Kansas')
    ,('c2babfd7-18f8-41f6-b720-35b098912131', 'KY', 'Kentucky')
    ,('d92fd5ee-b460-4e46-8233-1761907bdcce', 'LA', 'Louisiana')
    ,('a7622c3a-c355-429d-9098-37598fd0091a', 'MA', 'Massachusetts')
    ,('f36a54dd-2fec-4d4e-883f-2a2b1e4bdc0c', 'MD', 'Maryland')
    ,('4c5a80f4-3de9-4d2d-9a9c-cefa6b2c13d7', 'ME', 'Maine')
    ,('791987a5-9d08-4baa-90e4-18c43f2fdfdd', 'MI', 'Michigan')
    ,('b449f37f-d72e-4930-b815-749090228999', 'MN', 'Minnesota')
    ,('fbeed08d-5cb6-4050-9109-3690ba77dd1d', 'MO', 'Missouri')
    ,('160030ef-e6f2-4f13-8a31-cec03f95a676', 'MS', 'Mississippi')
    ,('cedf85bc-28c4-4bc5-a070-2909536cad8d', 'MT', 'Montana')
    ,('ba5a04b8-0395-4d65-9384-2319da2d8576', 'NC', 'North Carolina')
    ,('43b54cb6-65f5-4545-a06d-69aaa4e6b6d7', 'ND', 'North Dakota')
    ,('8565f2a6-d28f-447b-aec6-2c7f7d00ce3d', 'NE', 'Nebraska')
    ,('f7e39b7e-d31d-4131-a2e7-4098b5c3ef3a', 'NH', 'New Hamphsire')
    ,('451ec112-bbbe-41f5-a035-fc9a46738d92', 'NJ', 'New Jersey')
    ,('5acdcc89-5146-4934-b979-9efe7713e2ca', 'NM', 'New Mexico')
    ,('e21f7650-9872-4cd9-be24-ba9fc351b14b', 'NV', 'Nevada')
    ,('36c5bd07-7c71-4355-9104-45316d0d1500', 'NY', 'New York')
    ,('3ad74f1d-85c5-4fae-8c32-d5e0ceb07116', 'OH', 'Ohio')
    ,('1b2de1ef-7c86-4b22-a758-be607a0d94b4', 'OK', 'Oklahoma')
    ,('0bd77c98-ee9e-414f-b081-a7d43052e823', 'OR', 'Oregon')
    ,('a2e4f8e8-d904-405e-a6a3-e127f1c42fa0', 'PA', 'Pennsylvania')
    ,('a3bd0c23-6ee1-41ff-8a40-7244c3c45886', 'PR', 'Puerto Rico')
    ,('a3b0f3f5-5402-4577-bf42-964c19c9ae0e', 'RI', 'Rhode Island')
    ,('0dc13c8c-b093-4a33-9522-33cdc2a28d2e', 'SC', 'South Carolina')
    ,('fa956728-2f1c-4ea1-a304-01bd7620b35c', 'SD', 'South Dakota')
    ,('16e1fd00-2487-4c00-b404-f4246788ee54', 'TN', 'Tennessee')
    ,('4132dd18-8533-4f74-9732-90a36c8a92b7', 'TX', 'Texas')
    ,('94e9ed8c-e316-46a3-98ae-adf33ff97407', 'UT', 'Utah')
    ,('63a27264-169c-4e41-b84c-cd514ef2e548', 'VA', 'Virginia')
    ,('c839baed-0482-4a5a-9145-3a95a8688754', 'VT', 'Vermont')
    ,('dacc1374-f4ce-4f2e-b0d3-8341b3cc9daa', 'WA', 'Washington')
    ,('70bd7275-76b6-42c1-9ae4-76101b248fe0', 'WI', 'Wisconsin')
    ,('5cdfe3c1-ff7c-4abe-a95a-1ec0f1cf02bb', 'WV', 'West Virginia')
    ,('3f3f24f4-8fd0-45de-9fd8-ac5476c64e89', 'WY', 'Wyoming')
) as Source
    (Id, Abbreviation, Name)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.Abbreviation = Source.Abbreviation
    ,Target.Name = Source.Name

when not matched by target then
insert (Id, Abbreviation, Name)
values (Id, Abbreviation, Name)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;