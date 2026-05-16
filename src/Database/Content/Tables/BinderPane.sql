create table [Content].[BinderPane] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PaneId] uniqueidentifier not null,
    [BinderId] uniqueidentifier not null,
    constraint [PK_Content_BinderPane] primary key clustered ([Id]),
    constraint [FK_Content_BinderPane_Pane] foreign key ([PaneId]) references [Framework].[Pane] ([Id]),
    constraint [FK_Content_BinderPane_Binder] foreign key ([BinderId]) references [Content].[Binder] ([Id]),
);