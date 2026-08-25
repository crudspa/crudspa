create type [ContentMessaging].[PopulationTokenValueList] as table (
    [ContactId] uniqueidentifier not null,
    [Key] nvarchar(75) not null,
    [Value] nvarchar(max) not null,
    primary key ([ContactId], [Key])
);