using Element;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Element.Admin
{
    public class ResourceAccessor
    {
        public ResourceAccessor()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.IsDynamic)
                .SelectMany(x => x.ExportedTypes)
                .Where(x => x.IsClass)
                .ToArray();
            var enums = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.IsDynamic)
                .SelectMany(x => x.ExportedTypes)
                .Where(x => x.IsEnum)
                .ToArray();
            Resources = types.Select(x => x.GetCustomAttributes(false).OfType<ResourceAttribute>().FirstOrDefault()).Where(x => x != null).ToDictionary(x => x.Id, x => x.Name);
            var resourceEnumTypes = enums.Where(x => x.GetCustomAttributes(false).OfType<ResourcesAttribute>().Any()).ToArray();
            foreach (var resourceEnumType in resourceEnumTypes)
            {
                var names = Enum.GetNames(resourceEnumType);
                foreach (var name in names)
                {
                    if (Resources.ContainsKey(name))
                    {
                        continue;
                    }
                    var description = resourceEnumType.GetField(name).GetCustomAttributes(false).OfType<DescriptionAttribute>().FirstOrDefault()?.Description ?? name;
                    Resources.Add(name, description);
                }
            }
        }

        public Dictionary<string, string> Resources { get; }
    }
}
