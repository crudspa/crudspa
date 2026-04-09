declare @now datetimeoffset = sysdatetimeoffset()
declare @sessionId uniqueidentifier = '22f1a393-c003-4587-8f1d-02369d9c6c53'

if not exists (select 1 from [Framework].[Organization] where Id = '7eaa4a2d-5a80-4c2a-8fc0-5fa5b70d55c1')
begin
        insert [Framework].[Organization] (
             Id
            ,VersionOf
            ,Updated
            ,UpdatedBy
            ,IsDeleted
            ,Name
            ,TimeZoneId
        )
        values (
             '7eaa4a2d-5a80-4c2a-8fc0-5fa5b70d55c1'
            ,'7eaa4a2d-5a80-4c2a-8fc0-5fa5b70d55c1'
            ,@now
            ,@sessionId
            ,0
            ,'Crudspa Samples'
            ,'America/Denver'
        )
end

if not exists (select 1 from [Framework].[Organization] where Id = '32ff24f1-6c27-4ec4-aec0-ca2b8d6ac192')
begin
        insert [Framework].[Organization] (
             Id
            ,VersionOf
            ,Updated
            ,UpdatedBy
            ,IsDeleted
            ,Name
            ,TimeZoneId
        )
        values (
             '32ff24f1-6c27-4ec4-aec0-ca2b8d6ac192'
            ,'32ff24f1-6c27-4ec4-aec0-ca2b8d6ac192'
            ,@now
            ,@sessionId
            ,0
            ,'Catalog Company'
            ,'America/New_York'
        )
end