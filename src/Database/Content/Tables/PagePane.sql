create table [Content].[PagePane] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PaneId] uniqueidentifier not null,
    [PageId] uniqueidentifier not null,
    constraint [PK_Content_PagePane] primary key clustered ([Id]),
    constraint [FK_Content_PagePane_Pane] foreign key ([PaneId]) references [Framework].[Pane] ([Id]),
    constraint [FK_Content_PagePane_Page] foreign key ([PageId]) references [Content].[Page] ([Id]),
);