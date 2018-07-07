namespace Nihei.SC4Buddy.Model
{
    using System;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public abstract class ModelBase
    {
        [JsonProperty]
        public Guid Id { get; set; }

        protected ModelBase()
        {
            Id = Guid.NewGuid();
        }

        protected bool Equals(ModelBase other)
        {
            return Id.Equals(other.Id);
        }

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
            var other = obj as ModelBase;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
