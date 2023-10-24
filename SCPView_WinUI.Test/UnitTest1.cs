using SCPView_WinUI.Data;

namespace SCPView_WinUI.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Test1()
        {
            var list = await SCPService.GetSeriesList();
            Console.Write(list.First().Name);
            Assert.IsFalse(list.Count == 0);
        }

        [Test]
        public async Task Test2()
        {
            string url = "/scp-series-cn";
            var itemList = await SCPService.GetItemList(url);
            Assert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }
        [Test]
        public async Task Test3()
        {
            string url = "/scp-series-cn-2";
            var itemList = await SCPService.GetItemList(url);
            Assert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }
        [Test]
        public async Task Test4()
        {
            string url = "/scp-series-cn-3";
            var itemList = await SCPService.GetItemList(url);
            Assert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }
        [Test]
        public async Task Test5()
        {
            string url = "/scp-series";
            var itemList = await SCPService.GetItemList(url);
            Assert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }
        [Test]
        public async Task Test6()
        {
            string url = "/scp-series-2";
            var itemList = await SCPService.GetItemList(url);
            Assert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }
        [Test]
        public async Task Test7()
        {
            string url = "/scp-series-2";
            var itemList = await SCPService.GetItemList(url);
            Assert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }
        [Test]
        public async Task Test8()
        {
            string url = "/scp-series-3";
            var itemList = await SCPService.GetItemList(url);
            Assert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }
        [Test]
        public async Task Test9()
        {
            string url = "/scp-series-4";
            var itemList = await SCPService.GetItemList(url);
            Assert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }
        [Test]
        public async Task Test10()
        {
            string url = "/scp-series-5";
            var itemList = await SCPService.GetItemList(url);
            Assert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }
        [Test]
        public async Task Test11()
        {
            string url = "/scp-series-6";
            var itemList = await SCPService.GetItemList(url);
            Assert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }
        [Test]
        public async Task Test12()
        {
            string url = "/scp-series-7";
            var itemList = await SCPService.GetItemList(url);
            Assert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }
        [Test]
        public async Task Test13()
        {
            string url = "/scp-series-8";
            var itemList = await SCPService.GetItemList(url);
            Assert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }

        [Test]
        public async Task TestContent()
        {
            string url = "/scp-cn-002";
            var item = await SCPService.GetItemContent(url);
            Assert.IsNotNull(item);
            Console.WriteLine(item.Name + "\n" + item.Contents);
        }

        [Test]
        public async Task TestZZOContent()
        {
            string url = "/scp-cn-001";
            var list = await SCPService.GetZZOList(url);
            Assert.IsNotNull(list);
            Console.Write(list.Count());
        }
    }
}