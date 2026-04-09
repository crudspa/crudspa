create table [Education].[MetalinguisticCategory] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(50) not null,
    [Key] nvarchar(50) not null,
    constraint [PK_Education_MetalinguisticCategory] primary key clustered ([Id]),
);