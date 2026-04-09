create table [Content].[EmailLog] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [EmailId] uniqueidentifier not null,
    [RecipientId] uniqueidentifier not null,
    [RecipientEmail] nvarchar(75) not null,
    [Processed] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [Status] int default(0) not null,
    [ApiResponse] nvarchar(max) null,
    constraint [PK_Content_EmailLog] primary key clustered ([Id]),
    constraint [FK_Content_EmailLog_Email] foreign key ([EmailId]) references [Content].[Email] ([Id]),
    constraint [FK_Content_EmailLog_Recipient] foreign key ([RecipientId]) references [Framework].[Contact] ([Id]),
);