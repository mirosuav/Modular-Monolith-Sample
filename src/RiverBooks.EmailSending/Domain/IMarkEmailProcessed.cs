using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiverBooks.EmailSending.Domain;

internal interface IMarkEmailProcessed
{
    Task MarkEmailSend(Guid emailId, CancellationToken cancellationToken);
}
