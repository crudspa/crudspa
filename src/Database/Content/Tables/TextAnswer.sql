create table [Content].[TextAnswer] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [QuestionId] uniqueidentifier not null,
    [Kind] int default(0) not null,
    [Label] nvarchar(150) null,
    [Placeholder] nvarchar(150) null,
    constraint [PK_Content_TextAnswer] primary key clustered ([Id]),
    constraint [FK_Content_TextAnswer_Question] foreign key ([QuestionId]) references [Content].[Question] ([Id]),
);