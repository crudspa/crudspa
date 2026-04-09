create table [Framework].[License] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [OwnerId] uniqueidentifier not null,
    [Name] nvarchar(50) not null,
    [Description] nvarchar(max) null,
    constraint [PK_Framework_License] primary key clustered ([Id]),
    constraint [FK_Framework_License_Owner] foreign key ([OwnerId]) references [Framework].[Organization] ([Id]),
);