create table [Content].[MultimediaElement] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ElementId] uniqueidentifier not null,
    [ContainerId] uniqueidentifier not null,
    constraint [PK_Content_MultimediaElement] primary key clustered ([Id]),
    constraint [FK_Content_MultimediaElement_Element] foreign key ([ElementId]) references [Content].[Element] ([Id]),
    constraint [FK_Content_MultimediaElement_Container] foreign key ([ContainerId]) references [Content].[Container] ([Id]),
);