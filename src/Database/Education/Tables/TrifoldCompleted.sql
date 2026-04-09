create table [Education].[TrifoldCompleted] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [StudentId] uniqueidentifier not null,
    [TrifoldId] uniqueidentifier not null,
    [DeviceTimestamp] datetimeoffset(7) not null,
    constraint [PK_Education_TrifoldCompleted] primary key clustered ([Id]),
    constraint [FK_Education_TrifoldCompleted_Student] foreign key ([StudentId]) references [Education].[Student] ([Id]),
    constraint [FK_Education_TrifoldCompleted_Trifold] foreign key ([TrifoldId]) references [Education].[Trifold] ([Id]),
);