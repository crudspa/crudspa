create table [Education].[ClassroomStudent] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ClassroomId] uniqueidentifier not null,
    [StudentId] uniqueidentifier not null,
    constraint [PK_Education_ClassroomStudent] primary key clustered ([Id]),
    constraint [FK_Education_ClassroomStudent_Classroom] foreign key ([ClassroomId]) references [Education].[Classroom] ([Id]),
    constraint [FK_Education_ClassroomStudent_Student] foreign key ([StudentId]) references [Education].[Student] ([Id]),
);