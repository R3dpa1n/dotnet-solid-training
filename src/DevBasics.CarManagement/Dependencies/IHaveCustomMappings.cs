using AutoMapper;

namespace DevBasics.CarManagement.Dependencies
{
    public interface IHaveCustomMappings
    {
        void CreateMappings(IMapperConfigurationExpression configuration);
    }
}
