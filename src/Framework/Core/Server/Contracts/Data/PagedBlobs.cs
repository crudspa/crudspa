using System.Collections.ObjectModel;

namespace Crudspa.Framework.Core.Server.Contracts.Data;

public class PagedBlobs : Paged
{
    public ObservableCollection<String> BlobKeys
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}