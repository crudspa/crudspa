create table [Samples].[ComposerContact] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ContactId] uniqueidentifier not null,
    [UserId] uniqueidentifier null,
    constraint [PK_Samples_ComposerContact] primary key clustered ([Id]),
    constraint [FK_Samples_ComposerContact_Contact] foreign key ([ContactId]) references [Framework].[Contact] ([Id]),
    constraint [FK_Samples_ComposerContact_User] foreign key ([UserId]) references [Framework].[User] ([Id]),
);