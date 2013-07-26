namespace NIHEI.Common.UI.Elements
{
    using System.Windows.Forms;

    public class ListViewItemWithObjectValue<T> : ListViewItem
    {
        public ListViewItemWithObjectValue(string text, T value)
            : base(text)
        {
            Value = value;
        }

        public T Value { get; set; }
    }
}
