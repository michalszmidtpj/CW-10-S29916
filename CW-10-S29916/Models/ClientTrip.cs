using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CW_10_S29916.Models;

[Table("Client_Trip")]
public partial class ClientTrip
{
    public int IdClient { get; set; }

    public int IdTrip { get; set; }

    public DateTime RegisteredAt { get; set; }

    public DateTime? PaymentDate { get; set; }

    public virtual Client IdClientNavigation { get; set; } = null!;

    public virtual Trip IdTripNavigation { get; set; } = null!;
}
