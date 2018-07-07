namespace Nihei.Common.UI.Elements
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    public class ListViewItemWithObjectValue<T> : ListViewItem
    {
        public ListViewItemWithObjectValue(string text, T value)
            : base(text)
        {
            Value = value;
        }

        public T Value { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((ListViewItemWithObjectValue<T>)obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(Value);
        }

        protected bool Equals(ListViewItemWithObjectValue<T> other)
        {
            return EqualityComparer<T>.Default.Equals(Value, other.Value);
        }
    }
}
