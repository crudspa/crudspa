create proc [EducationSchool].[StudentSecretCodeIsAvailable] (
     @StudentId uniqueidentifier
    ,@SecretCode nvarchar(75)
    ,@Available bit output
) as

if exists (
    select 1
    from [Education].[Student-Active] student
    where student.SecretCode = @SecretCode
        and student.Id != isnull(@StudentId, newid())
)
begin
    set @Available = 0
end
else
begin
    set @Available = 1
end