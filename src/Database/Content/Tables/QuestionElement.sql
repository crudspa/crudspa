create table [Content].[QuestionElement] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ElementId] uniqueidentifier not null,
    [QuestionId] uniqueidentifier not null,
    constraint [PK_Content_QuestionElement] primary key clustered ([Id]),
    constraint [FK_Content_QuestionElement_Element] foreign key ([ElementId]) references [Content].[Element] ([Id]),
    constraint [FK_Content_QuestionElement_Question] foreign key ([QuestionId]) references [Content].[Question] ([Id]),
);