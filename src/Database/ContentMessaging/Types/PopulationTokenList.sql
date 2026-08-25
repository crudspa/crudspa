create type [ContentMessaging].[PopulationTokenList] as table (
    [Key] nvarchar(75) not null primary key,
    [Description] nvarchar(150) null,
    [Ordinal] int not null
);