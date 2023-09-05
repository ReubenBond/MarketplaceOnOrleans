﻿using Common.Entities;
using Common.Events;
using Microsoft.Extensions.Logging;
using Orleans.Interfaces;
using Orleans.Runtime;

namespace Orleans.Grains;

public class SellerActor : Grain, ISellerActor
{
    private readonly ILogger<SellerActor> logger;

    private readonly IPersistentState<Seller> seller;

    private readonly List<int> productIds = new List<int>();

    public SellerActor(
        [PersistentState(stateName: "seller", storageName: Infra.Constants.OrleansStorage)] IPersistentState<Seller> seller,
        ILogger<SellerActor> logger)
    {
        this.seller = seller;
        this.logger = logger;
    }

    // only in case there is interactive product query
    public Task IndexProduct(int productId)
    {
        this.productIds.Add(productId);
        return Task.CompletedTask;
    }

    public async Task SetSeller(Seller seller)
    {
        this.seller.State = seller;
        await this.seller.WriteStateAsync();
    }

    public Task ProcessNewInvoice(InvoiceIssued invoiceIssued)
    {
        // logger.LogInformation("New invoice received: ", invoiceIssued);
        return Task.CompletedTask;
    }

    public Task ProcessPaymentConfirmed(PaymentConfirmed paymentConfirmed)
    {
        throw new NotImplementedException();
    }

    public Task ProcessPaymentFailed(PaymentFailed paymentFailed)
    {
        throw new NotImplementedException();
    }

    public Task ProcessShipmentNotification(ShipmentNotification shipmentNotification)
    {
        // logger.LogInformation("New invoice received: ", invoiceIssued);
        return Task.CompletedTask;
    }

    public Task ProcessDeliveryNotification(DeliveryNotification deliveryNotification)
    {
        return Task.CompletedTask;
    }

}


