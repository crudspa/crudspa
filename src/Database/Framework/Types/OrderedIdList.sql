create type [Framework].[OrderedIdList] as table (
    [Id] uniqueidentifier not null,
    [Ordinal] int not null
);