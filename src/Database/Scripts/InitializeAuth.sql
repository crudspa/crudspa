declare @now datetimeoffset = sysdatetimeoffset()
declare @sessionId uniqueidentifier = '22f1a393-c003-4587-8f1d-02369d9c6c53'
declare @catalogOrganizationId uniqueidentifier = '32ff24f1-6c27-4ec4-aec0-ca2b8d6ac192'
declare @samplesOrganizationId uniqueidentifier = '7eaa4a2d-5a80-4c2a-8fc0-5fa5b70d55c1'
declare @catalogConnectionId uniqueidentifier = (
    select top 1 Id
    from [Framework].[AuthConnection-Active]
    where OrganizationId = @catalogOrganizationId
        and Provider = N'password-email-code'
)
declare @samplesConnectionId uniqueidentifier = (
    select top 1 Id
    from [Framework].[AuthConnection-Active]
    where OrganizationId = @samplesOrganizationId
        and Provider = N'password-email-code'
)

if @catalogConnectionId is null
begin
    set @catalogConnectionId = '7e0a4784-3a04-4907-af48-4c25f17e53f8'

    insert [Framework].[AuthConnection] (
         Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,IsDeleted
        ,OrganizationId
        ,Provider
        ,Enabled
    )
    values (
         @catalogConnectionId
        ,@catalogConnectionId
        ,@now
        ,@sessionId
        ,0
        ,@catalogOrganizationId
        ,N'password-email-code'
        ,1
    )
end

if @samplesConnectionId is null
begin
    set @samplesConnectionId = 'e89e1394-14e6-49ec-a42e-e8c5ff406349'

    insert [Framework].[AuthConnection] (
         Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,IsDeleted
        ,OrganizationId
        ,Provider
        ,Enabled
    )
    values (
         @samplesConnectionId
        ,@samplesConnectionId
        ,@now
        ,@sessionId
        ,0
        ,@samplesOrganizationId
        ,N'password-email-code'
        ,1
    )
end

if not exists (
    select 1
    from [Framework].[AuthPolicy-Active]
    where OrganizationId = @catalogOrganizationId
        and Audience = N'catalog'
)
    insert [Framework].[AuthPolicy] (
         Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,IsDeleted
        ,OrganizationId
        ,AuthConnectionId
        ,Audience
        ,IdleTimeoutMinutes
        ,AbsoluteTimeoutMinutes
        ,Persist
        ,AutoRedirect
        ,Fallback
        ,Enabled
    )
    values (
         '725d30e4-06a4-48ca-b8d4-a61f5f4dc29b'
        ,'725d30e4-06a4-48ca-b8d4-a61f5f4dc29b'
        ,@now
        ,@sessionId
        ,0
        ,@catalogOrganizationId
        ,@catalogConnectionId
        ,N'catalog'
        ,4320
        ,10080
        ,0
        ,0
        ,0
        ,1
    )

if not exists (
    select 1
    from [Framework].[AuthPolicy-Active]
    where OrganizationId = @samplesOrganizationId
        and Audience = N'composer'
)
    insert [Framework].[AuthPolicy] (
         Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,IsDeleted
        ,OrganizationId
        ,AuthConnectionId
        ,Audience
        ,IdleTimeoutMinutes
        ,AbsoluteTimeoutMinutes
        ,Persist
        ,AutoRedirect
        ,Fallback
        ,Enabled
    )
    values (
         '7257f433-e1fd-429d-a12f-1fa5ed9da4eb'
        ,'7257f433-e1fd-429d-a12f-1fa5ed9da4eb'
        ,@now
        ,@sessionId
        ,0
        ,@samplesOrganizationId
        ,@samplesConnectionId
        ,N'composer'
        ,4320
        ,10080
        ,0
        ,0
        ,0
        ,1
    )

if not exists (
    select 1
    from [Framework].[AuthPolicy-Active]
    where OrganizationId = @samplesOrganizationId
        and Audience = N'consumer'
)
    insert [Framework].[AuthPolicy] (
         Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,IsDeleted
        ,OrganizationId
        ,AuthConnectionId
        ,Audience
        ,IdleTimeoutMinutes
        ,AbsoluteTimeoutMinutes
        ,Persist
        ,AutoRedirect
        ,Fallback
        ,Enabled
    )
    values (
         'f637d247-b02d-488b-b3f3-f495c98279eb'
        ,'f637d247-b02d-488b-b3f3-f495c98279eb'
        ,@now
        ,@sessionId
        ,0
        ,@samplesOrganizationId
        ,@samplesConnectionId
        ,N'consumer'
        ,4320
        ,10080
        ,0
        ,0
        ,0
        ,1
    )