﻿using Common.Entities;
using Common.Events;
using Orleans.Concurrency;
using Orleans.Interfaces;
using System.Transactions;

namespace Orleans.Transactional;

public interface ITransactionalOrderActor : IOrderActor
{
    [Transaction(TransactionOption.Join)]
    new Task Checkout(ReserveStock reserveStock);

    [OneWay]
    [Transaction(TransactionOption.Join)]
    new Task ProcessShipmentNotification(ShipmentNotification shipmentNotification);

    [Transaction(TransactionOption.CreateOrJoin)]
    new Task<List<Order>> GetOrders();

    [Transaction(TransactionOption.CreateOrJoin)]
    new Task<int> GetNumOrders();

    [Transaction(TransactionOption.CreateOrJoin)]
    Task TestTransaction(Order order);

}
