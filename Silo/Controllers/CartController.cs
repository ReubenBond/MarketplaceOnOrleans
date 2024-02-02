﻿using Common.Entities;
using Common.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using OrleansApp.Interfaces;
using Orleans.Interfaces.Replication;
using Common;

namespace Silo.Controllers;

[ApiController]
public sealed class CartController : ControllerBase
{
    private readonly ILogger<CartController> logger;

    // pick type of cart actor: default, eventual, redis
    private delegate ICartActor GetCartActorDelegate(IGrainFactory grains, long customerId);
    private readonly GetCartActorDelegate callback;

    public CartController(AppConfig config, ILogger<CartController> logger)
    {
        this.logger = logger;
        this.callback = config.StreamReplication ? GetEventualCartActor : GetCartActor;
    }

    private ICartActor GetCartActor(IGrainFactory grains, long customerId)
    {
        return grains.GetGrain<ICartActor>(customerId);
    }

    private ICartActor GetEventualCartActor(IGrainFactory grains, long customerId)
    {
        return grains.GetGrain<IEventualCartActor>(customerId);
    }

    [Route("/cart/{customerId}/add")]
    [HttpPatch]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    public async Task<ActionResult> AddItem([FromServices] IGrainFactory grains, long customerId, [FromBody] CartItem item)
    {
        var cartGrain = this.callback(grains, customerId);
        await cartGrain.AddItem(item);
        return Ok();
    }

    [Route("/cart/{customerId}/checkout")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> NotifyCheckout([FromServices] IGrainFactory grains, long customerId, [FromBody] CustomerCheckout customerCheckout)
    {
        var cartGrain = this.callback(grains, customerId);
        try
        {
            await cartGrain.NotifyCheckout(customerCheckout);
            return Ok();
        } catch(Exception e)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
        }
    }

    [Route("/cart/{customerId}/seal")]
    [HttpPatch]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> Seal([FromServices] IGrainFactory grains, int customerId)
    {
        var cartGrain = this.callback(grains, customerId);
        try 
        {
            await cartGrain.Seal();
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
        }
    }
}

