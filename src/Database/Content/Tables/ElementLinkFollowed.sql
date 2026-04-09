create table [Content].[ElementLinkFollowed] (
    [Id] uniqueidentifier not null,
    [ElementId] uniqueidentifier not null,
    [Url] nvarchar(250) not null,
    [Followed] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [FollowedBy] uniqueidentifier not null,
    constraint [PK_Content_ElementLinkFollowed] primary key clustered ([Id]),
    constraint [FK_Content_ElementLinkFollowed_Element] foreign key ([ElementId]) references [Content].[Element] ([Id]),
);