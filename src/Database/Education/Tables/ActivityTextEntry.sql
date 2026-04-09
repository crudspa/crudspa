create table [Education].[ActivityTextEntry] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [AssignmentId] uniqueidentifier not null,
    [Text] nvarchar(max) not null,
    [Made] datetimeoffset(7) not null,
    [Ordinal] int not null,
    constraint [PK_Education_ActivityTextEntry] primary key clustered ([Id]),
    constraint [FK_Education_ActivityTextEntry_Assignment] foreign key ([AssignmentId]) references [Education].[ActivityAssignment] ([Id]),
);