// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

namespace Stationeers.Addons.PluginCompiler.Whitelists
{
    internal sealed class GameWhitelist : IWhitelistRegistry
    {
        public void Register(PluginWhitelist whitelist)
        {
            whitelist.WhitelistTypes();
            whitelist.BlacklistTypes();
            whitelist.WhitelistTypesNamespaces(
                // ::.*
                typeof(Bread),
                
                // Assets.*
                typeof(Reagents.Recipe), 
                typeof(Assets.Features.AtmosphericScattering.Code.AtmosphericScatteringSun),
                typeof(Assets.Scripts.Composition),
                typeof(Assets.Scripts.AI.BehaviorType),
                typeof(Assets.Scripts.Atmospherics.Chemistry),
                typeof(Assets.Scripts.Database.DatabaseConnector),
                typeof(Assets.Scripts.Database.DatabaseConnector),
                typeof(Assets.Scripts.Effects.EffectType),
                typeof(Assets.Scripts.Objects.Electrical.Autolathe),
                typeof(Assets.Scripts.Objects.Pipes.HydroponicTray),
                typeof(Assets.Scripts.Objects.Appliances.Appliance),
                typeof(Assets.Scripts.Objects.Chutes.Harvester),
                typeof(Assets.Scripts.Objects.Clothing.Clothing),
                typeof(Assets.Scripts.Objects.Entities.Animal),
                typeof(Assets.Scripts.Objects.Items.Backpack),
                typeof(Assets.Scripts.Objects.Motherboards.CameraDisplay),
                typeof(Assets.Scripts.Objects.Structures.Airlock),
                typeof(Assets.Scripts.Objects.Weapons.Torpedo),
                typeof(Assets.Scripts.Objects.SpaceShuttle.LaunchPad),
                typeof(Assets.Scripts.Events.Tutorial),
                typeof(Assets.Scripts.Inventory.SlotDisplay),
                typeof(Assets.Scripts.Leaderboard.LeaderboardManager),
                typeof(Assets.Scripts.Networking.ChatMessage),
                typeof(Assets.Scripts.Networks.CableNetwork),
                typeof(Assets.Scripts.Objects.Attack),
                typeof(Assets.Scripts.Ping.PingManager),
                typeof(Assets.Scripts.Serialization.XmlSaveLoad), // Is this safe?
                typeof(Assets.Scripts.Sound.AudioManager), 
                typeof(Assets.Scripts.Util.Permutation), 
                typeof(Assets.Scripts.Vehicles.Vehicle), 
                typeof(Assets.Scripts.Voxel.Minables), 
                typeof(Assets.Scripts.Weather.StormDirection), 
                typeof(Assets.Scripts.AssetCreation.ThingCreation), 
                typeof(Assets.Scripts.AwayMission.AwayMissionInfo), 
                typeof(Assets.Scripts.FirstPerson.FirstPersonHelmet), 
                typeof(Assets.Scripts.GridSystem.Cell), 
                typeof(Assets.Scripts.PlayerInfo.PlayerDetail), 
                typeof(Assets.Scripts.SeasonalEvents.SeasonalEvent), 
                typeof(Assets.Scripts.UI.Digit),
                
                // Objects.*
                typeof(Objects.DirtCanister),
                typeof(Objects.Electrical.Fridge),
                typeof(Objects.Items.Consumable),
                typeof(Objects.Pipes.OrganPipe),
                typeof(Objects.SpaceShuttle.CouplingUnitDirection),
                typeof(Objects.Structures.Frame),

                // ImGui
                typeof(ImGuiNET.ImGui),
                typeof(ImGuiNET.Unity.DearImGui)
            );
        }
    }
}