create table [Content].[EmailTemplate] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [MembershipId] uniqueidentifier not null,
    [Title] nvarchar(75) not null,
    [Subject] nvarchar(150) not null,
    [Body] nvarchar(max) not null,
    constraint [PK_Content_EmailTemplate] primary key clustered ([Id]),
    constraint [FK_Content_EmailTemplate_Membership] foreign key ([MembershipId]) references [Content].[Membership] ([Id]),
);