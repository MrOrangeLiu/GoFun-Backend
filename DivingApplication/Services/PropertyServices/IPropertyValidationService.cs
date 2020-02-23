namespace DivingApplication.Services.PropertyServices
{
    public interface IPropertyValidationService
    {
        bool HasValidProperties<T>(string fields);
    }
}