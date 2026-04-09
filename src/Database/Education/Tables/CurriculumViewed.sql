create table [Education].[CurriculumViewed] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    constraint [PK_Education_CurriculumViewed] primary key clustered ([Id]),
);