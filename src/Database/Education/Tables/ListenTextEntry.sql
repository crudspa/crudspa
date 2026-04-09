create table [Education].[ListenTextEntry] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [AssignmentId] uniqueidentifier not null,
    [QuestionId] uniqueidentifier not null,
    [Text] nvarchar(max) not null,
    constraint [PK_Education_ListenTextEntry] primary key clustered ([Id]),
    constraint [FK_Education_ListenTextEntry_Assignment] foreign key ([AssignmentId]) references [Education].[AssessmentAssignment] ([Id]),
    constraint [FK_Education_ListenTextEntry_Question] foreign key ([QuestionId]) references [Education].[ListenQuestion] ([Id]),
);