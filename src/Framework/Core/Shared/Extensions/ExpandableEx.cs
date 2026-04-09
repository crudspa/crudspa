namespace Crudspa.Framework.Core.Shared.Extensions;

public static class ExpandableEx
{
    extension(Expandable expandable)
    {
        public void Toggle() =>
            expandable.Expanded = expandable.Expanded != true && expandable.Children.HasItems();

        public Boolean HasChild(Guid? id)
        {
            if (expandable.Children.HasAny(x => x.Id.Equals(id)))
                return true;

            foreach (var child in expandable.Children)
            {
                var result = child.HasChild(id);
                if (result) return true;
            }

            return false;
        }

        public void Select(Guid? id)
        {
            if (id is null) return;

            expandable.Selected = expandable.Id.Equals(id);

            foreach (var child in expandable.Children) child.Select(id);
        }

        private Boolean ExpandSelectedNode()
        {
            if (expandable.Selected == true)
            {
                expandable.Expanded = true;
                return true;
            }

            foreach (var child in expandable.Children)
            {
                if (child.ExpandSelectedNode())
                {
                    expandable.Expanded = true;
                    return true;
                }
            }

            return false;
        }
    }

    extension(IList<Expandable> parents)
    {
        public Guid? FindParent(Guid? id)
        {
            foreach (var parent in parents)
            {
                if (parent.Children.HasAny(x => x.Id.Equals(id)))
                    return parent.Id;

                var result = parent.Children.FindParent(id);

                if (result is not null)
                    return result;
            }

            return null;
        }

        public void ExpandSelected()
        {
            foreach (var parent in parents) parent.ExpandSelectedNode();
        }

        public void Select(Guid? id)
        {
            if (id is null) return;

            foreach (var parent in parents)
                parent.Select(id);

            parents.ExpandSelected();
        }
    }
}