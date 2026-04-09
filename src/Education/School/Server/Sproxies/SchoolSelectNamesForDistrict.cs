namespace Crudspa.Education.School.Server.Sproxies;

public static class SchoolSelectNamesForDistrict
{
    public static async Task<IList<Named>> Execute(String connection, Guid? districtId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.SchoolSelectNamesForDistrict";

        command.AddParameter("@DistrictId", districtId);

        return await command.ReadNameds(connection);
    }
}