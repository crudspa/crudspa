create table [Education].[ActivityElement] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ElementId] uniqueidentifier not null,
    [ActivityId] uniqueidentifier not null,
    constraint [PK_Education_ActivityElement] primary key clustered ([Id]),
    constraint [FK_Education_ActivityElement_Element] foreign key ([ElementId]) references [Content].[Element] ([Id]),
    constraint [FK_Education_ActivityElement_Activity] foreign key ([ActivityId]) references [Education].[Activity] ([Id]),
);