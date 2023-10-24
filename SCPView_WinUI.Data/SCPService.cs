using RestSharp;
using SCPView_WinUI.Data.Model;
using SCPView_WinUI.Data.Parser;
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
        public static RestClient GetClient(RestClientOptions options)
        {
            var client = new RestClient(options);
            return client;
        }

        /// <summary>
        /// 获取所有SCP列表
        /// </summary>
        /// <returns></returns>
        public static async Task<List<SCPMenuItem>> GetSeriesList()
        {
            List<SCPMenuItem> menuItems = new List<SCPMenuItem>();
            var options = new RestClientOptions(SCPUrl.HOST);
            var client = GetClient(options);
            try
            {
                var respone = await client.GetAsync(new RestRequest());
                if (respone.IsSuccessful)
                {
                    string body = respone.Content;
                    menuItems = SCPSeriesListParser.Parse(body);
                    return menuItems;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return new List<SCPMenuItem>();
        }

        /// <summary>
        /// 获取某个系列的列表
        /// </summary>
        /// <param name="listUrl">系列地址</param>
        /// <returns></returns>
        public static async Task<Dictionary<string, List<SCPItemList>>> GetItemList(string listUrl)
        {
            Dictionary<string, List<SCPItemList>> itemList = new Dictionary<string, List<SCPItemList>>();
            var options = new RestClientOptions(SCPUrl.REFERER);
            var client = GetClient(options);
            var request = new RestRequest(listUrl);
            try
            {
                var response = await client.GetAsync(request);
                if (response.IsSuccessful)
                {
                    string body = response.Content;
                    itemList = SCPListParser.Parse(body);
                    return itemList;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return itemList;
        }

        /// <summary>
        /// 获取指定SCP内容
        /// </summary>
        /// <param name="scpContentUrl">指定SCP的URL</param>
        /// <returns></returns>
        public static async Task<SCPItem> GetItemContent(string scpContentUrl)
        {
            var scpItem = new SCPItem();    
            var options = new RestClientOptions(SCPUrl.REFERER);
            
            var client = GetClient(options);
            var request = new RestRequest(scpContentUrl);
            try
            {
                var response = await client.GetAsync(request);
                if(response.IsSuccessful)
                {
                    string body = response.Content;
                    scpItem = SCPContentParser.Parse(body);
                    return scpItem;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return scpItem;
        }

        /// <summary>
        /// 获取001内容
        /// </summary>
        /// <param name="zzoUrl">001条目地址</param>
        /// <returns></returns>
        public static async Task<List<SCPSeries>> GetZZOList(string zzoUrl)
        {
            List<SCPSeries> seriesList = new List<SCPSeries>();
            var options = new RestClientOptions(SCPUrl.REFERER);
            var client = GetClient(options);
            var request = new RestRequest(zzoUrl);
            try
            {
                var response = await client.GetAsync(request);
                if(response.IsSuccessful)
                {
                    string body = response.Content;
                    seriesList = await SCPContentParser.ParseZZO(body);
                    return seriesList;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return seriesList;
        }

        public static async Task<SCPBanner> GetBanner()
        {
            SCPBanner banner = new SCPBanner();
            var options = new RestClientOptions(SCPUrl.REFERER);
            var client = GetClient(options);
            try
            {
                var responese = await client.GetAsync(new RestRequest());
                if(responese.IsSuccessful)
                {
                    string body = responese.Content;
                    banner = BannerParser.Parser(body);
                    return banner;
                }
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.Message);
            }

            return banner;
        }
    }
}
