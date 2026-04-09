create table [Education].[AssignmentBatch] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [BookId] uniqueidentifier null,
    [GameId] uniqueidentifier null,
    [StudentId] uniqueidentifier not null,
    [Published] datetimeoffset(7) not null,
    constraint [PK_Education_AssignmentBatch] primary key clustered ([Id]),
    constraint [FK_Education_AssignmentBatch_Book] foreign key ([BookId]) references [Education].[Book] ([Id]),
    constraint [FK_Education_AssignmentBatch_Student] foreign key ([StudentId]) references [Education].[Student] ([Id]),
);