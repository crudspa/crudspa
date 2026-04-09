create table [Content].[ThreadTag] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ThreadId] uniqueidentifier not null,
    [TagId] uniqueidentifier not null,
    constraint [PK_Content_ThreadTag] primary key clustered ([Id]),
    constraint [FK_Content_ThreadTag_Thread] foreign key ([ThreadId]) references [Content].[Thread] ([Id]),
    constraint [FK_Content_ThreadTag_Tag] foreign key ([TagId]) references [Content].[Tag] ([Id]),
);