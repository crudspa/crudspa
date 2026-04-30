create table [Content].[AnswerChoiceReply] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [QuestionReplyId] uniqueidentifier not null,
    [ChoiceId] uniqueidentifier not null,
    constraint [PK_Content_AnswerChoiceReply] primary key clustered ([Id]),
    constraint [FK_Content_AnswerChoiceReply_QuestionReply] foreign key ([QuestionReplyId]) references [Content].[QuestionReply] ([Id]),
    constraint [FK_Content_AnswerChoiceReply_Choice] foreign key ([ChoiceId]) references [Content].[OptionsAnswerChoice] ([Id]),
);