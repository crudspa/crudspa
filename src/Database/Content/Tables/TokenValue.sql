create table [Content].[TokenValue] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [TokenId] uniqueidentifier not null,
    [ContactId] uniqueidentifier not null,
    [Value] nvarchar(max) not null,
    constraint [PK_Content_TokenValue] primary key clustered ([Id]),
    constraint [FK_Content_TokenValue_Token] foreign key ([TokenId]) references [Content].[Token] ([Id]),
    constraint [FK_Content_TokenValue_Contact] foreign key ([ContactId]) references [Framework].[Contact] ([Id]),
);