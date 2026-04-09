create table [Education].[PrefaceCompleted] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [StudentId] uniqueidentifier not null,
    [BookId] uniqueidentifier not null,
    [DeviceTimestamp] datetimeoffset(7) not null,
    constraint [PK_Education_PrefaceCompleted] primary key clustered ([Id]),
    constraint [FK_Education_PrefaceCompleted_Student] foreign key ([StudentId]) references [Education].[Student] ([Id]),
    constraint [FK_Education_PrefaceCompleted_Book] foreign key ([BookId]) references [Education].[Book] ([Id]),
);