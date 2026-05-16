create table [Content].[SmsMessage] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [SmsId] uniqueidentifier null,
    [SmsChannelKey] nvarchar(50) null,
    [MembershipId] uniqueidentifier null,
    [MemberId] uniqueidentifier null,
    [ContactId] uniqueidentifier null,
    [ContactPhoneId] uniqueidentifier null,
    [Direction] int default(0) not null,
    [Body] nvarchar(max) null,
    [FromNumber] nvarchar(20) not null,
    [ToNumber] nvarchar(20) not null,
    [Occurred] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [Status] int default(0) not null,
    [StatusUpdated] datetimeoffset(7) null,
    [Provider] int default(0) not null,
    [ProviderMessageId] nvarchar(75) null,
    [ProviderStatus] nvarchar(75) null,
    [ProviderErrorCode] nvarchar(25) null,
    [ProviderErrorMessage] nvarchar(500) null,
    [SegmentCount] int null,
    [ApiResponse] nvarchar(max) null,
    constraint [PK_Content_SmsMessage] primary key clustered ([Id]),
    constraint [FK_Content_SmsMessage_Sms] foreign key ([SmsId]) references [Content].[Sms] ([Id]),
    constraint [FK_Content_SmsMessage_Membership] foreign key ([MembershipId]) references [Content].[Membership] ([Id]),
    constraint [FK_Content_SmsMessage_Member] foreign key ([MemberId]) references [Content].[Member] ([Id]),
    constraint [FK_Content_SmsMessage_Contact] foreign key ([ContactId]) references [Framework].[Contact] ([Id]),
    constraint [FK_Content_SmsMessage_ContactPhone] foreign key ([ContactPhoneId]) references [Framework].[ContactPhone] ([Id]),
);

go

create nonclustered index [IX_Content_SmsMessage_Provider_Channel_Message]
on [Content].[SmsMessage] ([Provider], [SmsChannelKey], [ProviderMessageId])
where [ProviderMessageId] is not null;