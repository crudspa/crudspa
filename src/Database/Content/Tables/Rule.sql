create table [Content].[Rule] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(75) not null,
    [Key] nvarchar(75) not null,
    [TypeId] uniqueidentifier not null,
    [DefaultValue] nvarchar(max) not null,
    constraint [PK_Content_Rule] primary key clustered ([Id]),
    constraint [FK_Content_Rule_Type] foreign key ([TypeId]) references [Content].[RuleType] ([Id]),
);