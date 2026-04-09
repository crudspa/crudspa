create table [Education].[ReadParagraph] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ReadPartId] uniqueidentifier not null,
    [Text] nvarchar(max) not null,
    [Ordinal] int not null,
    constraint [PK_Education_ReadParagraph] primary key clustered ([Id]),
    constraint [FK_Education_ReadParagraph_ReadPart] foreign key ([ReadPartId]) references [Education].[ReadPart] ([Id]),
);