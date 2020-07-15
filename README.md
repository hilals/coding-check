# Overview

This application is a currency converter which allows users to convert an amount from a foreign currency to the Canadian currency, or from the Canadian currency to the foreign currency. The user can use the latest exchange rate or choose a historical one by entering the date. Bank of Canada's exchange rates are used for the calculation.

## Solution Structure
The **CurrencyConverter.sln** contains the following projects:
* **ConsoleApp** : This is the startup project. It contains the console UI. It depends on **Services** and **Models**. Dependencies are configured in the **main**, since there is no Web Api project (so no **startup.cs**).
* **Services**: This is class library project that contains the services that are used for currency conversion. This is app, there is only one service, which is the **BocCurrencyConverter** (Boc = Bank of Canada). We can expand the application by adding for example a service for Bank of America conversion. This project depends on **Models**.
* **Models**: This contains the models that will be used across the application.
* **ServicesTests** : This project contains the unit tests for the **Services** project.

## Assumptions
The assumptions made are the following: 
* There is no need to expose the currency conversion service through an **API**, hence why a Web API project was not created. If we were to expose APIs, we would add a Web API project which would use the **Services** class library.

 
## Usage

The application is developed using .Net Core 3.1, so Visual Studio 2019 is required to open the solution.

To run the application, open the CurrencyConverter.sln solution with Visual Studio 2019 and click on the run button. This should open the console app. To avoid seeing some debug specific logging, run the application using **Release** environment.

The application will ask the user to enter the foreign currency, conversion type (from, to), the amount to convert, and the exchange rate date(optional). Then it will display the amount converted, exchange rate and exchange rate date. 

Here is how the console app looks in action: 

```
Please specify the foreign currency to convert to/from Canadian. Please enter the ISO 4217 Code (ex: USD, EUR, etc).
USD

Please enter 'from' if you wish to convert from USD to CAN, or 'to' if you wish to convert from CAN to USD.
from

Do you wish to use a specific conversion date? Enter yes or no
yes

Please enter the conversion date in the yyyy-MM-dd format. The date has to be a weekday starting 2017.
2020-07-10.

Please enter enter the amount that you wish to convert.
50

50.0000 USD is 67.9700 CAD.
Exchange rate is 1.3594
Exchange rate date is 2020-07-10

Do you wish to do another conversion? Type 'yes' to continue or 'no' to quit.
```

## Ideas for Improvements ##
Here are ways that we can improve the application:

* **Implement caching** : If a previous request was for the USD-CAN exchange rate on July 10th 2020, the next request does not need to hit the Bank of Canada api to get the exchange rate again. So it makes sense to cache the exchange rate and date. This will reduce considerably the time it takes for the service to return the conversion, since there is no need to hit the BOC api.
*  **Use a logger**: Since the services are a class library, we should be using a logger to log errors to a file or database. This will help in troubleshooting in the future.
* **Use DTO objects  as a parameter for services methods**: If we take a look at the method **GetCurrencyConvertedAmount** in the Services class ***BocCurrencyConverterSvc**, it accepts 4 parameters. If in the future, a decision is made to and new parameters have to be added to the method, things could get ugly. Imagine a method with 8 parameters... So using Data Transfer Objects is a good alternative to having many parameters. These objects would contain all the information required, and then they would get sent to the services. For example, we can create a CurrencyExchangeDTO, which contains all the fields required (ie foreign currency, conversion type, exchange amount, exchange date), and the method using it would look like **GetCurrencyConvertedAmount(CurrencyExchangeDTO currencyExchangeDTO).**
