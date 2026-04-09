create table [Content].[TrackViewed] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [TrackId] uniqueidentifier not null,
    constraint [PK_Content_TrackViewed] primary key clustered ([Id]),
    constraint [FK_Content_TrackViewed_Track] foreign key ([TrackId]) references [Content].[Track] ([Id]),
);