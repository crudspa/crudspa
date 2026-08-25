create type [ContentMessaging].[ScopedStageScheduleList] as table (
    [ScopeKey] uniqueidentifier not null,
    [StageId] uniqueidentifier not null,
    [Send] datetimeoffset(7) not null,
    [Overridden] bit not null,
    primary key ([ScopeKey], [StageId])
);