create table [Education].[BookActivity] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [BookId] uniqueidentifier not null,
    [ActivityId] uniqueidentifier not null,
    constraint [PK_Education_BookActivity] primary key clustered ([Id]),
    constraint [FK_Education_BookActivity_Book] foreign key ([BookId]) references [Education].[Book] ([Id]),
    constraint [FK_Education_BookActivity_Activity] foreign key ([ActivityId]) references [Education].[Activity] ([Id]),
);