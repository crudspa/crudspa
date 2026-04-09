create table [Education].[ContentArea] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(50) not null,
    [Key] nvarchar(50) not null,
    [AppNavText] nvarchar(50) not null,
    [SuppressAudioChoices] bit default(0) not null,
    [MetalinguisticCategoryId] uniqueidentifier null,
    constraint [PK_Education_ContentArea] primary key clustered ([Id]),
    constraint [FK_Education_ContentArea_MetalinguisticCategory] foreign key ([MetalinguisticCategoryId]) references [Education].[MetalinguisticCategory] ([Id]),
);