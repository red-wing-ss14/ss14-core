namespace Content.Server._RW.PAI;

/// <summary>
///     Component for Advanced Personal AI devices powered by Gemini API.
/// </summary>
[RegisterComponent]
public sealed partial class AdvancedPAIComponent : Component
{
    /// <summary>
    ///     Whether the AI assistant is activated.
    /// </summary>
    [DataField]
    public bool Activated = false;

    /// <summary>
    ///     The name of the assistant selected by the user.
    /// </summary>
    [DataField]
    public string AssistantName = string.Empty;

    /// <summary>
    ///     The user who last activated or held the pAI device.
    /// </summary>
    [DataField]
    public EntityUid? LastUser;

    /// <summary>
    ///     Whether the AI is currently processing an API request.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public bool Processing = false;
}
