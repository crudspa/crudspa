create table [Content].[SmsAttachment] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [SmsId] uniqueidentifier not null,
    [ImageId] uniqueidentifier not null,
    [Ordinal] int not null,
    constraint [PK_Content_SmsAttachment] primary key clustered ([Id]),
    constraint [FK_Content_SmsAttachment_Sms] foreign key ([SmsId]) references [Content].[Sms] ([Id]),
    constraint [FK_Content_SmsAttachment_Image] foreign key ([ImageId]) references [Framework].[ImageFile] ([Id]),
);