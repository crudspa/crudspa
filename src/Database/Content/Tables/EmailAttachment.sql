create table [Content].[EmailAttachment] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [EmailId] uniqueidentifier not null,
    [PdfId] uniqueidentifier not null,
    [Ordinal] int not null,
    constraint [PK_Content_EmailAttachment] primary key clustered ([Id]),
    constraint [FK_Content_EmailAttachment_Email] foreign key ([EmailId]) references [Content].[Email] ([Id]),
    constraint [FK_Content_EmailAttachment_Pdf] foreign key ([PdfId]) references [Framework].[PdfFile] ([Id]),
);