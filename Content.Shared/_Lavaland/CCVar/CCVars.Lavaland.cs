// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Configuration;

// ReSharper disable once CheckNamespace
namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    /// <summary>
    ///     Should the Lavaland roundstart generation be enabled.
    /// </summary>
    public static readonly CVarDef<bool> LavalandEnabled =
        CVarDef.Create("lavaland.enabled", true, CVar.SERVERONLY);

    // RW start
    /// <summary>
    ///     Minimum player count required to spawn the InteQSizo ruin on Lavaland.
    /// </summary>
    public static readonly CVarDef<int> InteQSizoMinPlayers =
        CVarDef.Create("lavaland.inteq_sizo_min_players", 20, CVar.SERVERONLY);
    // RW end
}
