using AutoMapper;

namespace Files.Application.Common.Mappings;

/// <summary>
/// Интерфейс для автоматического маппинга сущностей
/// Реализуйте этот интерфейс в DTO классах для автоматической регистрации маппингов
/// </summary>
/// <typeparam name="T">Тип источника (обычно сущность из Domain)</typeparam>
public interface IMapWith<T>
{
    /// <summary>
    /// Конфигурация маппинга
    /// </summary>
    void Mapping(Profile profile)
    {
        profile.CreateMap(typeof(T), GetType());
    }
}