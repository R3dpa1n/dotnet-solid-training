using System;

namespace DevBasics.CarManagement.Dependencies
{
    public class GetCarHistoryResponseModel
    {
        public string JsonRequest { get; set; }
        public string JsonResponse { get; set; }
        public string Url { get; set; }
        public string RequestHeader { get; set; }
        public DateTime CreationDate { get; set; }
        public string ResponseHeader { get; set; }
    }
}