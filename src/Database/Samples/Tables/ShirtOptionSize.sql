create table [Samples].[ShirtOptionSize] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ShirtOptionId] uniqueidentifier not null,
    [SizeId] uniqueidentifier not null,
    constraint [PK_Samples_ShirtOptionSize] primary key clustered ([Id]),
    constraint [FK_Samples_ShirtOptionSize_ShirtOption] foreign key ([ShirtOptionId]) references [Samples].[ShirtOption] ([Id]),
    constraint [FK_Samples_ShirtOptionSize_Size] foreign key ([SizeId]) references [Samples].[Size] ([Id]),
);