using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace TwilTool.Services.PhoneNumberParsingTests
{

    [TestClass]
    public class PhoneNumberParserTests
    {
        [TestMethod]
        [DataRow(";")]
        [DataRow("|")]
        [DataRow("\r\n")]
        [DataRow("\r")]
        public void successful_multiparse(string seperator)
        {

            //*************  arrange  ******************
            string s = $"(843) 519-4007{seperator}(843) 279-6430{seperator}(336) 539-5033{seperator}(843) 564-2279";

            var parser = new PhoneNumberParsing.PhoneNumberParser();

            //*************    act    ******************
            var results = parser.ParseMultipFromText(s);

            //*************  assert   ******************
            results.TrueForAll(r => r.IsValidNumber);
            results.Count.Should().Be(4);
            results[0].Result.Should().Be("843-519-4007");
            results[1].Result.Should().Be("843-279-6430");
            results[2].Result.Should().Be("336-539-5033");
            results[3].Result.Should().Be("843-564-2279");

        }

        [TestMethod]
        public void ensure_newlinefeed_is_replaced_before_partial_newline()
        {

            //*************  arrange  ******************
            string s = $"(843) 519-4007\r\n(843) 279-6430\r(336) 539 5033\r\n(843) 564-2279";

            var parser = new PhoneNumberParsing.PhoneNumberParser();

            //*************    act    ******************
            var results = parser.ParseMultipFromText(s);

            //*************  assert   ******************
            results.TrueForAll(r => r.IsValidNumber);
            results.Count.Should().Be(4, because: "If you don't replace full newline first, replacing \\r will leave the \\n all alone");
            results[0].Result.Should().Be("843-519-4007");
            results[1].Result.Should().Be("843-279-6430");
            results[2].Result.Should().Be("336-539-5033");
            results[3].Result.Should().Be("843-564-2279");
        }

        [TestMethod]
        [DataRow("(843) 519-4007")]
        [DataRow("(843)519-4007")]
        [DataRow("843.519.4007")]
        [DataRow("+1 843-519-4007")]
        [DataRow("+1843-519-4007")]
        [DataRow("843 519 4007")]
        [DataRow("843-519-4007")]
        public void successful_parse(string input)
        {
            //*************  arrange  ******************
            var parser = new PhoneNumberParsing.PhoneNumberParser();

            //*************    act    ******************
            var result = parser.ParseFromText(input);

            //*************  assert   ******************
            result.IsValidNumber.Should().BeTrue();
            result.Result.Should().Be("843-519-4007");

        }
    }
}
