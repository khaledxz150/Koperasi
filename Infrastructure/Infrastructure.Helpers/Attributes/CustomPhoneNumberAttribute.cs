using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Helpers.Attributes
{
    public class CustomPhoneNumberAttribute : ValidationAttribute
    {
        public readonly int _localizationKey;

        private static readonly Dictionary<string, string> PhoneRegexByCountry = new()
        {
            { "+962", @"^\+9627[789]\d{7}$" },  // Jordan: +9627XYYYYYYY
            { "+1", @"^\+1\d{10}$" }            // USA/Canada: +1XXXXXXXXXX
        };

        public CustomPhoneNumberAttribute(int localizationKey)
        {
            _localizationKey = localizationKey;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var memoryCache = (IMemoryCache)validationContext.GetService(typeof(IMemoryCache));
            var httpContext = new HttpContextAccessor().HttpContext;
            var languageID = httpContext?.Request.Headers["LanguageID"].FirstOrDefault() ?? "2";

            string errorMessage = "Invalid phone number";

            if (memoryCache.TryGetValue($"{languageID}-DictionaryLocalization", out Dictionary<int, string> localization))
            {
                if (localization.TryGetValue(_localizationKey, out var localizedMessage))
                {
                    errorMessage = localizedMessage;
                }
            }

            if (value == null || value is not string phoneNumber)
                return new ValidationResult(errorMessage);

            phoneNumber = phoneNumber.Trim();

            foreach (var (countryCode, pattern) in PhoneRegexByCountry)
            {
                if (phoneNumber.StartsWith(countryCode))
                {
                    if (Regex.IsMatch(phoneNumber, pattern))
                        return ValidationResult.Success;
                    else
                        return new ValidationResult(errorMessage);
                }
            }

            // No matching country code
            return new ValidationResult(errorMessage);
        }
    }
}
