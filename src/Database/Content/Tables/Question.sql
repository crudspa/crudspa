create table [Content].[Question] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Text] nvarchar(max) null,
    [AnswerTypeId] uniqueidentifier not null,
    constraint [PK_Content_Question] primary key clustered ([Id]),
    constraint [FK_Content_Question_AnswerType] foreign key ([AnswerTypeId]) references [Content].[AnswerType] ([Id]),
);