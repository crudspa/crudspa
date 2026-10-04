create table [Content].[PublicationGroup] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PortalId] uniqueidentifier not null,
    [Name] nvarchar(150) not null,
    [Ordinal] int not null,
    constraint [PK_Content_PublicationGroup] primary key clustered ([Id]),
    constraint [FK_Content_PublicationGroup_Portal] foreign key ([PortalId]) references [Framework].[Portal] ([Id]),
);