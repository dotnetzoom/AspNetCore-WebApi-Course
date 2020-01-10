namespace Entities.AuditableEntity
{
    /// <summary>
    /// It's a marker interface, in order to make our entities audit-able.
    /// Every entity you mark with this interface, will save audit info to the database.
    /// </summary>
    public interface IAuditableEntity
    {
    }
}