create table [Content].[ElementCompleted] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ContactId] uniqueidentifier not null,
    [ElementId] uniqueidentifier not null,
    [DeviceTimestamp] datetimeoffset(7) not null,
    constraint [PK_Content_ElementCompleted] primary key clustered ([Id]),
    constraint [FK_Content_ElementCompleted_Contact] foreign key ([ContactId]) references [Framework].[Contact] ([Id]),
    constraint [FK_Content_ElementCompleted_Element] foreign key ([ElementId]) references [Content].[Element] ([Id]),
);