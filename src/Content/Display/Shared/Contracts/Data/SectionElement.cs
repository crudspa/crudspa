using System.Text.Json;
using System.Text.Json.Serialization;

namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class SectionElement : Observable, IValidates, IOrderable
{
    private Object? _config;
    private String? _configJson;

    public Element Element
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? ConfigType
    {
        get;
        set => SetProperty(ref field, value);
    }

    [JsonConverter(typeof(FragmentConverter))] public String? ConfigJson
    {
        get => _config?.ToJson() ?? _configJson;
        set
        {
            _config = null;
            SetProperty(ref _configJson, value);
        }
    }

    [JsonIgnore] public Object? Config => _config;

    public Guid? Id
    {
        get => Element.Id;
        set => Element.Id = value;
    }

    public Guid? ElementId
    {
        get => Element.ElementId;
        set => Element.ElementId = value;
    }

    public Guid? SectionId
    {
        get => Element.SectionId;
        set => Element.SectionId = value;
    }

    public Guid? TypeId
    {
        get => Element.TypeId;
        set => Element.TypeId = value;
    }

    public Boolean? RequireInteraction
    {
        get => Element.RequireInteraction;
        set => Element.RequireInteraction = value;
    }

    public Int32? Ordinal
    {
        get => Element.Ordinal;
        set => Element.Ordinal = value;
    }

    public ElementType? ElementType
    {
        get => Element.ElementType;
        set => Element.ElementType = value;
    }

    public Box Box
    {
        get => Element.Box;
        set => Element.Box = value;
    }

    public Item Item
    {
        get => Element.Item;
        set => Element.Item = value;
    }

    public void SetConfig<T>(T config) where T : class
    {
        _config = config;
        ConfigType = typeof(T).FullName;
        _configJson = config.ToJson();
    }

    public T? As<T>() where T : class
    {
        if (_config is T config)
            return config;

        if (ConfigJson.HasNothing())
            return null;

        var targetType = typeof(T);
        var targetTypeName = targetType.FullName;

        if (ConfigType.HasSomething() && !ConfigType!.IsBasically(targetTypeName))
            return null;

        _config = ConfigJson.FromJson<T>();
        return _config as T;
    }

    public T RequireConfig<T>() where T : class
    {
        if (As<T>() is { } config)
            return config;

        var typeName = typeof(T).Name;
        throw new($"Element '{Id}' is not configured as '{typeName}'.");
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            errors.AddRange(Element.Validate());

            if (_config is null && ConfigType.HasSomething() && ConfigJson.HasSomething())
            {
                var configType = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Select(x => x.GetType(ConfigType!))
                    .FirstOrDefault(x => x is not null);

                if (configType is not null)
                    _config = JsonSerializer.Deserialize(ConfigJson, configType);
            }

            if (Config is IValidates validates)
                errors.AddRange(validates.Validate());
        });
    }
}