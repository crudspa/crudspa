create table [Samples].[BookTag] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [BookId] uniqueidentifier not null,
    [TagId] uniqueidentifier not null,
    constraint [PK_Samples_BookTag] primary key clustered ([Id]),
    constraint [FK_Samples_BookTag_Book] foreign key ([BookId]) references [Samples].[Book] ([Id]),
    constraint [FK_Samples_BookTag_Tag] foreign key ([TagId]) references [Samples].[Tag] ([Id]),
);