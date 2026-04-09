create table [Samples].[Tag] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(40) not null,
    [Ordinal] int not null,
    constraint [PK_Samples_Tag] primary key clustered ([Id]),
);