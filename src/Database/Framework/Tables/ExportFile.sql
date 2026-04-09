create table [Framework].[ExportFile] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [BlobId] uniqueidentifier not null,
    [Name] nvarchar(150) not null,
    [Format] nvarchar(10) not null,
    [Description] nvarchar(max) null,
    [Published] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    constraint [PK_Framework_ExportFile] primary key clustered ([Id]),
);