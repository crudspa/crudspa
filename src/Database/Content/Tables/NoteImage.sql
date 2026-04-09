create table [Content].[NoteImage] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [NoteId] uniqueidentifier not null,
    [ImageFileId] uniqueidentifier null,
    [Ordinal] int not null,
    constraint [PK_Content_NoteImage] primary key clustered ([Id]),
    constraint [FK_Content_NoteImage_Note] foreign key ([NoteId]) references [Content].[NoteElement] ([Id]),
    constraint [FK_Content_NoteImage_ImageFile] foreign key ([ImageFileId]) references [Framework].[ImageFile] ([Id]),
);