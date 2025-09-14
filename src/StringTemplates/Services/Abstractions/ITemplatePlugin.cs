namespace StringTemplates.Services;

/// <summary>
/// Interface used internally to inject multiple string template plugins into a single <see cref="ITemplateService"/>.
/// </summary>
public interface ITemplatePlugin : ITemplateService;

/// <inheritdoc cref="ITemplatePlugin"/>
/// <typeparam name="TInput">Optional input parameter that can be used to provide context for value retrieval.</typeparam>
public interface ITemplatePlugin<in TInput> : ITemplateService<TInput> where TInput : class?;
