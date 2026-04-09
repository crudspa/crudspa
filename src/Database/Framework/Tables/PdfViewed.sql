create table [Framework].[PdfViewed] (
    [Id] uniqueidentifier not null,
    [PdfFileId] uniqueidentifier not null,
    [Viewed] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [ViewedBy] uniqueidentifier not null,
    constraint [PK_Framework_PdfViewed] primary key clustered ([Id]),
    constraint [FK_Framework_PdfViewed_PdfFile] foreign key ([PdfFileId]) references [Framework].[PdfFile] ([Id]),
);