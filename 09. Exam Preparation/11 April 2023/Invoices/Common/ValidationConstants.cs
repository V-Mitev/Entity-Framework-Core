namespace Invoices.Common
{
    public class ValidationConstants
    {
        // Product

        public const int ProductNameMinLength = 9;
        public const int ProductNameMaxLength = 30;

        public const string ProductPriceMinValue = "5.00";
        public const string ProductPriceMaxValue = "1000.00";

        public const int ProductCategoryTypeMinValue = 0;
        public const int ProductCategoryTypeMaxValue = 4;

        // Address

        public const int AddressStreetNameMinLength = 10;
        public const int AddressStreetNameMaxLength = 20;

        public const int AddressCityMinLength = 5;
        public const int AddressCityMaxLength = 15;

        public const int AddressCountryMinLength = 5;
        public const int AddressCountryMaxLength = 15;

        // Invoice 

        public const int InvoiceNumberMinValue = 1000000000;
        public const int InvoiceNumberMaxValue = 1500000000;

        public const int InvoiceCurrencyTypeMinValue = 0;
        public const int InvoiceCurrencyTypeMaxValue = 2;

        // Client

        public const int ClientNameMinLength = 10;
        public const int ClientNameMaxLength = 25;

        public const int ClientNumberVatMinValue = 10;
        public const int ClientNumberVatMaxValue = 15;
    }
}