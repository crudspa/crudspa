create table [Education].[StudentBook] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [StudentId] uniqueidentifier not null,
    [BookId] uniqueidentifier not null,
    [SchoolYearId] uniqueidentifier not null,
    constraint [PK_Education_StudentBook] primary key clustered ([Id]),
    constraint [FK_Education_StudentBook_Student] foreign key ([StudentId]) references [Education].[Student] ([Id]),
    constraint [FK_Education_StudentBook_Book] foreign key ([BookId]) references [Education].[Book] ([Id]),
    constraint [FK_Education_StudentBook_SchoolYear] foreign key ([SchoolYearId]) references [Education].[SchoolYear] ([Id]),
);