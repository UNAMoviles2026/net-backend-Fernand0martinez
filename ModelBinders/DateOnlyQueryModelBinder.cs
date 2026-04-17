using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace reservations_api.ModelBinders;

public sealed class DateOnlyQueryModelBinder : IModelBinder
{
  private const string ExpectedFormat = "yyyy-MM-dd";

  public Task BindModelAsync(ModelBindingContext bindingContext)
  {
    ArgumentNullException.ThrowIfNull(bindingContext);

    var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
    if (valueResult == ValueProviderResult.None)
    {
      bindingContext.ModelState.TryAddModelError(
          bindingContext.ModelName,
          $"The '{bindingContext.ModelName}' query parameter is required.");
      return Task.CompletedTask;
    }

    bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueResult);

    var rawValue = valueResult.FirstValue;
    if (string.IsNullOrWhiteSpace(rawValue))
    {
      bindingContext.ModelState.TryAddModelError(
          bindingContext.ModelName,
          $"The '{bindingContext.ModelName}' query parameter is required.");
      return Task.CompletedTask;
    }

    var isValid = DateOnly.TryParseExact(
        rawValue,
        ExpectedFormat,
        CultureInfo.InvariantCulture,
        DateTimeStyles.None,
        out var parsedDate);

    if (!isValid)
    {
      bindingContext.ModelState.TryAddModelError(
          bindingContext.ModelName,
          $"Invalid date format. Use {ExpectedFormat}.");
      return Task.CompletedTask;
    }

    bindingContext.Result = ModelBindingResult.Success(parsedDate);
    return Task.CompletedTask;
  }
}
