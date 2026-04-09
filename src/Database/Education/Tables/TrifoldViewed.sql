create table [Education].[TrifoldViewed] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [TrifoldId] uniqueidentifier not null,
    constraint [PK_Education_TrifoldViewed] primary key clustered ([Id]),
    constraint [FK_Education_TrifoldViewed_Trifold] foreign key ([TrifoldId]) references [Education].[Trifold] ([Id]),
);