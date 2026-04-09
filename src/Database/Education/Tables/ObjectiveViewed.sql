create table [Education].[ObjectiveViewed] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ObjectiveId] uniqueidentifier not null,
    constraint [PK_Education_ObjectiveViewed] primary key clustered ([Id]),
    constraint [FK_Education_ObjectiveViewed_Objective] foreign key ([ObjectiveId]) references [Education].[Objective] ([Id]),
);