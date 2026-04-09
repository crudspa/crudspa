create table [Content].[Style] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ContentPortalId] uniqueidentifier not null,
    [RuleId] uniqueidentifier not null,
    [ConfigJson] nvarchar(max) not null,
    constraint [PK_Content_Style] primary key clustered ([Id]),
    constraint [FK_Content_Style_ContentPortal] foreign key ([ContentPortalId]) references [Content].[ContentPortal] ([Id]),
    constraint [FK_Content_Style_Rule] foreign key ([RuleId]) references [Content].[Rule] ([Id]),
);