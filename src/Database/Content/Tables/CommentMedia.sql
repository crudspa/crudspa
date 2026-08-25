create table [Content].[CommentMedia] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [CommentId] uniqueidentifier not null,
    [Type] int default(0) not null,
    [AudioId] uniqueidentifier null,
    [ImageId] uniqueidentifier null,
    [PdfId] uniqueidentifier null,
    [VideoId] uniqueidentifier null,
    [Ordinal] int not null,
    constraint [PK_Content_CommentMedia] primary key clustered ([Id]),
    constraint [FK_Content_CommentMedia_Comment] foreign key ([CommentId]) references [Content].[Comment] ([Id]),
    constraint [FK_Content_CommentMedia_Audio] foreign key ([AudioId]) references [Framework].[AudioFile] ([Id]),
    constraint [FK_Content_CommentMedia_Image] foreign key ([ImageId]) references [Framework].[ImageFile] ([Id]),
    constraint [FK_Content_CommentMedia_Pdf] foreign key ([PdfId]) references [Framework].[PdfFile] ([Id]),
    constraint [FK_Content_CommentMedia_Video] foreign key ([VideoId]) references [Framework].[VideoFile] ([Id]),
    constraint [CK_Content_CommentMedia_TypeFile] check (
        ([Type] = 0 and [AudioId] is not null and [ImageId] is null and [PdfId] is null and [VideoId] is null)
        or ([Type] = 1 and [AudioId] is null and [ImageId] is not null and [PdfId] is null and [VideoId] is null)
        or ([Type] = 2 and [AudioId] is null and [ImageId] is null and [PdfId] is not null and [VideoId] is null)
        or ([Type] = 3 and [AudioId] is null and [ImageId] is null and [PdfId] is null and [VideoId] is not null)
    ),
);

go

create nonclustered index [IX_Content_CommentMedia_Comment_Ordinal]
    on [Content].[CommentMedia] ([CommentId], [Ordinal])
    include ([Id], [VersionOf], [IsDeleted], [Type], [AudioId], [ImageId], [PdfId], [VideoId]);

go

create nonclustered index [IX_Content_CommentMedia_Audio]
    on [Content].[CommentMedia] ([AudioId])
    include ([CommentId])
    where [AudioId] is not null;

go

create nonclustered index [IX_Content_CommentMedia_Image]
    on [Content].[CommentMedia] ([ImageId])
    include ([CommentId])
    where [ImageId] is not null;

go

create nonclustered index [IX_Content_CommentMedia_Pdf]
    on [Content].[CommentMedia] ([PdfId])
    include ([CommentId])
    where [PdfId] is not null;

go

create nonclustered index [IX_Content_CommentMedia_Video]
    on [Content].[CommentMedia] ([VideoId])
    include ([CommentId])
    where [VideoId] is not null;