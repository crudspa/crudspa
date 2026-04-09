using System.Text;

namespace Crudspa.Content.Display.Client.Extensions;

public static class ItemEx
{
    extension(Item item)
    {
        public String ItemStyles()
        {
            var styles = new StringBuilder(String.Empty);

            if (item.BasisId.Equals(BasisIds.Auto))
                styles.Append("flex-basis: auto; ");
            else if (item.BasisId.Equals(BasisIds.Fixed))
                styles.Append($"flex-basis: {item.BasisAmount}; ");
            else if (item.BasisId.Equals(BasisIds.Percentage))
                styles.Append($"flex-basis: {item.BasisAmount}; ");

            if (item.Grow.HasSomething())
                styles.Append($"flex-grow: {item.Grow}; ");

            if (item.Shrink.HasSomething())
                styles.Append($"flex-shrink: {item.Shrink}; ");

            if (item.AlignSelfId.Equals(AlignSelfIds.Auto))
                styles.Append("align-self: auto; ");
            else if (item.AlignSelfId.Equals(AlignSelfIds.Baseline))
                styles.Append("align-self: baseline; ");
            else if (item.AlignSelfId.Equals(AlignSelfIds.Center))
                styles.Append("align-self: center; ");
            else if (item.AlignSelfId.Equals(AlignSelfIds.End))
                styles.Append("align-self: end; ");
            else if (item.AlignSelfId.Equals(AlignSelfIds.Start))
                styles.Append("align-self: start; ");
            else if (item.AlignSelfId.Equals(AlignSelfIds.Stretch))
                styles.Append("align-self: stretch; ");

            if (item.MaxWidth.HasSomething())
                styles.Append($"max-width: {item.MaxWidth}; ");

            if (item.MinWidth.HasSomething())
                styles.Append($"min-width: {item.MinWidth}; ");

            if (item.Width.HasSomething())
                styles.Append($"width: {item.Width}; ");

            return styles.ToString();
        }
    }
}