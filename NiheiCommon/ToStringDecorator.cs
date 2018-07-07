namespace Nihei.Common
{
    public class ToStringDecorator<T>
    {
        public ToStringDecorator(T objectValue, string stringValue)
        {
            ObjectValue = objectValue;
            StringValue = stringValue;
        }

        public T ObjectValue { get; set; }

        public string StringValue { get; set; }

        public override string ToString()
        {
            return StringValue;
        }
    }
}
