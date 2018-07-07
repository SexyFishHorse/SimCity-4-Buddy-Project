namespace Nihei.SC4Buddy.Model
{
    using System;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public abstract class ModelBase
    {
        protected ModelBase(Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
        }

        [JsonProperty]
        public Guid Id { get; }

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

            return obj is ModelBase other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        protected bool Equals(ModelBase other)
        {
            return Id.Equals(other.Id);
        }
    }
}
