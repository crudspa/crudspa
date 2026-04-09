create table [Education].[MapViewed] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [BookId] uniqueidentifier not null,
    constraint [PK_Education_MapViewed] primary key clustered ([Id]),
    constraint [FK_Education_MapViewed_Book] foreign key ([BookId]) references [Education].[Book] ([Id]),
);