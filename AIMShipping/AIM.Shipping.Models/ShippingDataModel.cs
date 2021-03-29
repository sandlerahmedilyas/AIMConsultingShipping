using System;

namespace AIM.Shipping.Models
{
    /// <summary>
    /// Simple data class that represents the data model in question.
    /// </summary>
    public sealed class ShippingDataModel
    {
        public int Id { get; set; }

        public int PetId { get; set; }

        public int Quantity { get; set; }

        public DateTime? ShipDate { get; set; }

        public string Status { get; set; }

        public bool Complete { get; set; }

    }
}
