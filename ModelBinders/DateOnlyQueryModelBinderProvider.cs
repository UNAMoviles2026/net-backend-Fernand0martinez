using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace reservations_api.ModelBinders;

public class DateOnlyQueryModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        if (context.Metadata.ModelType == typeof(DateOnly))
        {
            return new DateOnlyQueryModelBinder();
        }

        return null;
    }
}
