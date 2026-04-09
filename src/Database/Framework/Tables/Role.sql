create table [Framework].[Role] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(50) not null,
    [OrganizationId] uniqueidentifier not null,
    constraint [PK_Framework_Role] primary key clustered ([Id]),
    constraint [FK_Framework_Role_Organization] foreign key ([OrganizationId]) references [Framework].[Organization] ([Id]),
);