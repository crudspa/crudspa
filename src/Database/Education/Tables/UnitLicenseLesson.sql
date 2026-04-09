create table [Education].[UnitLicenseLesson] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [UnitLicenseId] uniqueidentifier not null,
    [LessonId] uniqueidentifier not null,
    constraint [PK_Education_UnitLicenseLesson] primary key clustered ([Id]),
    constraint [FK_Education_UnitLicenseLesson_UnitLicense] foreign key ([UnitLicenseId]) references [Education].[UnitLicense] ([Id]),
    constraint [FK_Education_UnitLicenseLesson_Lesson] foreign key ([LessonId]) references [Education].[Lesson] ([Id]),
);