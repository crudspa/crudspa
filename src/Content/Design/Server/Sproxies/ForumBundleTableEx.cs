using System.Data;

namespace Crudspa.Content.Design.Server.Sproxies;

public static class ForumBundleTableEx
{
    extension(IEnumerable<ForumBundle> forumBundles)
    {
        public DataTable ToForumBundleTable()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("BundleId", typeof(Guid));
            dataTable.Columns.Add("ThreadRule", typeof(Int32));
            dataTable.Columns.Add("CommentRule", typeof(Int32));

            foreach (var forumBundle in forumBundles.Where(x =>
                         x.BundleId.HasValue && x.ThreadRule != ForumBundle.Rules.NotUsed))
            {
                dataTable.Rows.Add(
                    forumBundle.BundleId,
                    (Int32)forumBundle.ThreadRule,
                    (Int32)ForumBundle.Rules.NotUsed);
            }

            return dataTable;
        }
    }
}