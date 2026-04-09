create table [Content].[Token] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [MembershipId] uniqueidentifier not null,
    [Key] nvarchar(75) not null,
    [Description] nvarchar(150) null,
    [Ordinal] int not null,
    constraint [PK_Content_Token] primary key clustered ([Id]),
    constraint [FK_Content_Token_Membership] foreign key ([MembershipId]) references [Content].[Membership] ([Id]),
);