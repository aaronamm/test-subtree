using Newtonsoft.Json;
using System;

namespace CodeSanook.Common.Models
{
    public abstract class Entity<TId>
    {
        [JsonProperty("id")]
        public virtual TId Id { get; set; }//Allow settable for JSON.NET

        public override bool Equals(object obj) => Equals(obj as Entity<TId>);

        public virtual bool Equals(Entity<TId> other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (!IsTransient(this) && !IsTransient(other) && Equals(Id, other.Id))
            {
                var thisType = GetType();
                var otherType = other.GetType();
                return thisType.IsAssignableFrom(otherType) || otherType.IsAssignableFrom(thisType);
            }

            return false;
        }

        /// <summary>
        /// If object has assigned ID, use hash from ID
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (Equals(Id, default(TId)))
            {
                return base.GetHashCode();
            }

            return Id.GetHashCode();
        }

        /// <summary>
        /// new object but haven't get assign ID value
        /// current ID is equal to the default value of ID type
        /// </summary>
        private static bool IsTransient(Entity<TId> obj) => 
            obj != null && Equals(obj.Id, default(TId));

        private Type GetUnproxiedType() => GetType();

    }
}