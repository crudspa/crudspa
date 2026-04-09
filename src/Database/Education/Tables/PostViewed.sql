create table [Education].[PostViewed] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PostId] uniqueidentifier not null,
    constraint [PK_Education_PostViewed] primary key clustered ([Id]),
    constraint [FK_Education_PostViewed_Post] foreign key ([PostId]) references [Education].[Post] ([Id]),
);