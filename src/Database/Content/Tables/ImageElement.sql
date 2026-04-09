create table [Content].[ImageElement] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ElementId] uniqueidentifier not null,
    [FileId] uniqueidentifier not null,
    [HyperlinkUrl] nvarchar(max) null,
    constraint [PK_Content_ImageElement] primary key clustered ([Id]),
    constraint [FK_Content_ImageElement_Element] foreign key ([ElementId]) references [Content].[Element] ([Id]),
    constraint [FK_Content_ImageElement_File] foreign key ([FileId]) references [Framework].[ImageFile] ([Id]),
);