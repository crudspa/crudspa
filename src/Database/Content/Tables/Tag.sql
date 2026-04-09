create table [Content].[Tag] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Title] nvarchar(150) not null,
    constraint [PK_Content_Tag] primary key clustered ([Id]),
);