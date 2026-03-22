using System.Reflection;
using AutoMapper;

namespace Files.Application.Common.Mappings;

public class AssemblyMappingProfile : Profile
{
    public AssemblyMappingProfile(Assembly assembly)
    {
        ApplyMappingsFromAssembly(assembly);
    }

    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        // Получаем все типы, реализующие IMapWith<>
        var types = assembly.GetExportedTypes()
            .Where(type => type.GetInterfaces()
                .Any(i => i.IsGenericType &&
                          i.GetGenericTypeDefinition() == typeof(IMapWith<>)))
            .ToList();

        foreach (var type in types)
        {
            try
            {
                var instance = Activator.CreateInstance(type);
                var methodInfo = type.GetMethod("Mapping");

                if (methodInfo != null && instance != null)
                {
                    methodInfo.Invoke(instance, new object[] { this });
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку, не прерывая выполнение
                Console.WriteLine($"Ошибка при регистрации маппинга для типа {type.Name}: {ex.Message}");
            }
        }
    }
}