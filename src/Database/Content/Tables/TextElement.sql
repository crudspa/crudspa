create table [Content].[TextElement] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ElementId] uniqueidentifier not null,
    [Text] nvarchar(max) not null,
    constraint [PK_Content_TextElement] primary key clustered ([Id]),
    constraint [FK_Content_TextElement_Element] foreign key ([ElementId]) references [Content].[Element] ([Id]),
);