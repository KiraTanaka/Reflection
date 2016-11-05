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
        public static List<Type> LoadTypesPlugins()
        {
            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(pathToDll, "*.dll"));
            List<Type> listType = new List<Type>();
            files.ForEach(file =>
                      listType.AddRange(Assembly.LoadFrom(file).GetExportedTypes()
                      .Where(x => x.GetInterface(typeof(IPlugin).Name) != null & x.GetConstructor(Type.EmptyTypes) != null)
                        .ToList()));
            return listType;
        }
        static void Main(string[] args)
        {
            List<Type> pluginTypes = LoadTypesPlugins();
            List<IPlugin> plugins = new List<IPlugin>();
            Console.WriteLine("Available plugins:");
            foreach (Type pluginType in pluginTypes)
            {
                IPlugin plugin = (IPlugin)Activator.CreateInstance(pluginType);
                plugins.Add(plugin);
                Console.WriteLine(plugin.Name);
            }
            Console.ReadLine();
        }
    }
}
