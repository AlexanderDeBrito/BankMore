namespace BankMore.TransferService.Domain.Entities
{
    public class IdempotencyKey
    {
        public string Key { get; set; }
        public string Request { get; set; }
        public string Result { get; set; }


        public IdempotencyKey(string key, string request, string result)
        {
            Key = key;
            Request = request;
            Result = result;
        }

        public IdempotencyKey()
        {
        }
    }
}
