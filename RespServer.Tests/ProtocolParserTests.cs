using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RespServer.Protocol;

namespace RespServer.Tests
{
    [TestFixture]
    public class ProtocolParserTests
    {
        private List<RespPart> ToParts(List<RespMarker> markers)
        {
            List<RespPart> ret = new List<RespPart>();
            foreach (var m in markers)
            {
                var body = new byte[m.Length];
                ret.Add(new RespPart(m, body));
            }
            return ret;
        }
        [TestCase()]
        public void TestStringArray()
        {
            List<RespMarker> markers = new List<RespMarker>();
            markers.Add(new RespMarker(RespMarker.MarkerType.Array, 3));
            markers.Add(new RespMarker(RespMarker.MarkerType.String, 1));
            markers.Add(new RespMarker(RespMarker.MarkerType.String, 1));
            markers.Add(new RespMarker(RespMarker.MarkerType.String, 1));
            var parts = ToParts(markers);

            RespParser parser = new RespParser();
            Assert.IsNull(parser.MessageHandle(parts[0]));
            Assert.IsNull(parser.MessageHandle(parts[1]));
            Assert.IsNull(parser.MessageHandle(parts[2]));
            var result = parser.MessageHandle(parts[3]);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result[0].GetType() == typeof(byte[]));
        }

        [TestCase()]
        public void TestNestedArray()
        {
            List<RespMarker> markers = new List<RespMarker>();
            markers.Add(new RespMarker(RespMarker.MarkerType.Array, 1));
            markers.Add(new RespMarker(RespMarker.MarkerType.Array, 3));
            markers.Add(new RespMarker(RespMarker.MarkerType.String, 1));
            markers.Add(new RespMarker(RespMarker.MarkerType.String, 1));
            markers.Add(new RespMarker(RespMarker.MarkerType.String, 1));
            var parts = ToParts(markers);

            RespParser parser = new RespParser();
            Assert.IsNull(parser.MessageHandle(parts[0]));
            Assert.IsNull(parser.MessageHandle(parts[1]));
            Assert.IsNull(parser.MessageHandle(parts[2]));
            Assert.IsNull(parser.MessageHandle(parts[3]));
            var result = parser.MessageHandle(parts[4]);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            var innerArray = result[0] as List<object>;
            Assert.IsNotNull(innerArray);
            Assert.AreEqual(3, innerArray.Count);
            Assert.IsTrue(innerArray[0].GetType() == typeof(byte[]));
        }

        [TestCase()]
        private void TestStringDeserialize1()
        {
            String input = "$10\r\n0123456789\r\n";


        }
        [TestCase()]
        private void TestStringDeserialize2()
        {
            String input = "$10\r\n0123456789";
        }
    }
}
