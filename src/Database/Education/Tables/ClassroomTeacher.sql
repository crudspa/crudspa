create table [Education].[ClassroomTeacher] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ClassroomId] uniqueidentifier not null,
    [SchoolContactId] uniqueidentifier not null,
    constraint [PK_Education_ClassroomTeacher] primary key clustered ([Id]),
    constraint [FK_Education_ClassroomTeacher_Classroom] foreign key ([ClassroomId]) references [Education].[Classroom] ([Id]),
    constraint [FK_Education_ClassroomTeacher_SchoolContact] foreign key ([SchoolContactId]) references [Education].[SchoolContact] ([Id]),
);