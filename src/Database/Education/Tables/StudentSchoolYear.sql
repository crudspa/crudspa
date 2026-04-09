create table [Education].[StudentSchoolYear] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [StudentId] uniqueidentifier not null,
    [SchoolYearId] uniqueidentifier not null,
    constraint [PK_Education_StudentSchoolYear] primary key clustered ([Id]),
    constraint [FK_Education_StudentSchoolYear_Student] foreign key ([StudentId]) references [Education].[Student] ([Id]),
    constraint [FK_Education_StudentSchoolYear_SchoolYear] foreign key ([SchoolYearId]) references [Education].[SchoolYear] ([Id]),
);