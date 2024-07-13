namespace Beaglepuss;

/// <summary>
/// Just in case the addons need more than just the configuration.
/// </summary>
/// <param name="Config"></param>
public record struct PluginData(
    Configuration Config,
    StringManager<StringIdentifier> StringManager);