namespace LowEnergyJetpack
{
    using System;
    using System.Collections.Generic;
    using Sandbox.Definitions;
    using VRage.Game;
    using VRage.Game.Components;
    using VRage.Game.ObjectBuilders.Definitions;
    using VRage.Utils;

    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class Main : MySessionComponentBase
    {
        private const float LowEnergy = 0.000000000000000000000000000000000001f;
        private const string CharacterDefinition = "Default_Astronaut";
        private const string Hydrogen = "Hydrogen";

        public override void LoadData()
        {
            try
            {
                MyCharacterDefinition myCharacterDefinition;
                if (!MyDefinitionManager.Static.Characters.TryGetValue(CharacterDefinition, out myCharacterDefinition))
                {
                    this.LogMessage($"Cannot find character definition for {CharacterDefinition} so cannot alter JetPack settings");
                    return;
                }

                var thrustProperties = this.GetThrustPropertiesAndSetNewFuelConverter(myCharacterDefinition);
                if (thrustProperties != null)
                {
                    this.SetThrusterPowerConsumption(thrustProperties);
                }

                this.RemoveSuitResourceStorageItems(myCharacterDefinition.SuitResourceStorage);
            }
            catch (Exception exception)
            {
                this.LogException(exception);
            }
        }

        private void RemoveSuitResourceStorageItems(List<SuitResourceDefinition> suitResourceStorage)
        {
            if (suitResourceStorage == null || suitResourceStorage.Count <= 0)
            {
                this.LogMessage("Cannot find the SuitResourceStorage so cannot alter JetPack settings.");
                return;
            }

            int hydrogenRemovedCount = this.RemoveGasProperty(suitResourceStorage, Hydrogen);
            this.LogMessage($"Found and removed [{hydrogenRemovedCount}] {Hydrogen} resource storage from the suit.");
        }

        private int RemoveGasProperty(List<SuitResourceDefinition> suitResourceStorage, string subTypeId)
        {
            return suitResourceStorage.RemoveAll(s => s.Id.TypeId == typeof(MyObjectBuilder_GasProperties) && s.Id.SubtypeId == subTypeId);
        }

        private MyObjectBuilder_ThrustDefinition GetThrustPropertiesAndSetNewFuelConverter(MyCharacterDefinition myCharacterDefinition)
        {
            var jetPack = myCharacterDefinition.Jetpack;
            if (jetPack == null)
            {
                this.LogMessage("Cannot find JetPack in the character definition so cannot alter JetPack settings");
                return null;
            }

            var thrustProperties = jetPack.ThrustProperties;
            if (thrustProperties == null)
            {
                this.LogMessage("Cannot find ThrustProperties on the JetPack in the character definition so cannot alter JetPack settings");
                return null;
            }

            thrustProperties.FuelConverter = new MyFuelConverterInfo();
            return thrustProperties;
        }

        private void SetThrusterPowerConsumption(MyObjectBuilder_ThrustDefinition thrustProperties)
        {
            thrustProperties.MinPowerConsumption = LowEnergy;
            thrustProperties.MaxPowerConsumption = LowEnergy;
            this.LogMessage("Altered the JetPack thruster settings to low energy");
        }

        private void LogMessage(string message)
        {
            MyLog.Default.WriteLineAndConsole($"LowEnergyJetpack: {message}");
        }

        private void LogException(Exception exception)
        {
            this.LogMessage($"{exception.Message}{Environment.NewLine}{exception.StackTrace}");
        }
    }
}
