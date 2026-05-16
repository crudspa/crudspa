create table [Education].[GuideBinder] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [BinderId] uniqueidentifier not null,
    [GuideImageId] uniqueidentifier null,
    constraint [PK_Education_GuideBinder] primary key clustered ([Id]),
    constraint [FK_Education_GuideBinder_Binder] foreign key ([BinderId]) references [Content].[Binder] ([Id]),
    constraint [FK_Education_GuideBinder_GuideImage] foreign key ([GuideImageId]) references [Framework].[ImageFile] ([Id]),
);