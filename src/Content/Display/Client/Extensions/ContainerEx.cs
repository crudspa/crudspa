using System.Text;

namespace Crudspa.Content.Display.Client.Extensions;

public static class ContainerEx
{
    extension(Shared.Contracts.Data.Container container)
    {
        public String ContainerStyles()
        {
            var styles = new StringBuilder("display: flex; ");

            if (container.DirectionId is null || container.DirectionId.Equals(DirectionIds.Row))
                styles.Append("flex-direction: row; ");
            else if (container.DirectionId.Equals(DirectionIds.Column))
                styles.Append("flex-direction: column; ");

            if (container.WrapId.Equals(WrapIds.Wrap))
                styles.Append("flex-wrap: wrap; ");
            else if (container.WrapId.Equals(WrapIds.None))
                styles.Append("flex-wrap: nowrap; ");
            else if (container.WrapId.Equals(WrapIds.Reverse))
                styles.Append("flex-wrap: wrap-reverse; ");

            if (container.JustifyContentId.Equals(JustifyContentIds.Around))
                styles.Append("justify-content: space-around; ");
            else if (container.JustifyContentId.Equals(JustifyContentIds.Between))
                styles.Append("justify-content: space-between; ");
            else if (container.JustifyContentId.Equals(JustifyContentIds.Center))
                styles.Append("justify-content: center; ");
            else if (container.JustifyContentId.Equals(JustifyContentIds.End))
                styles.Append("justify-content: end; ");
            else if (container.JustifyContentId.Equals(JustifyContentIds.Evenly))
                styles.Append("justify-content: space-evenly; ");
            else if (container.JustifyContentId.Equals(JustifyContentIds.Normal))
                styles.Append("justify-content: normal; ");
            else if (container.JustifyContentId.Equals(JustifyContentIds.Start))
                styles.Append("justify-content: start; ");

            if (container.AlignItemsId.Equals(AlignItemsIds.Baseline))
                styles.Append("align-items: baseline; ");
            else if (container.AlignItemsId.Equals(AlignItemsIds.Center))
                styles.Append("align-items: center; ");
            else if (container.AlignItemsId.Equals(AlignItemsIds.End))
                styles.Append("align-items: end; ");
            else if (container.AlignItemsId.Equals(AlignItemsIds.Normal))
                styles.Append("align-items: normal; ");
            else if (container.AlignItemsId.Equals(AlignItemsIds.Start))
                styles.Append("align-items: start; ");
            else if (container.AlignItemsId.Equals(AlignItemsIds.Stretch))
                styles.Append("align-items: stretch; ");

            if (container.AlignContentId.Equals(AlignContentIds.Around))
                styles.Append("align-content: space-around; ");
            else if (container.AlignContentId.Equals(AlignContentIds.Between))
                styles.Append("align-content: space-between; ");
            else if (container.AlignContentId.Equals(AlignContentIds.Center))
                styles.Append("align-content: center; ");
            else if (container.AlignContentId.Equals(AlignContentIds.End))
                styles.Append("align-content: end; ");
            else if (container.AlignContentId.Equals(AlignContentIds.Evenly))
                styles.Append("align-content: space-evenly; ");
            else if (container.AlignContentId.Equals(AlignContentIds.Normal))
                styles.Append("align-content: normal; ");
            else if (container.AlignContentId.Equals(AlignContentIds.Start))
                styles.Append("align-content: start; ");
            else if (container.AlignContentId.Equals(AlignContentIds.Stretch))
                styles.Append("align-content: stretch; ");

            if (container.Gap.HasSomething())
                styles.Append($"gap: {container.Gap}; ");

            return styles.ToString();
        }
    }
}