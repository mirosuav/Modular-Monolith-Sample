﻿using MediatR;

namespace RiverBooks.EmailSending.Contracts;

public class SendEmailCommand : INotification
{
    public string To { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}