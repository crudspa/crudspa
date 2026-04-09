create table [Framework].[ContactPhone] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ContactId] uniqueidentifier not null,
    [Phone] nvarchar(10) not null,
    [Extension] nvarchar(10) null,
    [SupportsSms] bit default(1) not null,
    [Ordinal] int not null,
    constraint [PK_Framework_ContactPhone] primary key clustered ([Id]),
    constraint [FK_Framework_ContactPhone_Contact] foreign key ([ContactId]) references [Framework].[Contact] ([Id]),
);