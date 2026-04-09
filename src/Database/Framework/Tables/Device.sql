create table [Framework].[Device] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(75) not null,
    constraint [PK_Framework_Device] primary key clustered ([Id]),
);