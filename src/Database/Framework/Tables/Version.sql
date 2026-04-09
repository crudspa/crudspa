create table [Framework].[Version] (
    [Id] uniqueidentifier not null,
    [Created] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [Major] int not null,
    [Minor] int not null,
    [Build] int not null,
    [Revision] int not null,
    [Notes] nvarchar(max) null,
    constraint [PK_Framework_Version] primary key clustered ([Id]),
);