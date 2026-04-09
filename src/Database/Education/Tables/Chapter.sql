create table [Education].[Chapter] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Title] nvarchar(75) not null,
    [BookId] uniqueidentifier not null,
    [BinderId] uniqueidentifier not null,
    [Ordinal] int not null,
    constraint [PK_Education_Chapter] primary key clustered ([Id]),
    constraint [FK_Education_Chapter_Book] foreign key ([BookId]) references [Education].[Book] ([Id]),
    constraint [FK_Education_Chapter_Binder] foreign key ([BinderId]) references [Content].[Binder] ([Id]),
);