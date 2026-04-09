create table [Education].[GameViewed] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [GameId] uniqueidentifier not null,
    constraint [PK_Education_GameViewed] primary key clustered ([Id]),
    constraint [FK_Education_GameViewed_Game] foreign key ([GameId]) references [Education].[Game] ([Id]),
);