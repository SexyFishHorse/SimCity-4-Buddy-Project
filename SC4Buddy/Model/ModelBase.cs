using System;

namespace NIHEI.SC4Buddy.Model
{
    public abstract class ModelBase
    {
        public Guid Id { get; set; }

        protected ModelBase(Guid id)
        {
            Id = id;
        }

        private bool Equals(ModelBase other)
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

            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((ModelBase)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
