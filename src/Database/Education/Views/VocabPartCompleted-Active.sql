create view [Education].[VocabPartCompleted-Active] as

select vocabPartCompleted.Id as Id
    ,vocabPartCompleted.AssignmentId as AssignmentId
    ,vocabPartCompleted.VocabPartId as VocabPartId
    ,vocabPartCompleted.DeviceTimestamp as DeviceTimestamp
from [Education].[VocabPartCompleted] vocabPartCompleted
where 1=1
    and vocabPartCompleted.IsDeleted = 0