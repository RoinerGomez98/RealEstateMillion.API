using Microsoft.EntityFrameworkCore.Storage;
using RealEstateMillion.Domain.Interfaces;
using RealEstateMillion.Infrastructure.Data.Context;
using RealEstateMillion.Infrastructure.Data.Repositories;

namespace RealEstateMillion.Infrastructure.Data.UnitOfWork
{
    public class UnitOfWork(RealEstateMillionDbContext context) : IUnitOfWork,IDisposable
    {
        private readonly RealEstateMillionDbContext _context = context;
        private IDbContextTransaction? _transaction;

        private IPropertyRepository? _properties;
        private IPropertyImageRepository? _propertyImages;
        private IPropertyTraceRepository? _propertyTraces;
        private IOwnerRepository? _owners;

        public IPropertyRepository Properties
        {
            get { return _properties ??= new PropertyRepository(_context); }
        }

        public IPropertyImageRepository PropertyImages
        {
            get { return _propertyImages ??= new PropertyImageRepository(_context); }
        }

        public IPropertyTraceRepository PropertyTraces
        {
            get { return _propertyTraces ??= new PropertyTraceRepository(_context); }
        }

        public IOwnerRepository Owners
        {
            get { return _owners ??= new OwnerRepository(_context); }
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while saving changes to the database.", ex);
            }
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                try
                {
                    await _transaction.CommitAsync();
                }
                catch
                {
                    await RollbackTransactionAsync();
                    throw;
                }
                finally
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                try
                {
                    await _transaction.RollbackAsync();
                }
                finally
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
