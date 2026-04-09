create table [Education].[ReadTextEntry] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [AssignmentId] uniqueidentifier not null,
    [QuestionId] uniqueidentifier not null,
    [Text] nvarchar(max) not null,
    constraint [PK_Education_ReadTextEntry] primary key clustered ([Id]),
    constraint [FK_Education_ReadTextEntry_Assignment] foreign key ([AssignmentId]) references [Education].[AssessmentAssignment] ([Id]),
    constraint [FK_Education_ReadTextEntry_Question] foreign key ([QuestionId]) references [Education].[ReadQuestion] ([Id]),
);