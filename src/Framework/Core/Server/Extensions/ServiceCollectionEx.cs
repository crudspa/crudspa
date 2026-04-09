namespace Crudspa.Framework.Core.Server.Extensions;

public static class ServiceCollectionEx
{
    extension(IServiceCollection services)
    {
        public void AddByTypeName<TService>(String typeName)
        {
            if (typeName.HasNothing())
                throw new ArgumentException("Type name is required.", nameof(typeName));

            var actualType = Type.GetType(typeName, throwOnError: false);

            if (actualType is null)
                throw new($"Type '{typeName}' could not be found.");

            if (!typeof(TService).IsAssignableFrom(actualType))
                throw new($"Type '{actualType.FullName}' does not implement '{typeof(TService).FullName}'.");

            services.AddSingleton(typeof(TService), actualType);
        }
    }
}