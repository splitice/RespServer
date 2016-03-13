using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using RespServer.Protocol;

namespace RespServer.Tests
{
    [TestFixture]
    public class CommunicationTests
    {
        [TestCase]
        public void TestFirst()
        {
            String input = "*1\r\n$1\r\na\r\n";
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            var observableCollection = new Subject<byte>();

            RespCommunicator communicator = new RespCommunicator(observableCollection);
            int count = 0;
            List<object> value = null;
            communicator.MessageArrived += (sender, e) =>
            {
                Assert.IsNull(e.Exception);
                count++;
                value = e.Arguments;
            };

            communicator.Start();
            foreach (var b in bytes)
            {
                observableCollection.OnNext(b);
            }

            Assert.AreEqual(1,count);
            Assert.AreEqual(1,value.Count);
        }
    }
}
