create table [Content].[AudioElement] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ElementId] uniqueidentifier not null,
    [FileId] uniqueidentifier not null,
    constraint [PK_Content_AudioElement] primary key clustered ([Id]),
    constraint [FK_Content_AudioElement_Element] foreign key ([ElementId]) references [Content].[Element] ([Id]),
    constraint [FK_Content_AudioElement_File] foreign key ([FileId]) references [Framework].[AudioFile] ([Id]),
);