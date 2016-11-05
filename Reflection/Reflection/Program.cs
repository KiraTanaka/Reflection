using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Framework;
using System.IO;
using System.Linq.Expressions;

namespace Application
{
    class Program
    {
        readonly static string pathToDll = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "DLL");
        static void Main(string[] args)
        {
            List<IPlugin> plugins = new List<IPlugin>();
            Console.WriteLine("Available plugins:");
            Directory.GetFiles(pathToDll, "*.dll").ToList().ForEach(file =>
              Assembly.LoadFrom(file).GetExportedTypes()
                .Where(x => x.GetInterface(typeof(IPlugin).Name) != null & x.GetConstructor(Type.EmptyTypes) != null)
                .ToList().ForEach(pluginType =>
                {
                    plugins.Add((IPlugin)Activator.CreateInstance(pluginType));
                    Console.WriteLine(plugins.Last().Name);
                }));
            Console.ReadLine();
        }
    }
}
