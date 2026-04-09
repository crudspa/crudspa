create view [Education].[ChallengeLevel-Active] as

select challengeLevel.Id as Id
    ,challengeLevel.Name as Name
    ,challengeLevel.Ordinal as Ordinal
from [Education].[ChallengeLevel] challengeLevel
where 1=1
    and challengeLevel.IsDeleted = 0