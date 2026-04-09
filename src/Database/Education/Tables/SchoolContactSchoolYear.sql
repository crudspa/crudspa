create table [Education].[SchoolContactSchoolYear] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [SchoolContactId] uniqueidentifier not null,
    [SchoolYearId] uniqueidentifier not null,
    constraint [PK_Education_SchoolContactSchoolYear] primary key clustered ([Id]),
    constraint [FK_Education_SchoolContactSchoolYear_SchoolContact] foreign key ([SchoolContactId]) references [Education].[SchoolContact] ([Id]),
    constraint [FK_Education_SchoolContactSchoolYear_SchoolYear] foreign key ([SchoolYearId]) references [Education].[SchoolYear] ([Id]),
);