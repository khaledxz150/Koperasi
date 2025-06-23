using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Helpers.Attributes
{
    public class CustomEmailAddressAttribute : ValidationAttribute
    {
        public readonly int _localizationKey;

        public CustomEmailAddressAttribute(int localizationKey)
        {
            _localizationKey = localizationKey;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var memoryCache = (IMemoryCache)validationContext.GetService(typeof(IMemoryCache));
            var languageID = new HttpContextAccessor().HttpContext.Request.Headers["LanguageID"].Single();

            var validationWord = "";


            if (memoryCache.TryGetValue($"{languageID}-DictionaryLocalization", out Dictionary<int, string> localization))
            {
                if (localization.TryGetValue(_localizationKey, out var word))
                {
                    validationWord = word;
                }

            }
            if (value == null)
            {
                return new ValidationResult(validationWord);
            }

            if (!(value is string valueAsString))
            {
                return new ValidationResult(validationWord);
            }

            if (valueAsString.AsSpan().ContainsAny('\r', '\n'))
            {
                return new ValidationResult(validationWord);
            }

            // only return true if there is only 1 '@' character
            // and it is neither the first nor the last character
            int index = valueAsString.IndexOf('@');

            if (index > 0 &&
                index != valueAsString.Length - 1 &&
                index == valueAsString.LastIndexOf('@'))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(validationWord);

        }
    }

}
