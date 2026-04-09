create table [Samples].[Rating] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(40) not null,
    [Ordinal] int not null,
    constraint [PK_Samples_Rating] primary key clustered ([Id]),
);