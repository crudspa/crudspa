create table [Framework].[ContactEmail] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ContactId] uniqueidentifier not null,
    [Email] nvarchar(75) not null,
    [Ordinal] int not null,
    constraint [PK_Framework_ContactEmail] primary key clustered ([Id]),
    constraint [FK_Framework_ContactEmail_Contact] foreign key ([ContactId]) references [Framework].[Contact] ([Id]),
);