namespace NIHEI.SC4Buddy.View.Elements
{
    public class ComboBoxItem<T>
    {
        public ComboBoxItem(string text, T value)
        {
            Text = text;
            Value = value;
        }

        public string Text { get; set; }

        public T Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
