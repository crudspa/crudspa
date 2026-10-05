declare @sessionId uniqueidentifier = '22f1a393-c003-4587-8f1d-02369d9c6c53'
declare @now datetimeoffset = sysdatetimeoffset()
declare @id uniqueidentifier

declare @major tinyint
declare @minor tinyint
declare @build tinyint
declare @revision tinyint
declare @notes nvarchar(max)

select top 1
     @major = Major
    ,@minor = Minor
    ,@build = Build
    ,@revision = Revision
from [Framework].[Version]
order by Major desc, Minor desc, Build desc, Revision desc

if (@major is null)
begin
    set @major = 1
    set @minor = 0
    set @build = 100
    set @revision = 0
    set @notes = 'Initial deployment.'

    insert [Framework].[Version] (Id, Created, Major, Minor, Build, Revision, Notes)
    values (newid(), @now, 1, 0, @build, 0, @notes)
end

-- Apply updates in order, incrementing the version number as you go

if (@major = 1 and @minor = 0 and @build = 100 and @revision = 0)
begin
    set @build = 101
    set @notes = 'Sample upgrade script.'

    print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + concat('Upgrading to ', @major, '.', @minor, '.', @build, '.', @revision);
    print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + @notes

    insert [Framework].[Version] (Id, Created, Major, Minor, Build, Revision, Notes)
    values (newid(), @now, 1, 0, @build, 0, @notes)
end


if (@major = 1 and @minor = 0 and @build = 101)
begin
    set xact_abort on
    begin transaction

    -- Follow the existing content author grants and page-builder availability.
    declare @PortalPermissionRows table(Id uniqueidentifier not null default(newid()),PortalId uniqueidentifier not null);
    insert @PortalPermissionRows(PortalId)
    select PortalId from Framework.[PortalPermission-Active] existing
    where existing.PermissionId='b8b24c2e-b256-48cc-a5ae-43bcd2f78d80'
    and not exists(select 1 from Framework.[PortalPermission-Active] currentGrant where currentGrant.PortalId=existing.PortalId and currentGrant.PermissionId='d80bf2bd-a021-5516-a69d-fdc907a5b4fb');
    insert Framework.PortalPermission(Id,VersionOf,UpdatedBy,PortalId,PermissionId)
    select Id,Id,@sessionId,PortalId,'d80bf2bd-a021-5516-a69d-fdc907a5b4fb' from @PortalPermissionRows;
    declare @RolePermissionRows table(Id uniqueidentifier not null default(newid()),RoleId uniqueidentifier not null);
    insert @RolePermissionRows(RoleId)
    select RoleId from Framework.[RolePermission-Active] existing
    where existing.PermissionId='b8b24c2e-b256-48cc-a5ae-43bcd2f78d80'
    and not exists(select 1 from Framework.[RolePermission-Active] currentGrant where currentGrant.RoleId=existing.RoleId and currentGrant.PermissionId='d80bf2bd-a021-5516-a69d-fdc907a5b4fb');
    insert Framework.RolePermission(Id,VersionOf,UpdatedBy,RoleId,PermissionId)
    select Id,Id,@sessionId,RoleId,'d80bf2bd-a021-5516-a69d-fdc907a5b4fb' from @RolePermissionRows;
    declare @PortalPaneTypeRows table(Id uniqueidentifier not null default(newid()),PortalId uniqueidentifier not null);
    insert @PortalPaneTypeRows(PortalId)
    select PortalId from Framework.[PortalPaneType-Active] existing
    where existing.TypeId='5244a27e-d872-4842-853a-0edd15aa0fad'
    and existing.PortalId in(select Id from Framework.[Portal-Active] where [Key] in(N'composer',N'consumer'))
    and not exists(select 1 from Framework.[PortalPaneType-Active] currentGrant where currentGrant.PortalId=existing.PortalId and currentGrant.TypeId='17963cb5-fdea-5d4a-bea6-3abe465f4d3c');
    insert Framework.PortalPaneType(Id,VersionOf,UpdatedBy,PortalId,TypeId)
    select Id,Id,@sessionId,PortalId,'17963cb5-fdea-5d4a-bea6-3abe465f4d3c' from @PortalPaneTypeRows;
    declare @blogId uniqueidentifier,@parentId uniqueidentifier,@portalId uniqueidentifier,@statusId uniqueidentifier;
    declare publicationNav cursor local fast_forward for
     select Id,ParentId,PortalId,StatusId from Framework.[Segment-Active] where [Key]=N'blogs'
     and not exists(select 1 from Framework.[Segment-Active] publicationNav where publicationNav.ParentId=Framework.[Segment-Active].ParentId and publicationNav.[Key]=N'publications');
    open publicationNav;
    fetch next from publicationNav into @blogId,@parentId,@portalId,@statusId;
    while @@fetch_status=0
    begin
     declare @listId uniqueidentifier=newid(),@editId uniqueidentifier=newid(),@groupsId uniqueidentifier=newid(),@groupEditId uniqueidentifier=newid();
     insert Framework.Segment(Id,VersionOf,UpdatedBy,[Key],Title,StatusId,Fixed,Routable,Navigable,Mapable,RequiresId,PortalId,TypeId,PermissionId,ParentId,Ordinal)
     values
     (@listId,@listId,@sessionId,N'publications',N'Publications',@statusId,0,1,1,1,0,@portalId,'35f404f9-c08b-4c71-88c9-794b60741332','d80bf2bd-a021-5516-a69d-fdc907a5b4fb',@parentId,2),
     (@editId,@editId,@sessionId,N'publication',N'Publication',@statusId,0,1,1,0,1,@portalId,'35f404f9-c08b-4c71-88c9-794b60741332','d80bf2bd-a021-5516-a69d-fdc907a5b4fb',@listId,0),
     (@groupsId,@groupsId,@sessionId,N'publication-groups',N'Publication groups',@statusId,0,1,1,1,0,@portalId,'35f404f9-c08b-4c71-88c9-794b60741332','d80bf2bd-a021-5516-a69d-fdc907a5b4fb',@parentId,3),
     (@groupEditId,@groupEditId,@sessionId,N'publication-group',N'Publication group',@statusId,0,1,1,0,1,@portalId,'35f404f9-c08b-4c71-88c9-794b60741332','d80bf2bd-a021-5516-a69d-fdc907a5b4fb',@groupsId,0);
     declare @listPaneId uniqueidentifier=newid(),@editPaneId uniqueidentifier=newid(),@groupsPaneId uniqueidentifier=newid(),@groupEditPaneId uniqueidentifier=newid();
     insert Framework.Pane(Id,VersionOf,UpdatedBy,SegmentId,TypeId,[Key],Ordinal)
     values
     (@listPaneId,@listPaneId,@sessionId,@listId,'b6390fce-177a-5f21-aed2-e427fc9b65fa',N'details',0),
     (@editPaneId,@editPaneId,@sessionId,@editId,'b8a53b27-9442-54da-9b31-885cc99edd3c',N'details',0),
     (@groupsPaneId,@groupsPaneId,@sessionId,@groupsId,'6bc4d2fb-b68d-54e0-9c77-d9795a333b6e',N'details',0),
     (@groupEditPaneId,@groupEditPaneId,@sessionId,@groupEditId,'9a2e0ab2-4b36-5f2e-901b-4268f1728e98',N'details',0);
     fetch next from publicationNav into @blogId,@parentId,@portalId,@statusId;
    end
    close publicationNav;
    deallocate publicationNav;

    -- Expose the tools in the selected-site editor using its existing content feature pattern.
    insert Framework.PortalFeature(Id,PortalId,[Key],Title,IconId,PermissionId)
    select newid(),existing.PortalId,feature.[Key],feature.Title,existing.IconId,'d80bf2bd-a021-5516-a69d-fdc907a5b4fb'
    from Framework.PortalFeature existing
    cross join (values(N'publications',N'Publications'),(N'publication-groups',N'Publication groups')) feature([Key],Title)
    where existing.[Key]=N'blogs' and not exists(select 1 from Framework.PortalFeature currentFeature where currentFeature.PortalId=existing.PortalId and currentFeature.[Key]=feature.[Key]);

    declare @authorPublicationPaneRows table(Id uniqueidentifier not null default(newid()),PortalId uniqueidentifier not null,TypeId uniqueidentifier not null);
    insert @authorPublicationPaneRows(PortalId,TypeId)
    select existing.PortalId,mapping.TypeId from Framework.[PortalPaneType-Active] existing
    cross join (values
     (cast('0cc00d0a-67ce-4069-9fb3-4df3bf3f0cc7' as uniqueidentifier),cast('b6390fce-177a-5f21-aed2-e427fc9b65fa' as uniqueidentifier)),
     ('0cc00d0a-67ce-4069-9fb3-4df3bf3f0cc7','6bc4d2fb-b68d-54e0-9c77-d9795a333b6e'),
     ('c0b77408-8ec0-4a1d-87f0-019e57a6da1c','b8a53b27-9442-54da-9b31-885cc99edd3c'),
     ('c0b77408-8ec0-4a1d-87f0-019e57a6da1c','9a2e0ab2-4b36-5f2e-901b-4268f1728e98')
    ) mapping(PreviousTypeId,TypeId)
    where existing.TypeId=mapping.PreviousTypeId and not exists(select 1 from Framework.[PortalPaneType-Active] currentType where currentType.PortalId=existing.PortalId and currentType.TypeId=mapping.TypeId);
    insert Framework.PortalPaneType(Id,VersionOf,UpdatedBy,PortalId,TypeId)
    select Id,Id,@sessionId,PortalId,TypeId from @authorPublicationPaneRows;

    set @build = 102
    set @notes = 'Added shared Content publications authoring, groups, resources, and public pane.'
    insert [Framework].[Version] (Id, Created, Major, Minor, Build, Revision, Notes)
    values (newid(), @now, @major, @minor, @build, @revision, @notes)
    commit transaction
end
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Updating statistics...';
exec sp_updatestats
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Statistics updated.';
if (@major = 1 and @minor = 0 and @build = 102)
begin
    set xact_abort on
    begin transaction
    -- Shared publication services do not opt every portal into authoring buttons.
    delete from Framework.PortalFeature
    where PermissionId = 'd80bf2bd-a021-5516-a69d-fdc907a5b4fb'
        and [Key] in (N'publications',N'publication-groups');-- Rehearsal body; use the corresponding version-gated migration for deployment.
-- Keep group management in the standard tabbed Publications screen.
declare @publicationUi table (ListId uniqueidentifier, GroupsId uniqueidentifier);
insert @publicationUi
select publications.Id, groups.Id
from Framework.[Segment-Active] publications
join Framework.[Segment-Active] groups on groups.ParentId = publications.ParentId
    and groups.PortalId = publications.PortalId and groups.[Key] = N'publication-groups'
where publications.[Key] = N'publications'
    and publications.PermissionId = 'd80bf2bd-a021-5516-a69d-fdc907a5b4fb';

update segment set TypeId = 'e86d3ee2-22df-4cb1-bb66-ea417d34edeb'
from Framework.Segment segment join @publicationUi ui on ui.ListId = segment.Id;
update pane set Title = N'Publications'
from Framework.Pane pane join @publicationUi ui on ui.ListId = pane.SegmentId
where pane.Id = pane.VersionOf and pane.TypeId = 'b6390fce-177a-5f21-aed2-e427fc9b65fa';
update pane set SegmentId = ui.ListId, [Key] = N'groups', Title = N'Groups', Ordinal = 1
from Framework.Pane pane join @publicationUi ui on ui.GroupsId = pane.SegmentId
where pane.Id = pane.VersionOf and pane.TypeId = '6bc4d2fb-b68d-54e0-9c77-d9795a333b6e';
update segment set ParentId = ui.ListId
from Framework.Segment segment join @publicationUi ui on ui.GroupsId = segment.ParentId
where segment.Id = segment.VersionOf and segment.[Key] = N'publication-group';
update segment set IsDeleted = 1
from Framework.Segment segment join @publicationUi ui on ui.GroupsId = segment.Id;

    set @build = 103
    set @notes = 'Consolidated standard publication authoring tabs and explicit portal feature opt-in.'
    insert Framework.Version(Id,Created,Major,Minor,Build,Revision,Notes)
    values(newid(),@now,@major,@minor,@build,@revision,@notes)
    commit transaction
end