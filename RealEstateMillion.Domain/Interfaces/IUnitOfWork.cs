namespace RealEstateMillion.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IPropertyRepository Properties { get; }
        IPropertyImageRepository PropertyImages { get; }
        IPropertyTraceRepository PropertyTraces { get; }
        IOwnerRepository Owners { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        new void Dispose();
    }
}
