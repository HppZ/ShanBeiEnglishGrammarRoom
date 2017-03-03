using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;

namespace ConsoleApplication1
{
    class GetUrlHelper
    {
        private string baseUrl = "https://www.shanbay.com/footprints/category/34/?page={0}";
        private string baseUrl2 = "https://www.shanbay.com";

        private List<Item> urls = new List<Item>();
        private HttpClient client;

        public async Task StartAsync()
        {
            Init();
            await Start();
        }

        private void Init()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task Start()
        {
            for (int i = 1; i <= 7; i++)
            {
                var list = await GetItemInPage(i);
                foreach (var item in list)
                {
                    urls.Add(item);
                }
            }

            urls.Sort((x, y) =>
            {
                if (GetNum(x.Title) < GetNum(y.Title))
                    return -1;

                return 1;
            });

            //var c = urls.Count;
            //for (int i = 0; i < c; i++)
            //{
            //    var item = urls[i];
            //    Debug.WriteLine(i + " " + item.Title);
            //}

            Save(urls);
        }

        private void Save(IEnumerable<Item> list)
        {
            foreach (var item in list)
            {
                Debug.WriteLine(item.Html);
            }
        }

        private int GetNum(string x)
        {
            var regex = new Regex(@"[0-9]{1,}"); // nums
            var matches = regex.Matches(x);
            var match = matches[0];
            var num = int.Parse(x.Substring(match.Index, match.Length));
            return num;
        }

        private async Task<List<Item>> GetItemInPage(int page)
        {
            var result = new List<Item>();
            var url = string.Format(baseUrl, page);
            var html = await Get(url);

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var nodes = htmlDocument.DocumentNode.SelectNodes("//*[contains(@class,'well article-entry')]");

            foreach (var node in nodes)
            {
                var a = node.SelectSingleNode("a");
                var link = a.Attributes["href"].Value;
                var title = a.SelectSingleNode("h4").InnerText;
                a.Attributes["href"].Value = baseUrl2 + link;
                result.Add(new Item(title, link, a.OuterHtml));
            }

            return result;
        }

        private async Task<string> Get(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }

            return null;
        }

    }

    class Item
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Html { get; set; }
        public Item(string title, string url, string html)
        {
            Title = title;
            Url = url;
            Html = html;
        }
    }

}
