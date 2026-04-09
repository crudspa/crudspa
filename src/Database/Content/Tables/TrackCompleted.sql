create table [Content].[TrackCompleted] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ContactId] uniqueidentifier not null,
    [TrackId] uniqueidentifier not null,
    [Completed] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    constraint [PK_Content_TrackCompleted] primary key clustered ([Id]),
    constraint [FK_Content_TrackCompleted_Contact] foreign key ([ContactId]) references [Framework].[Contact] ([Id]),
    constraint [FK_Content_TrackCompleted_Track] foreign key ([TrackId]) references [Content].[Track] ([Id]),
);