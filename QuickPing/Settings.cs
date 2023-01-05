﻿using BepInEx.Configuration;
using Jotunn.Configs;
using Jotunn.Managers;
using UnityEngine;

namespace QuickPing
{
    static class Settings
    {

        public const string DefaultPingText = "PING !";
        public static ConfigEntry<bool> PingWhereLooking { get; private set; }
        public static ConfigEntry<bool> AddPin { get; private set; }
        public static ConfigEntry<bool> AskForName { get; private set; }
        public static ConfigEntry<KeyCode> PingKey { get; private set; }
        public static ConfigEntry<KeyCode> PinEverythingKey { get; internal set; }

        public static ConfigEntry<Minimap.PinType> DefaultPinType { get; internal set; }

        public static ConfigEntry<Color> PlayerColor { get; private set; }
        public static ConfigEntry<Color> ShoutColor { get; private set; }
        public static ConfigEntry<Color> WhisperColor { get; private set; }
        public static ConfigEntry<Color> PingColor { get; private set; }
        public static ConfigEntry<Color> DefaultColor { get; private set; }
        public static ConfigEntry<float> ClosestPinRange { get; private set; }

        public static ButtonConfig PingBtn { get; private set; }
        public static ButtonConfig PingEverythingBtn { get; private set; }
        public static void Init()
        {
            //GENERAL
            PingWhereLooking = QuickPingPlugin.Instance.Config.Bind("General",
            "PingWhereLooking",
            true,
            "Create a ping where you are looking when you press <PingKey>");

            AddPin = QuickPingPlugin.Instance.Config.Bind("General",
                "AddPinOnMap",
                true,
                "If true, add a pin when useful resources (copper, berries, campfire, portals etc.) are pinged.");

            AskForName = QuickPingPlugin.Instance.Config.Bind("General",
                "AskForName",
                true,
                "If true, ask for a name when you ping a location. It will be used for all pings of the same object.");

            ClosestPinRange = QuickPingPlugin.Instance.Config.Bind("General",
                "ClosestPinRange",
                2f,
                "Minimum distance between objects to pin/replace portal tag");


            DefaultPinType = QuickPingPlugin.Instance.Config.Bind("General",
                "DefaultPinType",
                Minimap.PinType.RandomEvent, //ExclamationPoint
                "Default pin when forcing adding a pin on map");

            //COLORS
            PlayerColor = QuickPingPlugin.Instance.Config.Bind("Colors",
                "PlayerColor",
                Color.green,
                "Color for Player name in pings/messages.");

            ShoutColor = QuickPingPlugin.Instance.Config.Bind("Colors",
                "ShoutColor",
                Color.yellow,
                "Color for Shout ping.");
            WhisperColor = QuickPingPlugin.Instance.Config.Bind("Colors",
                "WhisperColor",
                new Color(1f, 1f, 1f, 0.75f),
                "Color for Whisper ping.");
            PingColor = QuickPingPlugin.Instance.Config.Bind("Colors",
                "PingColor",
                new Color(0.6f, 0.7f, 1f, 1f),
                "Color for \"Ping\" ping.");
            DefaultColor = QuickPingPlugin.Instance.Config.Bind("Colors",
                "DefaultColor",
                Color.white,
                "Default color");

            //BINDINGS
            PingKey = QuickPingPlugin.Instance.Config.Bind("Bindings",
                "PingInputKey",
                KeyCode.T,
                "The keybind to trigger a ping where you are looking");

            PinEverythingKey = QuickPingPlugin.Instance.Config.Bind("Bindings",
                "PingEverythingInputKey",
                KeyCode.G,
                "Add a pin on minimap to whatever you're looking at.");

            AddInputs();
        }

        public static void AddInputs()
        {
            PingBtn = new ButtonConfig
            {
                Name = "Ping",
                Key = PingKey.Value,
                Hint = "Ping where you are looking, and pin useful resources",

            };

            PingEverythingBtn = new ButtonConfig
            {
                Name = "PinEverything",
                Key = PinEverythingKey.Value,
                Hint = "Pin on map everything you're looking at",

            };

            InputManager.Instance.AddButton(MyPluginInfo.PLUGIN_GUID, PingBtn);
            InputManager.Instance.AddButton(MyPluginInfo.PLUGIN_GUID, PingEverythingBtn);

        }
    }
}
