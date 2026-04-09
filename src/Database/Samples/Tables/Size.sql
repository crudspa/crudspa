create table [Samples].[Size] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ColorId] uniqueidentifier not null,
    [Name] nvarchar(20) not null,
    [Ordinal] int not null,
    constraint [PK_Samples_Size] primary key clustered ([Id]),
    constraint [FK_Samples_Size_Color] foreign key ([ColorId]) references [Samples].[Color] ([Id]),
);