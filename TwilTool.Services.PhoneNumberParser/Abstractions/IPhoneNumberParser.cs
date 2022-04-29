using System;
using System.Collections.Generic;
using System.Text;

namespace TwilTool.Services.PhoneNumberParsing.Abstractions
{
    public interface IPhoneNumberParser
    {
        List<NumberParseResult> ParseMultipFromText(string text, bool supportInternational = false);
        NumberParseResult ParseFromText(string text, bool supportInternational = false);
    }
}
