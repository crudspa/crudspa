create table [Education].[VocabChoiceSelection] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [AssignmentId] uniqueidentifier not null,
    [ChoiceId] uniqueidentifier not null,
    [Made] datetimeoffset(7) not null,
    constraint [PK_Education_VocabChoiceSelection] primary key clustered ([Id]),
    constraint [FK_Education_VocabChoiceSelection_Assignment] foreign key ([AssignmentId]) references [Education].[AssessmentAssignment] ([Id]),
    constraint [FK_Education_VocabChoiceSelection_Choice] foreign key ([ChoiceId]) references [Education].[VocabChoice] ([Id]),
);