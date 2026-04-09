create table [Education].[ContentViewed] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [BookId] uniqueidentifier not null,
    constraint [PK_Education_ContentViewed] primary key clustered ([Id]),
    constraint [FK_Education_ContentViewed_Book] foreign key ([BookId]) references [Education].[Book] ([Id]),
);