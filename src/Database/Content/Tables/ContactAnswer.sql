create table [Content].[ContactAnswer] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [QuestionId] uniqueidentifier not null,
    [Kind] int default(0) not null,
    [Label] nvarchar(150) null,
    constraint [PK_Content_ContactAnswer] primary key clustered ([Id]),
    constraint [FK_Content_ContactAnswer_Question] foreign key ([QuestionId]) references [Content].[Question] ([Id]),
);