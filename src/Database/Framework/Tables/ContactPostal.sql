create table [Framework].[ContactPostal] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ContactId] uniqueidentifier not null,
    [PostalId] uniqueidentifier not null,
    [Ordinal] int not null,
    constraint [PK_Framework_ContactPostal] primary key clustered ([Id]),
    constraint [FK_Framework_ContactPostal_Contact] foreign key ([ContactId]) references [Framework].[Contact] ([Id]),
    constraint [FK_Framework_ContactPostal_Postal] foreign key ([PostalId]) references [Framework].[UsaPostal] ([Id]),
);