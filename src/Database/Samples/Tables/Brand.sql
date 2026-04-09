create table [Samples].[Brand] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(60) not null,
    [Ordinal] int not null,
    constraint [PK_Samples_Brand] primary key clustered ([Id]),
);