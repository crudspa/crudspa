create table [Content].[SurveyPart] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [SurveyId] uniqueidentifier not null,
    [Title] nvarchar(75) not null,
    [Instructions] nvarchar(max) null,
    [Ordinal] int not null,
    constraint [PK_Content_SurveyPart] primary key clustered ([Id]),
    constraint [FK_Content_SurveyPart_Survey] foreign key ([SurveyId]) references [Content].[Survey] ([Id]),
);