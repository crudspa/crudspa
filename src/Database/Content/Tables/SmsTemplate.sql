create table [Content].[SmsTemplate] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [MembershipId] uniqueidentifier not null,
    [Title] nvarchar(75) not null,
    [Body] nvarchar(max) not null,
    constraint [PK_Content_SmsTemplate] primary key clustered ([Id]),
    constraint [FK_Content_SmsTemplate_Membership] foreign key ([MembershipId]) references [Content].[Membership] ([Id]),
);