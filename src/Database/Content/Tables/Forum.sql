create table [Content].[Forum] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PortalId] uniqueidentifier not null,
    [StatusId] uniqueidentifier not null,
    [PermissionId] uniqueidentifier null,
    [Title] nvarchar(150) not null,
    [Description] nvarchar(max) not null,
    [ImageId] uniqueidentifier null,
    [AccessMode] int default(1) not null,
    [Ordinal] int not null,
    constraint [PK_Content_Forum] primary key clustered ([Id]),
    constraint [FK_Content_Forum_Portal] foreign key ([PortalId]) references [Framework].[Portal] ([Id]),
    constraint [FK_Content_Forum_Status] foreign key ([StatusId]) references [Framework].[ContentStatus] ([Id]),
    constraint [FK_Content_Forum_Permission] foreign key ([PermissionId]) references [Framework].[Permission] ([Id]),
    constraint [FK_Content_Forum_Image] foreign key ([ImageId]) references [Framework].[ImageFile] ([Id]),
    constraint [CK_Content_Forum_AccessMode] check ([AccessMode] in (0, 1)),
);

go

create nonclustered index [IX_Content_Forum_Portal_Status_Ordinal]
    on [Content].[Forum] ([PortalId], [StatusId], [Ordinal])
    include ([Id], [VersionOf], [IsDeleted], [PermissionId], [Title], [ImageId], [AccessMode]);

go

create nonclustered index [IX_Content_Forum_Image]
    on [Content].[Forum] ([ImageId])
    include ([VersionOf])
    where [ImageId] is not null;