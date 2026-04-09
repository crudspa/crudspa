create table [Education].[ResultElement] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ElementId] uniqueidentifier not null,
    [ActivityElementId] uniqueidentifier not null,
    constraint [PK_Education_ResultElement] primary key clustered ([Id]),
    constraint [FK_Education_ResultElement_Element] foreign key ([ElementId]) references [Content].[Element] ([Id]),
    constraint [FK_Education_ResultElement_ActivityElement] foreign key ([ActivityElementId]) references [Education].[ActivityElement] ([Id]),
);