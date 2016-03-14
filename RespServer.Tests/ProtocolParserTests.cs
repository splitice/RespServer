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
            return markers.Select(ToPart).ToList();
        }

        private RespPart ToPart(RespMarker marker)
        {
            byte[] body;
            if (marker.Type == RespMarker.MarkerType.String || marker.Type == RespMarker.MarkerType.SimpleString)
            {
                body = new byte[marker.Length];
            }
            else if (marker.Type == RespMarker.MarkerType.Integer)
            {
                body = Encoding.ASCII.GetBytes("1");
            }
            else if (marker.Type == RespMarker.MarkerType.Array)
            {
                body = new byte[]{};
            }
            else
            {
                throw new Exception("No ToParts definition of type");
            }
            return new RespPart(marker, body);
        }

        [TestCase()]
        public void TestSimpleStringArray()
        {
            List<RespMarker> markers = new List<RespMarker>();
            markers.Add(new RespMarker(RespMarker.MarkerType.Array, 3));
            markers.Add(new RespMarker(RespMarker.MarkerType.SimpleString, 1));
            markers.Add(new RespMarker(RespMarker.MarkerType.SimpleString, 1));
            markers.Add(new RespMarker(RespMarker.MarkerType.SimpleString, 1));
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
        public void TestIntArray()
        {
            List<RespPart> parts = new List<RespPart>();
            parts.Add(ToPart(new RespMarker(RespMarker.MarkerType.Array, 3)));
            parts.Add(ToPart(new RespMarker(RespMarker.MarkerType.Integer, 0)));
            parts.Add(ToPart(new RespMarker(RespMarker.MarkerType.Integer, 0)));
            parts.Add(ToPart(new RespMarker(RespMarker.MarkerType.Integer, 0)));

            RespParser parser = new RespParser();
            Assert.IsNull(parser.MessageHandle(parts[0]));
            Assert.IsNull(parser.MessageHandle(parts[1]));
            Assert.IsNull(parser.MessageHandle(parts[2]));
            var result = parser.MessageHandle(parts[3]);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result[0] is int);
            Assert.AreEqual(1,result[0]);
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
    }
}
