namespace Crudspa.Education.District.Server.Sproxies;

public static class SchoolContactSchoolYearSelectSelectionsBySchoolContact
{
    public static async Task<IList<Selectable>> Execute(String connection, Guid? schoolContactId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.SchoolContactSchoolYearSelectSelectionsBySchoolContact";

        command.AddParameter("@SchoolContactId", schoolContactId);

        return await command.ReadSelectables(connection);
    }
}