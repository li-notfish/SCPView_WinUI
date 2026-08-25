using RestSharp;
using SCPView_WinUI.Data.Model;
using SCPView_WinUI.Data.Parser;
using SCPView_WinUI.Data.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPView_WinUI.Data
{
    public class SCPService
    {
        private static SCPDatabase? _database;

        private static SCPDatabase Database => _database ??= new SCPDatabase();

        public static RestClient GetClient(RestClientOptions options)
        {
            var client = new RestClient(options);
            return client;
        }

        private static async Task<T> WithRetry<T>(Func<Task<T>> action, int maxRetries = 3, int delayMs = 500)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    return await action();
                }
                catch when (i < maxRetries - 1)
                {
                    await Task.Delay(delayMs);
                }
            }
            return default!;
        }

        /// <summary>
        /// 获取所有SCP列表
        /// </summary>
        /// <returns></returns>
        public static async Task<List<SCPMenuItem>> GetSeriesList()
        {
            return await WithRetry(async () =>
            {
                var options = new RestClientOptions(SCPUrl.HOST);
                var client = GetClient(options);
                var respone = await client.GetAsync(new RestRequest());
                if (respone.IsSuccessful)
                {
                    string body = respone.Content;
                    return SCPSeriesListParser.Parse(body);
                }
                return new List<SCPMenuItem>();
            });
        }

        /// <summary>
        /// 获取某个系列的列表（优先从缓存读取）
        /// </summary>
        /// <param name="listUrl">系列地址</param>
        /// <returns></returns>
        public static async Task<Dictionary<string, List<SCPItemList>>> GetItemList(string listUrl)
        {
            try
            {
                var cached = Database.GetList(listUrl);
                if (cached != null) return cached;
            }
            catch { }

            var result = await WithRetry(async () =>
            {
                var options = new RestClientOptions(SCPUrl.REFERER);
                var client = GetClient(options);
                var request = new RestRequest(listUrl);
                var response = await client.GetAsync(request);
                if (response.IsSuccessful)
                {
                    string body = response.Content;
                    return SCPListParser.Parse(body);
                }
                return new Dictionary<string, List<SCPItemList>>();
            });

            if (result != null && result.Count > 0)
            {
                try { Database.SetList(listUrl, result); }
                catch { }
            }

            return result ?? new Dictionary<string, List<SCPItemList>>();
        }

        /// <summary>
        /// 获取指定SCP内容（优先从缓存读取）
        /// </summary>
        /// <param name="scpContentUrl">指定SCP的URL</param>
        /// <returns></returns>
        public static async Task<SCPItem> GetItemContent(string scpContentUrl)
        {
            try
            {
                var cached = Database.Get(scpContentUrl);
                if (cached != null) return cached;
            }
            catch { }

            var item = await WithRetry(async () =>
            {
                var options = new RestClientOptions(SCPUrl.REFERER);
                var client = GetClient(options);
                var request = new RestRequest(scpContentUrl);
                var response = await client.GetAsync(request);
                if (response.IsSuccessful)
                {
                    string body = response.Content;
                    return await SCPContentParser.ParseAsync(body);
                }
                return new SCPItem();
            });

            if (item != null && !string.IsNullOrEmpty(item.Name))
            {
                try { Database.Set(scpContentUrl, item); }
                catch { }
            }

            return item ?? new SCPItem();
        }

        /// <summary>
        /// 获取001内容
        /// </summary>
        /// <param name="zzoUrl">001条目地址</param>
        /// <returns></returns>
        public static async Task<List<SCPSeries>> GetZZOList(string zzoUrl)
        {
            return await WithRetry(async () =>
            {
                var options = new RestClientOptions(SCPUrl.REFERER);
                var client = GetClient(options);
                var request = new RestRequest(zzoUrl);
                var response = await client.GetAsync(request);
                if (response.IsSuccessful)
                {
                    string body = response.Content;
                    return await SCPContentParser.ParseZZO(body);
                }
                return new List<SCPSeries>();
            });
        }

        public static async Task<SCPBanner> GetBanner()
        {
            return await WithRetry(async () =>
            {
                var options = new RestClientOptions(SCPUrl.REFERER);
                var client = GetClient(options);
                var responese = await client.GetAsync(new RestRequest());
                if (responese.IsSuccessful)
                {
                    string body = responese.Content;
                    return BannerParser.Parser(body);
                }
                return new SCPBanner();
            });
        }

        public static void ClearCache()
        {
            Database.Clear();
        }

        public static int GetCacheCount()
        {
            return Database.Count();
        }
    }
}
