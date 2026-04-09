create table [Education].[UnitViewed] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [UnitId] uniqueidentifier not null,
    constraint [PK_Education_UnitViewed] primary key clustered ([Id]),
    constraint [FK_Education_UnitViewed_Unit] foreign key ([UnitId]) references [Education].[Unit] ([Id]),
);