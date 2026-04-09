create table [Samples].[CatalogContact] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ContactId] uniqueidentifier not null,
    [UserId] uniqueidentifier null,
    [LastIp] varbinary(16) null,
    constraint [PK_Samples_CatalogContact] primary key clustered ([Id]),
    constraint [FK_Samples_CatalogContact_Contact] foreign key ([ContactId]) references [Framework].[Contact] ([Id]),
    constraint [FK_Samples_CatalogContact_User] foreign key ([UserId]) references [Framework].[User] ([Id]),
);