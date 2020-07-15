using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ICurrencyConverterSvc
    {   public Task<CurrencyConversion> GetCurrencyConvertedAmount(string foreignCurrency, string conversionType, decimal amountToConvert, DateTime? exchangeRateDate = null);
    }
}
