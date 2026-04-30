create table [Content].[OptionsAnswerChoice] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [OptionsAnswerId] uniqueidentifier not null,
    [Text] nvarchar(max) not null,
    [Ordinal] int not null,
    constraint [PK_Content_OptionsAnswerChoice] primary key clustered ([Id]),
    constraint [FK_Content_OptionsAnswerChoice_OptionsAnswer] foreign key ([OptionsAnswerId]) references [Content].[OptionsAnswer] ([Id]),
);