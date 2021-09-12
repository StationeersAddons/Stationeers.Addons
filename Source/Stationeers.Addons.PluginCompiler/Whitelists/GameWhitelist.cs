// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

namespace Stationeers.Addons.PluginCompiler.Whitelists
{
    public sealed class GameWhitelist : IWhitelistRegistry
    {
        public void Register(PluginWhitelist whitelist)
        {
            whitelist.WhitelistTypes();
            whitelist.BlacklistTypes();
            whitelist.WhitelistTypesNamespaces(
                // ::.*
                typeof(Bread),
                
                // Assets.*
                typeof(Assets.Features.AtmosphericScattering.Code.AtmosphericScatteringSun),
                typeof(Assets.Scripts.Composition),
                typeof(Assets.Scripts.AI.BehaviorType),
                typeof(Assets.Scripts.Atmospherics.Chemistry),
                //typeof(Assets.Scripts.Database.DatabaseConnector),
                typeof(Assets.Scripts.Effects.EffectType),
                typeof(Assets.Scripts.Events.Tutorial),
                typeof(Assets.Scripts.Inventory.SlotDisplay),
                typeof(Assets.Scripts.Leaderboard.LeaderboardDetail),
                typeof(Assets.Scripts.Networking.ChatMessage),
                typeof(Assets.Scripts.Networks.CableNetwork),
                typeof(Assets.Scripts.Objects.Attack),
                typeof(Assets.Scripts.Ping.PingManager),
                typeof(Assets.Scripts.Serialization.XmlSaveLoad), // Is this safe?
                typeof(Assets.Scripts.Sound.AudioManager), 
                typeof(Assets.Scripts.Steam.StatInfo), 
                typeof(Assets.Scripts.Util.Permutation), 
                typeof(Assets.Scripts.Vehicles.Vehicle), 
                typeof(Assets.Scripts.Voxel.Mineables), 
                typeof(Assets.Scripts.Weather.StormDirection), 
                typeof(Assets.Scripts.AssetCreation.ThingCreation), 
                typeof(Assets.Scripts.AwayMission.ServerInfo), 
                typeof(Assets.Scripts.FirstPerson.FirstPersonHelmet), 
                typeof(Assets.Scripts.GridSystem.Cell), 
                //typeof(Assets.Scripts.OpenNat.Mapping), 
                typeof(Assets.Scripts.PlayerInfo.PlayerDetail), 
                typeof(Assets.Scripts.SeasonalEvents.SeasonalEvent), 
                typeof(Assets.Scripts.UI.Digit),
                
                // Objects.*
                typeof(Objects.DirtCanister),
                typeof(Objects.Electrical.Fridge),
                typeof(Objects.Items.Consumable),
                typeof(Objects.Pipes.OrganPipe),
                typeof(Objects.SpaceShuttle.CouplingUnitDirection),
                typeof(Objects.Structures.Frame)
            );
        }
    }
}