create table [Education].[PublisherContact] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PublisherId] uniqueidentifier not null,
    [ContactId] uniqueidentifier not null,
    [UserId] uniqueidentifier null,
    constraint [PK_Education_PublisherContact] primary key clustered ([Id]),
    constraint [FK_Education_PublisherContact_Publisher] foreign key ([PublisherId]) references [Education].[Publisher] ([Id]),
    constraint [FK_Education_PublisherContact_Contact] foreign key ([ContactId]) references [Framework].[Contact] ([Id]),
    constraint [FK_Education_PublisherContact_User] foreign key ([UserId]) references [Framework].[User] ([Id]),
);