create table [Content].[Sms] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [MembershipId] uniqueidentifier not null,
    [SmsChannelKey] nvarchar(50) null,
    [TemplateId] uniqueidentifier null,
    [Send] datetimeoffset(7) not null,
    [Body] nvarchar(max) not null,
    [Status] int default(0) not null,
    [Processed] datetimeoffset(7) null,
    [BatchId] uniqueidentifier null,
    constraint [PK_Content_Sms] primary key clustered ([Id]),
    constraint [FK_Content_Sms_Membership] foreign key ([MembershipId]) references [Content].[Membership] ([Id]),
    constraint [FK_Content_Sms_Template] foreign key ([TemplateId]) references [Content].[SmsTemplate] ([Id]),
);

go

create nonclustered index [IX_Content_Sms_SendQueue]
on [Content].[Sms] ([Status], [BatchId], [Send])
include ([MembershipId], [SmsChannelKey]);