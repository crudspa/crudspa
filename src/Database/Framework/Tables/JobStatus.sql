create table [Framework].[JobStatus] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(75) not null,
    [CssClass] nvarchar(50) not null,
    [Ordinal] int not null,
    constraint [PK_Framework_JobStatus] primary key clustered ([Id]),
);