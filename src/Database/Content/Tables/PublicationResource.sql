create table [Content].[PublicationResource] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PublicationId] uniqueidentifier not null,
    [TypeId] uniqueidentifier not null,
    [Label] nvarchar(150) null,
    [Url] nvarchar(2000) null,
    [PdfId] uniqueidentifier null,
    [ImageId] uniqueidentifier null,
    [Ordinal] int not null,
    constraint [CK_Content_PublicationResource_Destination] check (
        (case when nullif(ltrim(rtrim(Url)), N'') is not null then 1 else 0 end)
        + (case when PdfId is not null then 1 else 0 end)
        + (case when ImageId is not null then 1 else 0 end) = 1),
    constraint [CK_Content_PublicationResource_Url] check (Url is null or Url like N'https://_%' or Url like N'http://_%'),
    constraint [PK_Content_PublicationResource] primary key clustered ([Id]),
    constraint [FK_Content_PublicationResource_Publication] foreign key ([PublicationId]) references [Content].[Publication] ([Id]),
    constraint [FK_Content_PublicationResource_Type] foreign key ([TypeId]) references [Content].[PublicationResourceType] ([Id]),
    constraint [FK_Content_PublicationResource_Pdf] foreign key ([PdfId]) references [Framework].[PdfFile] ([Id]),
    constraint [FK_Content_PublicationResource_Image] foreign key ([ImageId]) references [Framework].[ImageFile] ([Id]),
);