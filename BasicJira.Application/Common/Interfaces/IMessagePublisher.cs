using System;
using System.Collections.Generic;
using System.Text;

namespace BasicJira.Application.Common.Interfaces;

public interface IMessagePublisher
{
    Task PublishAsync<T>(
        T message,
        CancellationToken cancellationToken = default);
}