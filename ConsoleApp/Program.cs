using Microsoft.Extensions.DependencyInjection;
using Services;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        private static IBocCurrencyConverterSvc _bocCurrencyConverterService ;
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
           
            services.AddServicesConnector();
            services.AddHttpClient<IBocCurrencyConverterSvc, BocCurrencyConverterSvc>();

            var provider = services.BuildServiceProvider();

            _bocCurrencyConverterService = provider.GetService<IBocCurrencyConverterSvc>();

            await runApp();
        }

        /// <summary>
        /// Displas the UI, uses the Banq of Canada currency converter service to do currency conversions, and then display the result.
        /// </summary>
        public static async Task runApp()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("\nPlease specify the foreign currency to convert to/from Canadian. Please enter the ISO 4217 Code (ex: USD, EUR, etc).");
                    var foreignCurrency = Console.ReadLine().ToUpper();

                    while (string.IsNullOrEmpty(foreignCurrency))
                    {
                        Console.WriteLine("\nThe foreign currency can't be empty. Please enter the foreign currency iso code.");
                        foreignCurrency = Console.ReadLine();
                    }

                    Console.WriteLine("\nPlease enter 'from' if you wish to convert from {0} to CAN, or 'to' if you wish to convert from CAN to {0}.", foreignCurrency);
                    var conversionType = Console.ReadLine();

                    while (string.IsNullOrEmpty(conversionType) || (conversionType.ToLower() != "from" && conversionType.ToLower() != "to"))
                    {
                        Console.WriteLine("\nInvalid conversion type. Please enter 'from' or 'to'.");
                        conversionType = Console.ReadLine();
                    }

                    Console.WriteLine("\nDo you wish to use a specific conversion date? Enter yes or no");
                    var wantsSpecificDate = Console.ReadLine();

                    while (string.IsNullOrEmpty(wantsSpecificDate) || (wantsSpecificDate.ToLower() != "yes" && wantsSpecificDate.ToLower() != "no"))
                    {
                        Console.WriteLine("\nInvalid entry. Please enter 'yes' if you wish to use a specific conversion date or 'no' if you dop wish to use to specify one.");
                        wantsSpecificDate = Console.ReadLine();
                    }

                    string exchangeRateDateStr = null;
                    DateTime dt;
                    if (wantsSpecificDate.ToLower() == "yes")
                    {
                        Console.WriteLine("\nPlease enter the conversion date in the yyyy-MM-dd format. The date has to be a weekday starting 2017.");
                        exchangeRateDateStr = Console.ReadLine();

                        while (string.IsNullOrEmpty(exchangeRateDateStr) || !DateTime.TryParseExact(exchangeRateDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            Console.WriteLine("\nThe date is invalid. Please enter a valid date in the yyyy-MM-dd format.");
                            exchangeRateDateStr = Console.ReadLine();
                        }
                    }

                    Console.WriteLine("\nPlease enter enter the amount that you wish to convert.");
                    var amountToConvertInput = Console.ReadLine();
                    Regex regex = new Regex(@"^[-+]?[0-9]*\.?[0-9]+$");

                    while (string.IsNullOrEmpty(amountToConvertInput) || !regex.IsMatch(amountToConvertInput))
                    {
                        Console.WriteLine("\nInvalid input, please enter a numeric amount");
                        amountToConvertInput = Console.ReadLine();
                    }

                    var amountToConvert  = Decimal.Parse(amountToConvertInput);

                    var currencyConversion = String.IsNullOrWhiteSpace(exchangeRateDateStr) ? await _bocCurrencyConverterService.GetCurrencyConvertedAmount(foreignCurrency, conversionType, amountToConvert)
                                                                                         : await _bocCurrencyConverterService.GetCurrencyConvertedAmount(foreignCurrency, conversionType, amountToConvert, exchangeRateDate: Convert.ToDateTime(exchangeRateDateStr));

                    Console.WriteLine("\n{0} {1} is {2} {3}.", Math.Round(amountToConvert, 4).ToString("N4"), currencyConversion.FromCurrencyIsoCode, currencyConversion.AmountConverted, currencyConversion.ToCurrencyIsoCode);
                    Console.WriteLine("Exchange rate is {0}", currencyConversion.ExchangeRate);
                    Console.WriteLine("Exchange rate date is {0}", currencyConversion.ExchangeRateDate.ToString("yyyy-MM-dd"));


                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("\n Error : {0}", e.Message);
                }

                catch (Exception)
                {
                    Console.WriteLine("\n An error has occurred, please contact the administration");
                }

                Console.WriteLine("\nDo you wish to do another conversion? Type 'yes' to continue or 'no' to quit.");

                var shouldContinue = Console.ReadLine();

                if (shouldContinue.ToLower() != "yes")
                {
                    break;
                }

            }
        }

    }
}
