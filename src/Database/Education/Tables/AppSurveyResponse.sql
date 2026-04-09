create table [Education].[AppSurveyResponse] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [AssignmentBatchId] uniqueidentifier not null,
    [QuestionId] uniqueidentifier not null,
    [AnswerId] uniqueidentifier not null,
    constraint [PK_Education_AppSurveyResponse] primary key clustered ([Id]),
);