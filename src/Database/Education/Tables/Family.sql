create table [Education].[Family] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [OrganizationId] uniqueidentifier not null,
    [SchoolId] uniqueidentifier not null,
    [ImportNum] int null,
    [SurveyComplete] bit default(0) null,
    constraint [PK_Education_Family] primary key clustered ([Id]),
    constraint [FK_Education_Family_Organization] foreign key ([OrganizationId]) references [Framework].[Organization] ([Id]),
    constraint [FK_Education_Family_School] foreign key ([SchoolId]) references [Education].[School] ([Id]),
);