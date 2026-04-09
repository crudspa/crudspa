create table [Samples].[Genre] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(80) not null,
    [Ordinal] int not null,
    constraint [PK_Samples_Genre] primary key clustered ([Id]),
);