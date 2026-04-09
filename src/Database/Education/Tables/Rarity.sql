create table [Education].[Rarity] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(50) not null,
    [Ordinal] int not null,
    constraint [PK_Education_Rarity] primary key clustered ([Id]),
);