create table [Framework].[LinkFollowed] (
    [Id] uniqueidentifier not null,
    [Url] nvarchar(250) not null,
    [Followed] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [FollowedBy] uniqueidentifier not null,
    constraint [PK_Framework_LinkFollowed] primary key clustered ([Id]),
);