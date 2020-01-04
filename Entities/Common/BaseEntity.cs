namespace Entities.Common
{
    public interface IEntity
    {
    }

    public abstract class BaseEntity<TKey> : IEntity
    {
        public TKey Id { get; set; }
        public int Version { get; set; }
        public int Status { get; set; } //to check that is post delete or update
    }

    public abstract class BaseEntity : BaseEntity<int>
    {
    }
}