create table [Framework].[AudioFile] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [BlobId] uniqueidentifier not null,
    [Name] nvarchar(150) not null,
    [Format] nvarchar(10) not null,
    [OptimizedStatus] int default(0) not null,
    [OptimizedBlobId] uniqueidentifier null,
    [OptimizedFormat] nvarchar(10) null,
    [OptimizedBatchId] uniqueidentifier null,
    constraint [PK_Framework_AudioFile] primary key clustered ([Id]),
);