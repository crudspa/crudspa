create view [Framework].[AuthTransaction-Active] as

select authenticationTransaction.Id as Id
    ,authenticationTransaction.Created as Created
    ,authenticationTransaction.Expires as Expires
    ,authenticationTransaction.Provider as Provider
    ,authenticationTransaction.Audience as Audience
    ,authenticationTransaction.ReturnPath as ReturnPath
    ,authenticationTransaction.Consumed as Consumed
from [Framework].[AuthTransaction] authenticationTransaction
where 1=1