create table [Content].[SmsPreference] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PortalId] uniqueidentifier not null,
    [SmsChannelKey] nvarchar(50) null,
    [ContactId] uniqueidentifier null,
    [ContactPhoneId] uniqueidentifier null,
    [Number] nvarchar(20) not null,
    [Status] int default(0) not null,
    [Source] int default(0) not null,
    [StatusChanged] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [SmsMessageId] uniqueidentifier null,
    [Notes] nvarchar(max) null,
    constraint [PK_Content_SmsPreference] primary key clustered ([Id]),
    constraint [FK_Content_SmsPreference_Portal] foreign key ([PortalId]) references [Framework].[Portal] ([Id]),
    constraint [FK_Content_SmsPreference_Contact] foreign key ([ContactId]) references [Framework].[Contact] ([Id]),
    constraint [FK_Content_SmsPreference_ContactPhone] foreign key ([ContactPhoneId]) references [Framework].[ContactPhone] ([Id]),
    constraint [FK_Content_SmsPreference_SmsMessage] foreign key ([SmsMessageId]) references [Content].[SmsMessage] ([Id]),
);

go

create nonclustered index [IX_Content_SmsPreference_Portal_Channel_Status_Number]
on [Content].[SmsPreference] ([PortalId], [SmsChannelKey], [Status], [ContactPhoneId], [Number]);