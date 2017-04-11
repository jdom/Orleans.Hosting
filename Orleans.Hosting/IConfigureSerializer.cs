namespace Orleans.Hosting
{
    public interface IConfigureSerializer
    {
        IExternalSerializer Serializer { get; set; }
    }

    public class JsonSerializer : IExternalSerializer
    {
        public bool IndentJson { get; set; }
    }

    public interface IExternalSerializer
    {
    }

    public static class ConfigureSerializerExtensions
    {
        /// <summary>Configures to use a specific serializer.</summary>
        public static IConfigureOptionsBuilder<TOptions> UseSerializer<TOptions>(this IConfigureOptionsBuilder<TOptions> optionsBuilder, IExternalSerializer serializer)
            where TOptions : class, IConfigureSerializer, new()
        {
            optionsBuilder.Configure(options => options.Serializer = serializer);
            return optionsBuilder;
        }

        /// <summary>Configures to use Json serializer with default options.</summary>
        public static IConfigureOptionsBuilder<TOptions> UseJson<TOptions>(this IConfigureOptionsBuilder<TOptions> optionsBuilder, bool indent = false)
            where TOptions : class, IConfigureSerializer, new()
        {
            return UseSerializer<TOptions>(optionsBuilder, new JsonSerializer() { IndentJson = indent });
        }
    }
}