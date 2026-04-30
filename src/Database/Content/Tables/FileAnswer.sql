create table [Content].[FileAnswer] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [QuestionId] uniqueidentifier not null,
    [Kind] int default(0) not null,
    constraint [PK_Content_FileAnswer] primary key clustered ([Id]),
    constraint [FK_Content_FileAnswer_Question] foreign key ([QuestionId]) references [Content].[Question] ([Id]),
);