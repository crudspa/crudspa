create type [Content].[ForumBundleList] as table (
     [BundleId] uniqueidentifier not null primary key clustered
    ,[ThreadRule] int not null
    ,[CommentRule] int not null
);