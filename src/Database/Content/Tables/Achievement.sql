create table [Content].[Achievement] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PortalId] uniqueidentifier not null,
    [Title] nvarchar(75) not null,
    [Description] nvarchar(max) null,
    [ImageId] uniqueidentifier not null,
    constraint [PK_Content_Achievement] primary key clustered ([Id]),
    constraint [FK_Content_Achievement_Portal] foreign key ([PortalId]) references [Framework].[Portal] ([Id]),
    constraint [FK_Content_Achievement_Image] foreign key ([ImageId]) references [Framework].[ImageFile] ([Id]),
);