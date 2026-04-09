create table [Content].[Binder] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [TypeId] uniqueidentifier not null,
    constraint [PK_Content_Binder] primary key clustered ([Id]),
    constraint [FK_Content_Binder_Type] foreign key ([TypeId]) references [Content].[BinderType] ([Id]),
);