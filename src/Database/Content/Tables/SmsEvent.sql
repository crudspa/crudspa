create table [Content].[SmsEvent] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [SmsMessageId] uniqueidentifier null,
    [SmsChannelKey] nvarchar(50) null,
    [Provider] int default(0) not null,
    [Type] int default(0) not null,
    [IdempotencyKey] nvarchar(150) not null,
    [ProviderMessageId] nvarchar(75) null,
    [ProviderStatus] nvarchar(75) null,
    [RequestUrl] nvarchar(500) null,
    [RequestSignature] nvarchar(500) null,
    [SignatureValid] bit default(1) not null,
    [Received] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [Processed] datetimeoffset(7) null,
    [Status] int default(0) not null,
    [Payload] nvarchar(max) not null,
    [ErrorMessage] nvarchar(max) null,
    constraint [PK_Content_SmsEvent] primary key clustered ([Id]),
    constraint [FK_Content_SmsEvent_SmsMessage] foreign key ([SmsMessageId]) references [Content].[SmsMessage] ([Id]),
);

go

create unique nonclustered index [UX_Content_SmsEvent_Provider_IdempotencyKey]
on [Content].[SmsEvent] ([Provider], [IdempotencyKey]);