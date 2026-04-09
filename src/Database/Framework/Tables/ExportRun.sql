create table [Framework].[ExportRun] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ProcedureId] uniqueidentifier not null,
    [Run] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    constraint [PK_Framework_ExportRun] primary key clustered ([Id]),
    constraint [FK_Framework_ExportRun_Procedure] foreign key ([ProcedureId]) references [Framework].[ExportProcedure] ([Id]),
);