if (exists(select * from information_schema.tables where table_schema = 'Framework' and table_name = 'Version'))
begin
    declare @now datetimeoffset = sysdatetimeoffset()

    -- Get the latest version number
    declare @major tinyint
    declare @minor tinyint
    declare @build tinyint
    declare @revision tinyint

    select top 1
         @major = Major
        ,@minor = Minor
        ,@build = Build
        ,@revision = Revision
    from [Framework].[Version]
    order by Major desc, Minor desc, Build desc, Revision desc

    -- Version-specific scripts
    --if (@major = 1 and @minor = 0 and @build = 100 and @revision = 0)
    --begin
    --end
end