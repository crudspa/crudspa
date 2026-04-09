create table [Framework].[TextFile] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [BlobId] uniqueidentifier not null,
    constraint [PK_Framework_TextFile] primary key clustered ([Id]),
);