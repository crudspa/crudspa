create table [Education].[BookGrade] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [BookId] uniqueidentifier not null,
    [GradeId] uniqueidentifier not null,
    constraint [PK_Education_BookGrade] primary key clustered ([Id]),
    constraint [FK_Education_BookGrade_Book] foreign key ([BookId]) references [Education].[Book] ([Id]),
    constraint [FK_Education_BookGrade_Grade] foreign key ([GradeId]) references [Education].[Grade] ([Id]),
);