create table [Content].[SurveyQuestion] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PartId] uniqueidentifier not null,
    [QuestionId] uniqueidentifier not null,
    [Ordinal] int not null,
    constraint [PK_Content_SurveyQuestion] primary key clustered ([Id]),
    constraint [FK_Content_SurveyQuestion_Part] foreign key ([PartId]) references [Content].[SurveyPart] ([Id]),
    constraint [FK_Content_SurveyQuestion_Question] foreign key ([QuestionId]) references [Content].[Question] ([Id]),
);