create table [Education].[ReportViewed] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ReportId] uniqueidentifier not null,
    constraint [PK_Education_ReportViewed] primary key clustered ([Id]),
    constraint [FK_Education_ReportViewed_Report] foreign key ([ReportId]) references [Education].[Report] ([Id]),
);