using Entities.Data;
using Entities.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Repositories
{
    public interface IUnitOfWork
    {
        EntityRepo<Address> AddressRepo { get; }
        EntityRepo<Category> CategoryRepo { get; }
        EntityRepo<Order> OrderRepo { get; }
        EntityRepo<OrderItem> OrderItemRepo { get; }
        EntityRepo<Product> ProductRepo { get; }
        EntityRepo<Cart> CartRepo { get; }
        EntityRepo<CartItem> CartItemRepo { get; }

        int SaveChanges();
        IDbContextTransaction BeginTransaction();
    }

    public class UnitOfWork : IUnitOfWork
    {
        AppDbContext dbContext;

        public EntityRepo<Address> AddressRepo { get; }
        public EntityRepo<Category> CategoryRepo { get; }
        public EntityRepo<Order> OrderRepo { get; }
        public EntityRepo<OrderItem> OrderItemRepo { get; }
        public EntityRepo<Product> ProductRepo { get; }
        public EntityRepo<Cart> CartRepo { get; }
        public EntityRepo<CartItem> CartItemRepo { get; }



        public UnitOfWork(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
            AddressRepo = new EntityRepo<Address>(dbContext);
            CategoryRepo = new EntityRepo<Category>(dbContext);
            OrderRepo = new EntityRepo<Order>(dbContext);
            OrderItemRepo = new EntityRepo<OrderItem>(dbContext);
            ProductRepo = new EntityRepo<Product>(dbContext);
            CartRepo = new EntityRepo<Cart>(dbContext);
            CartItemRepo = new EntityRepo<CartItem>(dbContext);
        }

        public int SaveChanges()
        {
            return dbContext.SaveChanges();
        }

        public IDbContextTransaction BeginTransaction()
        {
            return dbContext.Database.BeginTransaction();
        }
    }
}