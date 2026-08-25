create type [ContentMessaging].[StageScheduleList] as table (
    [OrganizationId] uniqueidentifier not null,
    [StageId] uniqueidentifier not null,
    [Send] datetimeoffset(7) not null,
    primary key ([OrganizationId], [StageId])
);