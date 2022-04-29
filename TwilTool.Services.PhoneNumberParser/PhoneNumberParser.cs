using System;
using System.Collections.Generic;
using System.Text;

namespace TwilTool.Services.PhoneNumberParsing
{
    public class PhoneNumberParser : Abstractions.IPhoneNumberParser
    {
        public List<NumberParseResult> ParseMultipFromText(string text, bool supportInternational = false)
        {
            if (string.IsNullOrEmpty(text))
                return new List<NumberParseResult>();

            text = text
                  .Replace(",", ";")
                 .Replace(Environment.NewLine, ";")
                 .Replace("\r", ";")
                 .Replace("|", ";");

            var lines = text.Split(';');
            var results = new List<NumberParseResult>();

            foreach (var l in lines)
            {
                if (!string.IsNullOrEmpty(l))
                    results.Add(this.ParseFromText(l, supportInternational));
            }

            return results;
        }

        public NumberParseResult ParseFromText(string text, bool supportInternational = false)
        {
            if (string.IsNullOrEmpty(text))
                return new NumberParseResult() { Result = text };

            if (_cachedResults.ContainsKey(text))
                return _cachedResults[text];

            var result = new NumberParseResult();
            if (supportInternational)
            {

                result.IsValidNumber = true;
                result.Result = text;
            }
            else
            {
                //this validation logic only works for US number
                var phoneString = text
                    .Replace(" ", "")
                    .Replace("+", "")
                    .Replace(".", "")
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace("-", "");

                if (phoneString.Length == 11 && phoneString.StartsWith("1"))
                    phoneString = phoneString.Substring(startIndex: 1, length: 10);

                if (phoneString.Length == 10 && long.TryParse(phoneString, out var i))
                {
                    phoneString = phoneString.Insert(startIndex: 3, value: "-");
                    phoneString = phoneString.Insert(startIndex: 7, value: "-");

                    result.Result = phoneString;
                    result.IsValidNumber = true;
                }
                else
                {
                    result.Result = text;
                    result.IsValidNumber = false;
                }
            }

            _cachedResults.Add(key: text, value: result);

            return result;
        }

        //caching is for perfance when doing compares in like a linq collection query
        private Dictionary<string, NumberParseResult> _cachedResults = new Dictionary<string, NumberParseResult>();
    }
}
