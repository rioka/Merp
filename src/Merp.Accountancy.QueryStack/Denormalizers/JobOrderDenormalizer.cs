﻿using Merp.Accountancy.CommandStack.Events;
using Merp.Accountancy.QueryStack.Model;
using Rebus.Handlers;
using System.Linq;
using System.Threading.Tasks;

namespace Merp.Accountancy.QueryStack.Denormalizers
{
    public class JobOrderDenormalizer : 
        IHandleMessages<JobOrderRegisteredEvent>,
        IHandleMessages<JobOrderExtendedEvent>,
        IHandleMessages<JobOrderCompletedEvent>
    {
        public async Task Handle(JobOrderRegisteredEvent message)
        {
            var jobOrder = new JobOrder();
                jobOrder.OriginalId = message.JobOrderId;
                jobOrder.CustomerId = message.CustomerId;
                jobOrder.Description = message.Description;
                jobOrder.ManagerId = message.ManagerId;
                jobOrder.DateOfStart = message.DateOfStart;
                jobOrder.DueDate = message.DueDate;
                jobOrder.Name = message.JobOrderName;
                jobOrder.Number = message.JobOrderNumber;
                jobOrder.Price = message.Price;
                jobOrder.Currency = message.Currency;
                jobOrder.PurchaseOrderNumber = message.PurchaseOrderNumber;
                jobOrder.IsCompleted = false;
                jobOrder.IsTimeAndMaterial = false;
            jobOrder.IsFixedPrice = true;

            using (var db = new AccountancyContext())
            {
                db.JobOrders.Add(jobOrder);
                await db.SaveChangesAsync();
            }
        }

        public async Task Handle(JobOrderExtendedEvent message)
        {
            using (var db = new AccountancyContext())
            {
                var jobOrder = db.JobOrders.OfType<JobOrder>().Where(jo => jo.OriginalId == message.JobOrderId).Single();
                jobOrder.DueDate = message.NewDueDate;
                jobOrder.Price = message.Price;
                await db.SaveChangesAsync();
            }
            
        }

        public async Task Handle(JobOrderCompletedEvent message)
        {
            using (var db = new AccountancyContext())
            {
                var jobOrder = db.JobOrders
                    .OfType<JobOrder>()
                    .Where(jo => jo.OriginalId == message.JobOrderId)
                    .Single();
                jobOrder.DateOfCompletion = message.DateOfCompletion;
                jobOrder.IsCompleted = true;
                await db.SaveChangesAsync();
            }
        }
    }
}
