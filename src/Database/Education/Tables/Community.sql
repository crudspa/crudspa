create table [Education].[Community] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [DistrictId] uniqueidentifier not null,
    [Name] nvarchar(100) not null,
    constraint [PK_Education_Community] primary key clustered ([Id]),
    constraint [FK_Education_Community_District] foreign key ([DistrictId]) references [Education].[District] ([Id]),
);