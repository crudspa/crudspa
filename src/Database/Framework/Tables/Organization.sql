create table [Framework].[Organization] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(75) not null,
    [TimeZoneId] nvarchar(32) not null,
    constraint [PK_Framework_Organization] primary key clustered ([Id]),
);