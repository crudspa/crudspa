create table [Framework].[Contact] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [FirstName] nvarchar(75) null,
    [LastName] nvarchar(75) null,
    [TimeZoneId] nvarchar(32) not null,
    constraint [PK_Framework_Contact] primary key clustered ([Id]),
);