using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    /// <summary>
    /// Gemini API Key for Advanced pAI.
    /// </summary>
    public static readonly CVarDef<string> GeminiApiKey =
        CVarDef.Create("gemini.api_key", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    /// Gemini endpoint URL.
    /// </summary>
    public static readonly CVarDef<string> GeminiApiUrl =
        CVarDef.Create("gemini.api_url", "https://generativelanguage.googleapis.com/v1beta", CVar.SERVERONLY);

    /// <summary>
    /// Model to use for Advanced pAI.
    /// </summary>
    public static readonly CVarDef<string> GeminiModel =
        CVarDef.Create("gemini.model", "", CVar.SERVERONLY);

    /// <summary>
    /// Max tokens for Gemini response.
    /// </summary>
    public static readonly CVarDef<int> GeminiMaxTokens =
        CVarDef.Create("gemini.max_tokens", 300, CVar.SERVERONLY);

    /// <summary>
    /// Temperature for Gemini response.
    /// </summary>
    public static readonly CVarDef<float> GeminiTemperature =
        CVarDef.Create("gemini.temperature", 0.7f, CVar.SERVERONLY);

    /// <summary>
    /// Thinking budget for Gemini response. Set to 0 to disable thinking for lower latency.
    /// </summary>
    public static readonly CVarDef<int> GeminiThinkingBudget =
        CVarDef.Create("gemini.thinking_budget", 0, CVar.SERVERONLY);
}

