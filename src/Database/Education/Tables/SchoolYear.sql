create table [Education].[SchoolYear] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(25) not null,
    [Starts] date default(convert(date, sysdatetime())) not null,
    [Ends] date default(convert(date, sysdatetime())) not null,
    constraint [PK_Education_SchoolYear] primary key clustered ([Id]),
);