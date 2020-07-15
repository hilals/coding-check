using Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services
{
    /// <summary>
    /// Bank of Canada currency conversion service which uses BOC's API to retrieve information, such as the exhange rate.
    /// </summary>
    public interface IBocCurrencyConverterSvc : ICurrencyConverterSvc{}
    public class BocCurrencyConverterSvc : IBocCurrencyConverterSvc
    {
        public const string conversionPrefix = "FX";
        public const string cadCurrencyISOCode = "CAD";
        private readonly HttpClient _httpClient;

        /// <summary>
        /// The service's constructor. 
        /// </summary>
        /// <param name="httpClient">HttpClient is injected.It is used to make api calls</param>
        public BocCurrencyConverterSvc (HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("https://www.bankofcanada.ca/valet/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient = httpClient;
        }
        /// <summary>
        /// Converts a foreign currency amount from/to Canadian currency.
        /// </summary>
        /// <param name="foreignCurrencyIsoCode">The foreign currency code in ISO 4217 format (ie EUR, USD,etc)</param>
        /// <param name="conversionType">The conversion type, 'from' to convert from the foreign currency to CAD, 'to' to convert from CAD to the foreign currency</param>
        /// <param name="amountToConvert">The amount of the foreign currency to convert</param>
        /// <param name="exchangeRateDate">The date of the exchange rate to use</param>
        /// <returns>Returns a CurrencyConversion object containing the from/to currency, exchange rate, converted amount and the exhange rate date</returns>
        public async Task<CurrencyConversion> GetCurrencyConvertedAmount(string foreignCurrencyIsoCode, string conversionType, decimal amountToConvert, DateTime? exchangeRateDate = null)
        {
            try
            {
                validateConversionParameters(foreignCurrencyIsoCode, conversionType, exchangeRateDate);
                
                var fromCurrencyIsoCode = conversionType == "from" ? foreignCurrencyIsoCode : cadCurrencyISOCode;
                var toCurrencyIsoCode   = fromCurrencyIsoCode == foreignCurrencyIsoCode ? cadCurrencyISOCode : foreignCurrencyIsoCode;

                var conversionLabel = conversionPrefix + fromCurrencyIsoCode + toCurrencyIsoCode;
                var apiCallBase     = "observations/" + conversionLabel;

                // If no date is specified, we retrieve the most recent exchange rate
                var reqUri = exchangeRateDate != null ? String.Format("{0}?start_date={1}&end_date={1}",apiCallBase, exchangeRateDate.Value.ToString("yyyy-MM-dd"))
                            : apiCallBase + "?recent=1";

                var response = await _httpClient.GetAsync(reqUri);

                response.EnsureSuccessStatusCode();
              
                var apiResponse = await response.Content.ReadAsStringAsync();

                var jsonDoc      = JsonDocument.Parse(apiResponse);
                var root         = jsonDoc.RootElement;
                var observations = root.GetProperty("observations");

                if (observations.GetArrayLength().Equals(0))
                {
                    throw new Exception("Exchange rate not found for the entered date.");
                }

                var respExchangeRate     = Decimal.Parse(observations[0].GetProperty(conversionLabel).GetProperty("v").GetString());
                var respExchangeRateDate = Convert.ToDateTime(observations[0].GetProperty("d").GetString());

                // 4 decimal places precision
                var amountConverted = Math.Round(amountToConvert * respExchangeRate, 4);

                var currencyConversion = new CurrencyConversion(fromCurrencyIsoCode, toCurrencyIsoCode, exchangeRate : respExchangeRate, amountConverted, exchangeRateDate: respExchangeRateDate);

                return currencyConversion;

            }
            catch (WebException e)
            {
                // If this was an application to be shipped, we would use logger ( to a file or database) here, not a Console.WriteLine().
                #if DEBUG
                    Console.WriteLine("\r\nWebException Raised. The following error occurred : {0}", e.Status);
                #endif
                throw;
            }

            catch (Exception e)
            {
                // If this was an application to be shipped, we would use logger ( to a file or database) here, not a Console.WriteLine().
                #if DEBUG
                    Console.WriteLine("\nThe following Exception was raised : {0}", e.Message);
                #endif
                throw;
            }
        }
        /// <summary>
        /// Validates the parameters required for the currency conversion.
        /// </summary>
        /// <param name="foreignCurrency"></param>
        /// <param name="conversionType"></param>
        /// <param name="exchangeRateDate"></param>
        private void validateConversionParameters(string foreignCurrency, string conversionType, DateTime? exchangeRateDate)
        {
            if (String.IsNullOrWhiteSpace(foreignCurrency))
            {
                throw new ArgumentNullException(String.Format("Foreign currency cannot be empty.", foreignCurrency));
            }

            if (foreignCurrency == "CAD")
            {
                throw new ArgumentException(String.Format("Foreign currency cannot be CAD.", foreignCurrency));
            }

            var allCurrencyISOCodes = getAllCurrencyISOCodes();
            if (!allCurrencyISOCodes.Contains(foreignCurrency))
            {
                throw new ArgumentException(String.Format("{0} is not a valid ISO Code.", foreignCurrency));
            }

            if (String.IsNullOrWhiteSpace(conversionType))
            {
                throw new ArgumentNullException(String.Format("{0} Conversion Type cannot be blank.", conversionType));
            }

            if (conversionType.ToLower() != "from" && conversionType.ToLower() != "to")
            {
                throw new ArgumentException(String.Format("{0} is not a valid conversion type. Type should be 'from' or 'to'.", conversionType));
            }

            if (exchangeRateDate != null)
            {
                DateTime dateTimeNow = DateTime.Now;
                var exchangeRateDateStr = exchangeRateDate.Value.ToString("yyyy-MM-dd");

                if (exchangeRateDate > dateTimeNow)
                {
                    throw new ArgumentException(String.Format("The date entered {0} is in the future.", exchangeRateDateStr));
                }

                if (exchangeRateDate.Value.Year < 2017)
                {
                    throw new ArgumentException(String.Format("The date entered {0} is before 2017. Bank of Canada's currency history goes as far as 2017.", exchangeRateDateStr));
                }

                if ((exchangeRateDate.Value.DayOfWeek == DayOfWeek.Saturday) || (exchangeRateDate.Value.DayOfWeek == DayOfWeek.Sunday))
                {
                    throw new ArgumentException(String.Format("The date entered {0} is a not a weekday. Bank of Canada coversion rates are updated weekdays at 16:30 ET.", exchangeRateDateStr));
                }

                var exchangeRateUpdateTime = new TimeSpan(16, 30, 0);

                if (dateTimeNow.Date == exchangeRateDate.Value.Date && dateTimeNow.TimeOfDay < exchangeRateUpdateTime)
                {
                    throw new ArgumentException(String.Format("The exchange rate has not been updated today yet. Bank of Canada coversion rates are updated weekdays at 16:30 ET."));
                }
            }
        }

        /// <summary>
        /// Retrieve the list of all the currency codes  in the ISO 4217 format
        /// </summary>
        /// <returns>List of all the currency codes in the ISO 4217 format</returns>
        private List<String> getAllCurrencyISOCodes()
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Select(cultureInfo => cultureInfo.LCID).Distinct()
                .Select(id => new RegionInfo(id))
                .GroupBy(regionInfo => regionInfo.ISOCurrencySymbol)
                .Select(grouping => grouping.First())
                .Select(regionInfo => regionInfo.ISOCurrencySymbol).ToList();
        }

    }

}
