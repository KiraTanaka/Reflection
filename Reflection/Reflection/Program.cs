using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Framework;
using System.IO;

namespace Application
{
    class Program
    {
        readonly static string pathToDll = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "DLL");
        public static List<Type> GetTypesByInterface<TInterface>(List<Assembly> listAssembly)
        {
            if (!typeof(TInterface).IsInterface)
                throw new ArgumentException("Type must be an interface");

            List<Type> listType = new List<Type>();
            listAssembly.ForEach(assembly=> 
                      listType.AddRange(assembly.GetExportedTypes()
                      .Where(x => x.GetInterface(typeof(TInterface).Name) != null )
                        .ToList()));
            return listType;
        }
        public static List<Assembly> LoadPlugin()
        {
            string[] files = Directory.GetFiles(pathToDll, "*.dll");
            List<Assembly> listAssembly = new List<Assembly>();
            foreach (var file in files)
            {
                listAssembly.Add(Assembly.LoadFrom(file));
            }
            return listAssembly;
        }
        static void Main(string[] args)
        {
            List<Assembly> listAssembly = LoadPlugin();
            List<Type> pluginTypes = GetTypesByInterface<IPlugin>(listAssembly);
            List<IPlugin> plugins = new List<IPlugin>();
            foreach (Type pluginType in pluginTypes)
            {
                IPlugin plugin = (IPlugin)Activator.CreateInstance(pluginType);
                plugins.Add(plugin);
            }
            Console.WriteLine("Available plugins:");
            plugins.ForEach(p => Console.WriteLine(p.Name));
            Console.ReadLine();
        }
    }
}
