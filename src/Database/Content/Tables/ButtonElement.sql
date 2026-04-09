create table [Content].[ButtonElement] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ElementId] uniqueidentifier not null,
    [ButtonId] uniqueidentifier not null,
    constraint [PK_Content_ButtonElement] primary key clustered ([Id]),
    constraint [FK_Content_ButtonElement_Element] foreign key ([ElementId]) references [Content].[Element] ([Id]),
    constraint [FK_Content_ButtonElement_Button] foreign key ([ButtonId]) references [Content].[Button] ([Id]),
);