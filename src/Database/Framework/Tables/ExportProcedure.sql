create table [Framework].[ExportProcedure] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ProcedureName] nvarchar(100) not null,
    constraint [PK_Framework_ExportProcedure] primary key clustered ([Id]),
);