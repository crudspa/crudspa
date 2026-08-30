create function [EducationRostering].[RosterSelection] (
     @RosterRunId uniqueidentifier
    ,@RosterSourceId uniqueidentifier
)
returns table
as
return
(
    with selectedSchool as (
        select staged.ExternalId
        from [Education].[RosterSchool] staged
            inner join [Education].[RosterSource-Active] source on source.Id = @RosterSourceId
            left join [Education].[RosterSchoolLink-Active] link
                on link.RosterSourceId = @RosterSourceId
                and link.ExternalId = staged.ExternalId
        where staged.RosterRunId = @RosterRunId
            and staged.Kind = N'school'
            and (link.Included = 1 or (staged.Status = N'active' and link.Id is null
                and not exists (
                    select 1
                    from [Education].[School-Active] school
                        inner join [Education].[District-Active] district on district.Id = school.DistrictId
                    where district.OrganizationId = source.OrganizationId
                )
                and not exists (
                    select 1
                    from [Education].[RosterSchoolLink-Active]
                    where RosterSourceId = @RosterSourceId
                )))
    ),
    selectedClass as (
        select class.ExternalId, class.CourseExternalId, class.TermExternalId
        from [Education].[RosterClass] class
            inner join selectedSchool school on school.ExternalId = class.SchoolExternalId
        where class.RosterRunId = @RosterRunId
    ),
    selectedRole as (
        select role.ExternalId, role.PersonExternalId
        from [Education].[RosterRole] role
            left join selectedSchool school on school.ExternalId = role.SchoolExternalId
        where role.RosterRunId = @RosterRunId
            and (role.SchoolExternalId is null or school.ExternalId is not null)
    ),
    selectedPerson as (
        select distinct PersonExternalId ExternalId
        from selectedRole
    ),
    selectedEnrollment as (
        select enrollment.ExternalId
        from [Education].[RosterEnrollment] enrollment
            inner join selectedClass class on class.ExternalId = enrollment.ClassExternalId
        where enrollment.RosterRunId = @RosterRunId
    ),
    selectedCourse as (
        select distinct course.ExternalId
        from [Education].[RosterCourse] course
            inner join selectedClass class on class.CourseExternalId = course.ExternalId
        where course.RosterRunId = @RosterRunId
    ),
    selectedTerm as (
        select distinct term.ExternalId
        from [Education].[RosterTerm] term
            inner join selectedClass class on class.TermExternalId = term.ExternalId
        where term.RosterRunId = @RosterRunId
    )
    select N'school' Kind, ExternalId from selectedSchool
    union all select N'class', ExternalId from selectedClass
    union all select N'role', ExternalId from selectedRole
    union all select N'person', ExternalId from selectedPerson
    union all select N'enrollment', ExternalId from selectedEnrollment
    union all select N'course', ExternalId from selectedCourse
    union all select N'term', ExternalId from selectedTerm
)