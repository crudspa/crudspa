create table [Content].[Member] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [MembershipId] uniqueidentifier not null,
    [ContactId] uniqueidentifier not null,
    [Status] int default(0) not null,
    constraint [PK_Content_Member] primary key clustered ([Id]),
    constraint [FK_Content_Member_Membership] foreign key ([MembershipId]) references [Content].[Membership] ([Id]),
    constraint [FK_Content_Member_Contact] foreign key ([ContactId]) references [Framework].[Contact] ([Id]),
);