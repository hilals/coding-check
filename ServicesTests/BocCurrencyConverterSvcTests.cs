using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Services.Tests
{
    [TestClass()]
    public class BocCurrencyConverterSvcTests
    {
        private static IBocCurrencyConverterSvc _bocCurrencyConverterService;
        public BocCurrencyConverterSvcTests()
        {
            var services = new ServiceCollection();

            services.AddServicesConnector();
            services.AddHttpClient<IBocCurrencyConverterSvc, BocCurrencyConverterSvc>();

            var provider = services.BuildServiceProvider();

            _bocCurrencyConverterService = provider.GetService<IBocCurrencyConverterSvc>();
        }
        [TestMethod()]
        public void GetCurrencyConvertedAmountTest_ReturnsConvertedFromAmountAtDate()
        {
            Task.Run(async () =>
            {
                var conversion = await _bocCurrencyConverterService.GetCurrencyConvertedAmount("USD", "from", 50M, DateTime.Parse("2020-07-10"));
                Assert.AreEqual(conversion.FromCurrencyIsoCode, "USD");
                Assert.AreEqual(conversion.ToCurrencyIsoCode, "CAD");
                Assert.AreEqual(conversion.ExchangeRate, 1.3594M);
                Assert.AreEqual(conversion.AmountConverted, 67.9700M);
                Assert.AreEqual(conversion.ExchangeRateDate.ToString("yyyy-MM-dd"), "2020-07-10");
            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void GetCurrencyConvertedAmountTest_ReturnsConvertedFromAmountNoDateProvided()
        {
            Task.Run(async () =>
            {
                var conversion = await _bocCurrencyConverterService.GetCurrencyConvertedAmount("USD", "from", 50.85M);
                Assert.AreEqual(conversion.FromCurrencyIsoCode, "USD");
                Assert.AreEqual(conversion.ToCurrencyIsoCode, "CAD");
                Assert.AreEqual(conversion.AmountConverted, Math.Round(50.85M * conversion.ExchangeRate, 4));
            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void GetCurrencyConvertedAmountTest_ReturnsConvertedToAmount()
        {
            Task.Run(async () =>
            {
                var conversion = await _bocCurrencyConverterService.GetCurrencyConvertedAmount("USD", "to", 50M, DateTime.Parse("2020-07-10"));
                Assert.AreEqual(conversion.FromCurrencyIsoCode, "CAD");
                Assert.AreEqual(conversion.ToCurrencyIsoCode, "USD");
                Assert.AreEqual(conversion.ExchangeRate, 0.7356M);
                Assert.AreEqual(conversion.AmountConverted, 36.7800M);
                Assert.AreEqual(conversion.ExchangeRateDate.ToString("yyyy-MM-dd"), "2020-07-10");
            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void GetCurrencyConvertedAmountTest_FutureDateThrowsArugmentException()
        {
            Task.Run(async () =>
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _bocCurrencyConverterService.GetCurrencyConvertedAmount(foreignCurrency : "USD", conversionType: "to", amountToConvert: 50M, exchangeRateDate: DateTime.Parse("2099-09-09")));
            }).GetAwaiter().GetResult();
        }
        [TestMethod()]
        public void GetCurrencyConvertedAmountTest_DateBefore2017ThrowsArugmentException()
        {
            Task.Run(async () =>
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _bocCurrencyConverterService.GetCurrencyConvertedAmount(foreignCurrency: "USD", conversionType: "to", amountToConvert: 50M, exchangeRateDate: DateTime.Parse("2016-09-09")));
            }).GetAwaiter().GetResult();
        }
        [TestMethod()]
        public void GetCurrencyConvertedAmountTest_DateOnWeekendThrowsArugmentException()
        {
            Task.Run(async () =>
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _bocCurrencyConverterService.GetCurrencyConvertedAmount(foreignCurrency: "USD", conversionType: "to", amountToConvert: 50M, exchangeRateDate: DateTime.Parse("2020-07-11")));
            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void GetCurrencyConvertedAmountTest_InvalidConversionTypeThrowsArugmentException()
        {
            Task.Run(async () =>
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _bocCurrencyConverterService.GetCurrencyConvertedAmount(foreignCurrency: "USD", conversionType: "abcd", amountToConvert: 50M, exchangeRateDate: DateTime.Parse("2020-07-10")));
            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void GetCurrencyConvertedAmountTest_InvalidCurrencyIsoCodeThrowsArugmentException()
        {
            Task.Run(async () =>
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _bocCurrencyConverterService.GetCurrencyConvertedAmount(foreignCurrency: "asdsad", conversionType: "to", amountToConvert: 50M, exchangeRateDate: DateTime.Parse("2020-07-10")));
            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void GetCurrencyConvertedAmountTest_CADForeignCurrencyThrowsArugmentException()
        {
            Task.Run(async () =>
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _bocCurrencyConverterService.GetCurrencyConvertedAmount(foreignCurrency: "CAD", conversionType: "to", amountToConvert: 50M, exchangeRateDate: DateTime.Parse("2020-07-10")));
            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void GetCurrencyConvertedAmountTest_ConversionTypeNotToOrFromThrowsArugmentException()
        {
            Task.Run(async () =>
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _bocCurrencyConverterService.GetCurrencyConvertedAmount(foreignCurrency: "USD", conversionType: "abcd", amountToConvert: 50M, exchangeRateDate: DateTime.Parse("2020-07-10")));
            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void GetCurrencyConvertedAmountTest_NoForeignCurrencyThrowsArugmentNullException()
        {
            Task.Run(async () =>
            {
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await _bocCurrencyConverterService.GetCurrencyConvertedAmount(foreignCurrency: null, conversionType: "to", amountToConvert: 50M, exchangeRateDate: DateTime.Parse("2020-07-10")));
            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void GetCurrencyConvertedAmountTest_NoConversionTypeThrowsArugmentNullException()
        {
            Task.Run(async () =>
            {
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await _bocCurrencyConverterService.GetCurrencyConvertedAmount(foreignCurrency: "USD", conversionType: null, amountToConvert: 50M, exchangeRateDate: DateTime.Parse("2020-07-10")));
            }).GetAwaiter().GetResult();
        }
    }
}