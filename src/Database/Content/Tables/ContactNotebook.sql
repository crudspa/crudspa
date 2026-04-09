create table [Content].[ContactNotebook] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ContactId] uniqueidentifier not null,
    [NotebookId] uniqueidentifier not null,
    constraint [PK_Content_ContactNotebook] primary key clustered ([Id]),
    constraint [FK_Content_ContactNotebook_Contact] foreign key ([ContactId]) references [Framework].[Contact] ([Id]),
    constraint [FK_Content_ContactNotebook_Notebook] foreign key ([NotebookId]) references [Content].[Notebook] ([Id]),
);