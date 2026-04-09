create table [Education].[ProviderContact] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ProviderId] uniqueidentifier not null,
    [ContactId] uniqueidentifier not null,
    [UserId] uniqueidentifier null,
    constraint [PK_Education_ProviderContact] primary key clustered ([Id]),
    constraint [FK_Education_ProviderContact_Provider] foreign key ([ProviderId]) references [Education].[Provider] ([Id]),
    constraint [FK_Education_ProviderContact_Contact] foreign key ([ContactId]) references [Framework].[Contact] ([Id]),
    constraint [FK_Education_ProviderContact_User] foreign key ([UserId]) references [Framework].[User] ([Id]),
);