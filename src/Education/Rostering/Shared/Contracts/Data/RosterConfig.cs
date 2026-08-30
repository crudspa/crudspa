using Crudspa.Education.Rostering.Shared.Contracts.Ids;
using Crudspa.Framework.Core.Shared.BaseClasses;
using Crudspa.Framework.Core.Shared.Contracts.Behavior;
using Crudspa.Framework.Core.Shared.Contracts.Data;
using Crudspa.Framework.Core.Shared.Extensions;
using System.Collections.ObjectModel;

namespace Crudspa.Education.Rostering.Shared.Contracts.Data;

public class RosterConfig : Observable, IValidates
{
    public ObservableCollection<RosterSource> Sources
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            foreach (var source in Sources)
                errors.AddRange(source.Validate());

            if (Sources.Count(x => x.Mode == RosterModes.Authoritative) > 1)
                errors.AddError("Only one roster source can be Authoritative.");

            if (Sources.GroupBy(x => new { x.Provider, x.Tenant }).Any(x => x.Count() > 1))
                errors.AddError("A roster source can only be configured once for a district.");
        });
    }
}