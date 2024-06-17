﻿using MediatR;
using RiverBooks.OrderProcessing.Contracts;
using RiverBooks.SharedKernel.Helpers;

namespace RiverBooks.OrderProcessing.ListOrdersForUser;

public record ListOrdersForUserQuery(Guid UserId) :
  IRequest<Resultable<List<OrderSummary>>>;


