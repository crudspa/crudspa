create proc [EducationCommon].[ActivityChoiceSelectionInsert] (
     @SessionId uniqueidentifier
    ,@AssignmentId uniqueidentifier
    ,@ChoiceId uniqueidentifier
    ,@Made datetimeoffset
    ,@TargetChoiceId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

-- If choice id doesn't exist it is a red herring
if not exists(select 1 from [Education].[ActivityChoice-Active] where Id = @ChoiceId)
begin
    set @ChoiceId = '00000000-0000-0000-0000-000000000000'
end

begin transaction

    insert [Education].[ActivityChoiceSelection] (
        Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,AssignmentId
        ,ChoiceId
        ,Made
        ,TargetChoiceId
    )
    values (
        @Id
        ,@Id
        ,@now
        ,@SessionId
        ,@AssignmentId
        ,@ChoiceId
        ,@Made
        ,@TargetChoiceId
    )

commit transaction