Run the regression harness with `dotnet run --project tests/RegressionTests.csproj -c Release` using .NET 10.

The harness executes the production plugin hooks and snapshot merge against managed game stand-ins. It covers the reported Manor/shop/lobby/Arctic route, repeated snapshots, omitted delta dictionaries, zero entries stripped by the host, direct purchases, a second shared upgrade, all 13 stat mappings, other players, authority guards, 0% probability, nonpositive events, and session resets.

The stand-ins do not emulate Unity avatar initialization or Photon transport. Before release, install the candidate on both host and guest, set strength sharing to 100%, share strength once, and verify strength stays at one through several maps and shops. Then acquire strength directly and share it again, checking the total increases exactly once for each acquisition. Also verify the other upgrades in game and the first spawn following a full sync.

Logs show the local stat before/after sharing and each incoming host stat plus the local bonus merged into it. Host migration and reconnecting mid-run still need multiplayer validation. Guest-only bonuses are session-local and clear on return to the main menu or lobby menu.
