create table [Content].[ForumUploadStage] (
    [BlobId] uniqueidentifier not null,
    [SessionId] uniqueidentifier not null,
    [UserId] uniqueidentifier not null,
    [ForumId] uniqueidentifier not null,
    [Type] int not null,
    [Name] nvarchar(150) not null,
    [Format] nvarchar(10) not null,
    [ContentType] nvarchar(100) not null,
    [Bytes] bigint not null,
    [Staged] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [Expires] datetimeoffset(7) not null,
    [Consumed] datetimeoffset(7) null,
    constraint [PK_Content_ForumUploadStage] primary key clustered ([BlobId]),
    constraint [FK_Content_ForumUploadStage_User] foreign key ([UserId]) references [Framework].[User] ([Id]),
    constraint [FK_Content_ForumUploadStage_Forum] foreign key ([ForumId]) references [Content].[Forum] ([Id]),
    constraint [CK_Content_ForumUploadStage_Type] check ([Type] between 0 and 3),
    constraint [CK_Content_ForumUploadStage_Bytes] check ([Bytes] > 0),
    constraint [CK_Content_ForumUploadStage_MaxBytes] check (
        ([Type] = 0 and [Bytes] <= 52428800)
        or ([Type] = 1 and [Bytes] <= 10485760)
        or ([Type] = 2 and [Bytes] <= 26214400)
        or ([Type] = 3 and [Bytes] <= 262144000)
    ),
);

go

create nonclustered index [IX_Content_ForumUploadStage_Expires]
    on [Content].[ForumUploadStage] ([Expires])
    include ([BlobId], [SessionId], [UserId], [ForumId], [Type], [Bytes]);

go

create nonclustered index [IX_Content_ForumUploadStage_User_Forum_Expires]
    on [Content].[ForumUploadStage] ([UserId], [ForumId], [Expires])
    include ([BlobId], [SessionId], [Type], [Bytes]);

go

create nonclustered index [IX_Content_ForumUploadStage_User_Forum_Staged]
    on [Content].[ForumUploadStage] ([UserId], [ForumId], [Staged])
    include ([Bytes], [Consumed], [Expires]);

go

create nonclustered index [IX_Content_ForumUploadStage_Consumed]
    on [Content].[ForumUploadStage] ([Consumed])
    include ([BlobId])
    where [Consumed] is not null;