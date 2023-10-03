using System;

namespace DevBasics.CarManagement.Dependencies
{
    public class CarTransactionHistoryDto
    {
        public int CarId { get; set; }
        public string JsonRequest { get; set; }
        public string JsonResponse { get; set; }
        public string Url { get; set; }
        public string RequestHeader { get; set; }
        public string ResponseHeader { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
