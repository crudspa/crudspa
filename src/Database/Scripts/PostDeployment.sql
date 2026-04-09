-- Turn on sqlcmd mode to eliminate Intellisense errors

print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeContentStatus.sql...'
:r .\..\Framework\Scripts\MergeContentStatus.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeDevice.sql...'
:r .\..\Framework\Scripts\MergeDevice.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeExportProcedure.sql...'
:r .\..\Framework\Scripts\MergeExportProcedure.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeIcon.sql...'
:r .\..\Framework\Scripts\MergeIcon.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeJobStatus.sql...'
:r .\..\Framework\Scripts\MergeJobStatus.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeJobType.sql...'
:r .\..\Framework\Scripts\MergeJobType.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeDeviceJobType.sql...'
:r .\..\Framework\Scripts\MergeDeviceJobType.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeNavigationType.sql...'
:r .\..\Framework\Scripts\MergeNavigationType.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergePaneType.sql...'
:r .\..\Framework\Scripts\MergePaneType.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergePermission.sql...'
:r .\..\Framework\Scripts\MergePermission.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeSegmentType.sql...'
:r .\..\Framework\Scripts\MergeSegmentType.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeUsaState.sql...'
:r .\..\Framework\Scripts\MergeUsaState.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeAlignContent.sql...'
:r .\..\Content\Scripts\MergeAlignContent.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeAlignItems.sql...'
:r .\..\Content\Scripts\MergeAlignItems.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeAlignSelf.sql...'
:r .\..\Content\Scripts\MergeAlignSelf.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeBasis.sql...'
:r .\..\Content\Scripts\MergeBasis.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeBinderType.sql...'
:r .\..\Content\Scripts\MergeBinderType.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeDirection.sql...'
:r .\..\Content\Scripts\MergeDirection.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeElementType.sql...'
:r .\..\Content\Scripts\MergeElementType.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeJustifyContent.sql...'
:r .\..\Content\Scripts\MergeJustifyContent.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergePageType.sql...'
:r .\..\Content\Scripts\MergePageType.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeRuleType.sql...'
:r .\..\Content\Scripts\MergeRuleType.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeRule.sql...'
:r .\..\Content\Scripts\MergeRule.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeSectionType.sql...'
:r .\..\Content\Scripts\MergeSectionType.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeWrap.sql...'
:r .\..\Content\Scripts\MergeWrap.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeActivityAssignmentStatus.sql...'
:r .\..\Education\Scripts\MergeActivityAssignmentStatus.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeActivityCategory.sql...'
:r .\..\Education\Scripts\MergeActivityCategory.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeActivityStatus.sql...'
:r .\..\Education\Scripts\MergeActivityStatus.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeActivityTypeStatus.sql...'
:r .\..\Education\Scripts\MergeActivityTypeStatus.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeActivityType.sql...'
:r .\..\Education\Scripts\MergeActivityType.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeAssessmentLevel.sql...'
:r .\..\Education\Scripts\MergeAssessmentLevel.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeAssessmentType.sql...'
:r .\..\Education\Scripts\MergeAssessmentType.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeAudioGender.sql...'
:r .\..\Education\Scripts\MergeAudioGender.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeBodyTemplate.sql...'
:r .\..\Education\Scripts\MergeBodyTemplate.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeBookCategory.sql...'
:r .\..\Education\Scripts\MergeBookCategory.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeBookSeason.sql...'
:r .\..\Education\Scripts\MergeBookSeason.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeBookStatus.sql...'
:r .\..\Education\Scripts\MergeBookStatus.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeChallengeLevel.sql...'
:r .\..\Education\Scripts\MergeChallengeLevel.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeClassroomType.sql...'
:r .\..\Education\Scripts\MergeClassroomType.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeConditionGroup.sql...'
:r .\..\Education\Scripts\MergeConditionGroup.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeContentCategory.sql...'
:r .\..\Education\Scripts\MergeContentCategory.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeContentGroup.sql...'
:r .\..\Education\Scripts\MergeContentGroup.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeFamilyMemberType.sql...'
:r .\..\Education\Scripts\MergeFamilyMemberType.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeGameActivityType.sql...'
:r .\..\Education\Scripts\MergeGameActivityType.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeGameSectionType.sql...'
:r .\..\Education\Scripts\MergeGameSectionType.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeGender.sql...'
:r .\..\Education\Scripts\MergeGender.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeGoalSettingGroup.sql...'
:r .\..\Education\Scripts\MergeGoalSettingGroup.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeGrade.sql...'
:r .\..\Education\Scripts\MergeGrade.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeListenQuestionCategory.sql...'
:r .\..\Education\Scripts\MergeListenQuestionCategory.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeMetalinguisticCategory.sql...'
:r .\..\Education\Scripts\MergeMetalinguisticCategory.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeContentArea.sql...'
:r .\..\Education\Scripts\MergeContentArea.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergePersonalizationGroup.sql...'
:r .\..\Education\Scripts\MergePersonalizationGroup.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeRarity.sql...'
:r .\..\Education\Scripts\MergeRarity.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeReadQuestionCategory.sql...'
:r .\..\Education\Scripts\MergeReadQuestionCategory.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeReadQuestionType.sql...'
:r .\..\Education\Scripts\MergeReadQuestionType.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeResearchGroup.sql...'
:r .\..\Education\Scripts\MergeResearchGroup.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeSchoolYear.sql...'
:r .\..\Education\Scripts\MergeSchoolYear.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeStudentStatus.sql...'
:r .\..\Education\Scripts\MergeStudentStatus.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeTitle.sql...'
:r .\..\Education\Scripts\MergeTitle.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeBrand.sql...'
:r .\..\Samples\Scripts\MergeBrand.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeColor.sql...'
:r .\..\Samples\Scripts\MergeColor.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeFormat.sql...'
:r .\..\Samples\Scripts\MergeFormat.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeGenre.sql...'
:r .\..\Samples\Scripts\MergeGenre.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeRating.sql...'
:r .\..\Samples\Scripts\MergeRating.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeSize.sql...'
:r .\..\Samples\Scripts\MergeSize.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeTag.sql...'
:r .\..\Samples\Scripts\MergeTag.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running InitializeOwners.sql...'
:r .\InitializeOwners.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergePortal.sql...'
:r .\..\Framework\Scripts\MergePortal.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergePortalFeature.sql...'
:r .\..\Framework\Scripts\MergePortalFeature.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergePortalPaneType.sql...'
:r .\..\Framework\Scripts\MergePortalPaneType.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergePortalPermission.sql...'
:r .\..\Framework\Scripts\MergePortalPermission.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergePortalSegmentType.sql...'
:r .\..\Framework\Scripts\MergePortalSegmentType.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running MergeReport.sql...'
:r .\..\Education\Scripts\MergeReport.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running Initialize.sql...'
:r .\Initialize.sql
GO
print format(getdate(), 'yyyy-MM-dd HH:mm:ss.fff ') + 'Running AfterUpgrade.sql...'
:r .\AfterUpgrade.sql
GO