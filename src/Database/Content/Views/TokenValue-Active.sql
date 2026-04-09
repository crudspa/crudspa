create view [Content].[TokenValue-Active] as

select tokenValue.Id as Id
    ,tokenValue.TokenId as TokenId
    ,tokenValue.ContactId as ContactId
    ,tokenValue.Value as Value
from [Content].[TokenValue] tokenValue
where 1=1