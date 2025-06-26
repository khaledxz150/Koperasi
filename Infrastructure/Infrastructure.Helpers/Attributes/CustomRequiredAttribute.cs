using System.ComponentModel.DataAnnotations;

using Infrastructure.Extensions;
using Infrastructure.Helpers.HttpContext;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
namespace Infrastructure.Helpers.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class CustomRequiredAttribute : RequiredAttribute
{
    public readonly bool _isRequired;
    public readonly int _localizationKey;
    public readonly string[] _replacee;
    public readonly double? _Min;
    public readonly double? _Max;
    public readonly double? _minLength;
    public readonly double? _maxLength;

    /// <summary>
    /// Creates a JRequiredAttribute instance for property validation.
    /// </summary>
    /// <param name="isRequired">Indicates if the property is required.</param>
    /// <param name="localizationKey">The localization key for error message lookup.</param>
    public CustomRequiredAttribute(bool isRequired, int localizationKey, params string[] replacee )
    {
        _isRequired = isRequired;
        _localizationKey = localizationKey;
        _replacee = replacee;
    }

    public CustomRequiredAttribute(bool isRequired, int localizationKey, double min, double max, double minLength, double maxLength, params string[] replacee)
    {
        _isRequired = isRequired;
        _localizationKey = localizationKey;
        _replacee = replacee;
        _Min = min;
        _Max = max;
        _minLength = minLength;
        _maxLength = maxLength;
    }





    public CustomRequiredAttribute(bool isRequired)
    {
        _isRequired = isRequired;
    }

    public CustomRequiredAttribute(bool isRequired, int localizationKey)
    {
        _isRequired = isRequired;
        _localizationKey = localizationKey;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var memoryCache = (IMemoryCache)validationContext.GetService(typeof(IMemoryCache))!;
        var languageID = HTTPContextHelper.GetCurrentLanguageFromHeader();

        var validationWord = "";


        if (memoryCache.TryGetValue($"{languageID}-DictionaryLocalization", out Dictionary<int,string> localization))
        {
            if (localization.TryGetValue(_localizationKey, out var word))
            {
                validationWord = word;
            }
         
        }

        if (_isRequired)
        {
            if (value is not null || (value is string && value != ""))
            {
                // Check numeric range if applicable
                if (_Min.HasValue && _Max.HasValue && _Max != 0 &&
                    double.TryParse(value.ToString(), out double doubleValue))
                {
                    if (doubleValue < _Min || doubleValue > _Max)
                    {
                        return new ValidationResult(validationWord);
                    }
                }

                // Check string length range if applicable
                if (_minLength.HasValue && _minLength > 0 || _maxLength.HasValue && _maxLength > 0)
                {
                    var strValue = value.ToString();
                    if (_minLength.HasValue && _minLength > 0 && strValue.Length < _minLength)
                    {
                        if (_replacee.IsNotNullOrEmpty())
                        {
                            for (int i = 0; i < _replacee.Length; i++)
                            {
                                validationWord = validationWord.Replace($"{{{i}}}", _replacee[i]);
                            }
                        }
                        return new ValidationResult(validationWord);
                    }
                    if (_maxLength.HasValue && _maxLength > 0 && strValue.Length > _maxLength)
                    {
                        for (int i = 0; i < _replacee.Length; i++)
                        {
                            validationWord = validationWord.Replace($"{{{i}}}", _replacee[i]);
                        }
                        return new ValidationResult(validationWord);
                    }
                }

                return ValidationResult.Success;
            }
            //var wrapper = validationContext.GetRequiredService<GrpcServicesWrapper>();
            //var response = wrapper.DictionaryLocalizationClient.GetDictionaryLocalizationAsync(new DictionaryRequest()
            //{
            //    LstIDs = { _localizationKey },
            //    LstIDsLength = 1
            //}).GetAwaiter().GetResult();
            if (_replacee.IsNotNullOrEmpty())
            {
                for (int i = 0; i < _replacee.Length; i++)
                {
                    validationWord = validationWord.Replace($"{{{i}}}", _replacee[i]);
                }
            }
            return new ValidationResult(validationWord);
        }
        return ValidationResult.Success;



    }

}