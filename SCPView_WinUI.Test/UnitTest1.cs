using NUnit.Framework.Legacy;
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
            ClassicAssert.IsFalse(list.Count == 0);
        }

        [Test]
        public async Task Test2()
        {
            string url = "/scp-series-cn";
            var itemList = await SCPService.GetItemList(url);
            ClassicAssert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }
        [Test]
        public async Task Test3()
        {
            string url = "/scp-series-cn-2";
            var itemList = await SCPService.GetItemList(url);
            ClassicAssert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }
        [Test]
        public async Task Test4()
        {
            string url = "/scp-series-cn-3";
            var itemList = await SCPService.GetItemList(url);
            ClassicAssert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }
        [Test]
        public async Task Test5()
        {
            string url = "/scp-series";
            var itemList = await SCPService.GetItemList(url);
            ClassicAssert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }
        [Test]
        public async Task Test6()
        {
            string url = "/scp-series-2";
            var itemList = await SCPService.GetItemList(url);
            ClassicAssert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }
        [Test]
        public async Task Test7()
        {
            string url = "/scp-series-2";
            var itemList = await SCPService.GetItemList(url);
            ClassicAssert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }
        [Test]
        public async Task Test8()
        {
            string url = "/scp-series-3";
            var itemList = await SCPService.GetItemList(url);
            ClassicAssert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }
        [Test]
        public async Task Test9()
        {
            string url = "/scp-series-4";
            var itemList = await SCPService.GetItemList(url);
            ClassicAssert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }
        [Test]
        public async Task Test10()
        {
            string url = "/scp-series-5";
            var itemList = await SCPService.GetItemList(url);
            ClassicAssert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }
        [Test]
        public async Task Test11()
        {
            string url = "/scp-series-6";
            var itemList = await SCPService.GetItemList(url);
            ClassicAssert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }
        [Test]
        public async Task Test12()
        {
            string url = "/scp-series-7";
            var itemList = await SCPService.GetItemList(url);
            ClassicAssert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }
        [Test]
        public async Task Test13()
        {
            string url = "/scp-series-8";
            var itemList = await SCPService.GetItemList(url);
            ClassicAssert.IsFalse(itemList.Count == 0);
            Console.WriteLine(itemList.First().Key + itemList.First().Value.First().HrefName);
        }

        [Test]
        public async Task TestContent()
        {
            string url = "/scp-cn-002";
            var item = await SCPService.GetItemContent(url);
            ClassicAssert.IsNotNull(item);
            Console.WriteLine(item.Name + "\n" + item.Contents);
        }

        [Test]
        public async Task TestZZOContent()
        {
            string url = "/scp-cn-001";
            var list = await SCPService.GetZZOList(url);
            ClassicAssert.IsNotNull(list);
            Console.Write(list.Count());
        }

        [Test]
        public async Task TestSCP_CN_1064()
        {
            string url = "/scp-cn-1064";
            var item = await SCPService.GetItemContent(url);
            ClassicAssert.IsNotNull(item);
            ClassicAssert.AreEqual("Keter", item.SafeLevel);
            ClassicAssert.IsNotEmpty(item.SpecialMeasures);
            ClassicAssert.IsNotEmpty(item.Contents);
            Console.WriteLine($"Name: {item.Name}");
            Console.WriteLine($"SafeLevel: {item.SafeLevel}");
            Console.WriteLine($"SpecialMeasures length: {item.SpecialMeasures.Length}");
            Console.WriteLine($"Contents length: {item.Contents.Length}");
            Console.WriteLine($"CollapsibleContents: {item.CollapsibleContents.Count}");
            Console.WriteLine($"BlockQuoteContents: {item.BlockQuoteContents.Count}");
        }

        [Test]
        public async Task TestContestList()
        {
            string url = "/summer-contest-2026";
            var items = await SCPService.GetContestList(url);
            ClassicAssert.IsTrue(items.Count > 0);
            ClassicAssert.IsFalse(string.IsNullOrEmpty(items[0].Title));
            ClassicAssert.IsFalse(string.IsNullOrEmpty(items[0].Author));
            Console.WriteLine($"Total items: {items.Count}");
            Console.WriteLine($"First: {items[0].Title} - {items[0].Author}");
            Console.WriteLine($"Last: {items.Last().Title} - {items.Last().Author}");
        }
    }
}