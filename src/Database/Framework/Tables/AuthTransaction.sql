create table [Framework].[AuthTransaction] (
    [Id] uniqueidentifier not null,
    [Created] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [Expires] datetimeoffset(7) not null,
    [Provider] nvarchar(75) not null,
    [Audience] nvarchar(25) not null,
    [ReturnPath] nvarchar(500) not null,
    [Consumed] datetimeoffset(7) null,
    constraint [PK_Framework_AuthTransaction] primary key clustered ([Id]),
    constraint [CK_Framework_AuthTransaction_Provider] check (len([Provider]) > 0),
    constraint [CK_Framework_AuthTransaction_Audience] check ([Audience] in (N'auto', N'district', N'school', N'student')),
    constraint [CK_Framework_AuthTransaction_ReturnPath] check ([ReturnPath] like N'/%' and [ReturnPath] not like N'//%' and charindex(N'\', [ReturnPath]) = 0),
    constraint [CK_Framework_AuthTransaction_Expires] check ([Expires] > [Created] and [Expires] <= dateadd(minute, 10, [Created])),
    constraint [CK_Framework_AuthTransaction_Consumed] check ([Consumed] is null or [Consumed] >= [Created]),
);

go

create nonclustered index [IX_Framework_AuthTransaction_Expires]
    on [Framework].[AuthTransaction] ([Expires])
    where [Consumed] is null;