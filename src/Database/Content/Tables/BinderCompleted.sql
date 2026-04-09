create table [Content].[BinderCompleted] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ContactId] uniqueidentifier not null,
    [BinderId] uniqueidentifier not null,
    [DeviceTimestamp] datetimeoffset(7) not null,
    constraint [PK_Content_BinderCompleted] primary key clustered ([Id]),
    constraint [FK_Content_BinderCompleted_Contact] foreign key ([ContactId]) references [Framework].[Contact] ([Id]),
    constraint [FK_Content_BinderCompleted_Binder] foreign key ([BinderId]) references [Content].[Binder] ([Id]),
);