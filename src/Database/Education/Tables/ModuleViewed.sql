create table [Education].[ModuleViewed] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ModuleId] uniqueidentifier not null,
    constraint [PK_Education_ModuleViewed] primary key clustered ([Id]),
    constraint [FK_Education_ModuleViewed_Module] foreign key ([ModuleId]) references [Education].[Module] ([Id]),
);