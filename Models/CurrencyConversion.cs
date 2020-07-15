using System;

namespace Models
{
    public class CurrencyConversion
    {
        private string fromCurrencyIsoCode;
        private string toCurrencyIsoCode;
        private decimal exchangeRate;
        private decimal amountConverted;
        private DateTime exchangeRateDate;

        public CurrencyConversion() {}

        /// <summary>
        /// Basic model containing information related to the result of a currency conversion.
        /// </summary>
        /// <param name="fromCurrencyIsoCode">The from currency code in ISO 4217 format</param>
        /// <param name="toCurrencyIsoCode">The to currency code in ISO 4217 format</param>
        /// <param name="exchangeRate">The exchange rate used to calculate the amount</param>
        /// <param name="amountConverted">The result of the conversion from the fromCurrencyIsoCode to toCurrencyIsoCode using the exchange rate at the exchangeRateDate</param>
        /// <param name="exchangeRateDate">The date of the exchange rate used</param>
        public CurrencyConversion(string fromCurrencyIsoCode, string toCurrencyIsoCode, decimal exchangeRate, decimal amountConverted, DateTime exchangeRateDate)
        {
            this.fromCurrencyIsoCode = fromCurrencyIsoCode;
            this.toCurrencyIsoCode = toCurrencyIsoCode;
            this.exchangeRate = exchangeRate;
            this.amountConverted = amountConverted;
            this.exchangeRateDate = exchangeRateDate;
        }

        public string FromCurrencyIsoCode
        {
            get { return fromCurrencyIsoCode; }
            set { fromCurrencyIsoCode = value; }
        }
        public string ToCurrencyIsoCode
        {
            get { return toCurrencyIsoCode; }
            set { toCurrencyIsoCode = value; }
        }
        public decimal ExchangeRate
        {
            get { return exchangeRate; }
            set { exchangeRate = value; }
        }

        public decimal AmountConverted
        {
            get { return amountConverted; }
            set { amountConverted = value; }
        }

        public DateTime ExchangeRateDate
        {
            get { return exchangeRateDate; }
            set { exchangeRateDate = value; }
        }

    }
}
