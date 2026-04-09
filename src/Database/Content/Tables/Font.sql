create table [Content].[Font] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(75) not null,
    [ContentPortalId] uniqueidentifier not null,
    [FileId] uniqueidentifier not null,
    constraint [PK_Content_Font] primary key clustered ([Id]),
    constraint [FK_Content_Font_ContentPortal] foreign key ([ContentPortalId]) references [Content].[ContentPortal] ([Id]),
    constraint [FK_Content_Font_File] foreign key ([FileId]) references [Framework].[FontFile] ([Id]),
);