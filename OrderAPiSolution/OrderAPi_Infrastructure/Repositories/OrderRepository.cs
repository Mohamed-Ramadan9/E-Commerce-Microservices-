﻿
using System.Linq.Expressions;
using E_Commerce.SharedLibrary.Logs;
using E_Commerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderAPi_Application.Interfaces;
using OrderAPi_Domain.Entites;
using OrderAPi_Infrastructure.Data;

namespace OrderAPi_Infrastructure.Repositories
{
    public class OrderRepository(OrderDbContext context) : IOrder
    {
        public async Task<Response> CreateAsync(Order entity)
        {
           try
            {
               var order = context.Orders.Add(entity).Entity;
                await context.SaveChangesAsync();
                return order.Id > 0 ? new Response(true, "Order placed successfully") :
                    new Response(false, "Error placing order");
            }
            catch(Exception ex) 
            {
                // Log Original Exception
                LogException.LogExceptions(ex);

                //Display message to the client
                return new Response(false, "Error occured while placing order");
            
            }
        }

        public async Task<Response> DeleteAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);
                if (order == null)
                    return new Response(false, "Order not found");
                context.Orders.Remove(order);
                await context.SaveChangesAsync();
                return new Response(true, "Order has Successfully deleted");
                        

            }
            catch (Exception ex)
            {
                // Log Original Exception
                LogException.LogExceptions(ex);

                //Display message to the client
                return new Response(false, "Error occured while placing order");

            }
        }

        public async Task<Order> FindByIdAsync(int id)
        {
            try
            {
                var order = await context.Orders!.FindAsync(id);
                return order;

            }
            catch (Exception ex)
            {
                // Log Original Exception
                LogException.LogExceptions(ex);

                //Display message to the client
                throw new Exception("Error occured while retrieving order");

            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            try
            {
                var orders = await context.Orders.AsNoTracking().ToListAsync();
                return orders;

            }
            catch (Exception ex)
            {
                // Log Original Exception
                LogException.LogExceptions(ex);

                //Display message to the client
                throw new Exception("Error occured while retrieving orders");

            }
        }

        public async Task<Order> GetByAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var order = await context.Orders.Where(predicate).FirstOrDefaultAsync();
                return order;
            }
            catch (Exception ex)
            {
                // Log Original Exception
                LogException.LogExceptions(ex);

                //Display message to the client
                throw new Exception("Error occured while retrieving order");

            }
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var order = await context.Orders.Where(predicate).ToListAsync();
                return order;
            }
            catch (Exception ex)
            {
                // Log Original Exception
                LogException.LogExceptions(ex);

                //Display message to the client
                throw new Exception("Error occured while placing order");

            }
        }

        public async Task<Response> UpdateAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);
                if (order is null)
                {
                    return new Response(false, "order not found");
                }
                context.Entry(order).State = EntityState.Detached;
                context.Orders.Update(entity);
                await context.SaveChangesAsync();
                 
                return new Response(true , "Order Updated");
            }
            catch (Exception ex)
            {
                // Log Original Exception
                LogException.LogExceptions(ex);

                //Display message to the client
                return new Response(false, "Error occured while placing order");

            }
        }
    }
}
