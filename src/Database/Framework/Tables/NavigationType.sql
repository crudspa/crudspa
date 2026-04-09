create table [Framework].[NavigationType] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(75) not null,
    [DisplayView] nvarchar(250) not null,
    constraint [PK_Framework_NavigationType] primary key clustered ([Id]),
);