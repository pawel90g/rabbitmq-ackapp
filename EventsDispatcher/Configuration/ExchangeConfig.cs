namespace EventsDispatcher
{
    public class ExchangeConfig
    {
        public long DefaultMessageExpiration { get; }
        public ExchangeConfig(long defaultMessageExpiration)
        {
            DefaultMessageExpiration = defaultMessageExpiration;
        }
    }
}