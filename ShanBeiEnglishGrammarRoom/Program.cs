using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication1;

namespace ShanBeiEnglishGrammarRoom
{
    class Program
    {
        static void Main(string[] args)
        {
            GetUrlHelper getUrlHelper = new GetUrlHelper();
            var task = getUrlHelper.StartAsync();
            task.Wait();


            Console.WriteLine("task completed");
            Console.ReadLine();
        }
    }
}
