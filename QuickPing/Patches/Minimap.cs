﻿using HarmonyLib;
using Jotunn.Managers;
using QuickPing.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QuickPing.Patches
{
    /// <summary>
    /// Not really a patch, but a modified game method used in a patch.
    /// </summary>
    internal static class Minimap_Patch
    {


        private static GameObject panel;
        private static GameObject nameInput;
        private static GameObject toggleSaveName;

        public static bool IsNaming = false;

        private static string tempOriginalText;

        /// <summary>
        /// Check if an object can be pinned
        /// </summary>
        /// <param name="strID"></param>
        /// <returns></returns>
        public static Minimap.PinType IsPinable(string strID)
        {

            string baseLocalization = Localization_Patch.GetBaseTranslation(strID);
            Dictionary<Minimap.PinType, List<string>> pinables = new()
            {
                //Fire pin
                {
                    Minimap.PinType.Icon0,
                    new List<string>
                    {
                        "$piece_firepit",
                        "$piece_bonfire",
                        "$piece_fire",
                        "GoblinCamp"
                    }
                },

                //Home pin
                {
                    Minimap.PinType.Icon1,
                    new List<string>
                    {
                        //"$",
                        //"$",
                        //"$",
                        //"$",
                    }
                },

                //Hammer pin
                {
                    Minimap.PinType.Icon2,
                    new List<string>
                    {
                        "$piece_deposit_copper",
                        "$piece_deposit_silver",
                        "$piece_deposit_silvervein",
                        "$piece_deposit_tin",
                        "$piece_mudpile"
                    }
                },

                //Point pin
                {
                    Minimap.PinType.Icon3,
                    new List<string>
                    {
                        "$item_raspberries",
                        "$item_blueberries",
                        "$item_cloudberries",
                        "$item_dragonegg",
                        "$item_dandelion",
                        "$item_mushroomcommon",
                        "$item_magecap",
                        "$item_mushroomblue",
                        "$item_thistle",
                        "$item_jotunpuffs",
                        // fix #58 conflicts with PlantEverything
                        "$peRaspberryBushName",
                        "$peBlueberryBushName",
                        "$peCloudberryBushName",
                        "$pePickableMushroomName",
                        "$pePickableYellowMushroomName",
                        "$pePickableBlueMushroomName",
                        "$pePickableThistleName",
                        "$pePickableDandelionName",

                    }
                },

                //Rune pin
                {
                    Minimap.PinType.Icon4,
                    new List<string>
                    {
                        "$location_forestcave",
                        "$location_forestcrypt",
                        "Stone Henge",
                        "$location_sunkencrypt",
                        "$location_mountaincave",
                        "$location_dvergrtown",
                        "$piece_portal"
                    }
                }
            };

            foreach (var pinType in pinables.Keys)
            {
                if (pinables[pinType].Contains(strID) || pinables[pinType].Contains(baseLocalization))
                {
                    return pinType;
                }
            }
            return Minimap.PinType.None;
        }

        public static void ForceAddPin(DataManager.PinnedObject pinnedObject) => AddPin(pinnedObject, force: true);

        public static void RenamePin(DataManager.PinnedObject pinnedObject) => AddPin(pinnedObject, rename: true);

        public static void AddPin(DataManager.PinnedObject pinnedObject, bool force = false, bool rename = false)
        {
            if (!Settings.AddPin.Value && !force) { return; }
            bool pinned = false;

            string strID = pinnedObject.PinData.m_name;



            Minimap.PinData pinData = new Minimap.PinData
            {
                m_type = IsPinable(strID)
            };

            //TODO ignore itemdrops
            //if (hover && hover.GetComponent<ItemDrop>())
            //    pinData.m_type = Minimap.PinType.None;

            Minimap.PinData closestPin = Minimap.instance.GetClosestPin(pinnedObject.PinData.m_pos, Settings.ClosestPinRange.Value);

            //Check portal 
            if (strID != null && strID != "" && Regex.IsMatch(strID, "piece_portal"))
            {
                pinData.m_name = strID.Split(':')[1];
                if (closestPin != null)
                {
                    Minimap.instance.RemovePin(closestPin);
                }

                pinData.m_pos = pinnedObject.PinData.m_pos;
                pinData = Minimap.instance.AddPin(pinData.m_pos, pinData.m_type, pinData.m_name, true, false, 0L);
                pinned = true;
                QuickPingPlugin.Log.LogInfo($"Add Portal Pin : Name:{pinData.m_name} x:{pinData.m_pos.x}, y:{pinData.m_pos.y}, Type:{pinData.m_type}");

            }
            else if (closestPin == null || rename)
            {

                pinData.m_name ??= Localization.instance.Localize(strID);

                // check for customnames
                bool customName = DataManager.CustomNames.ContainsKey(strID);
                if (customName)
                    pinData.m_name = DataManager.CustomNames[strID];

                pinData.m_pos = pinnedObject.PinData.m_pos;
                if (pinData.m_name == null || pinData.m_name == "" && !customName)
                {
                    pinData.m_name = Settings.DefaultPingText;
                }
                if (pinData.m_type == Minimap.PinType.None && force)
                {
                    pinData.m_type = Settings.DefaultPinType.Value;
                    pinData = Minimap.instance.AddPin(pinData.m_pos, pinData.m_type, pinData.m_name, true, false, 0L);
                    pinned = true;
                    QuickPingPlugin.Log.LogInfo($"Add Pin : Name:{pinData.m_name} x:{pinData.m_pos.x}, y:{pinData.m_pos.y}, Type:{pinData.m_type}");

                }

                if (pinData.m_type != Minimap.PinType.None)
                {
                    if (closestPin == null)
                    {
                        pinData = Minimap.instance.AddPin(pinData.m_pos, pinData.m_type, pinData.m_name, true, false, 0L);
                        pinned = true;
                    }
                    else if (rename)
                        pinData = closestPin;
                    QuickPingPlugin.Log.LogInfo($"Add Pin : Name:{pinData.m_name} x:{pinData.m_pos.x}, y:{pinData.m_pos.y}, Type:{pinData.m_type}");

                    //Check if Settings.AskForName.Value is true, and if CustomNames contains its name.
                    //if true ask for user input before adding pin
                    if (rename)
                    {
                        GUIManager.BlockInput(true);
                        InitNameInput();
                        tempOriginalText = strID;
                        Minimap.instance.ShowPinNameInput(pinData);
                    }
                }

            }

            if (pinnedObject.ZDOID != ZDOID.None && pinned)
            {
                GameObject obj = ZNetScene.instance.FindInstance(pinnedObject.ZDOID);
                IDestructible idestructible = obj.GetComponent<IDestructible>();

                if (idestructible != null)
                {


                    if (!DataManager.PinnedObjects.ContainsKey(pinnedObject.ZDOID))
                    {
                        DataManager.PinnedObjects[pinnedObject.ZDOID] = pinData;
                        if (!ZNet.m_isServer)
                        {
                            ZPackage package = DataManager.PackPinnedObject(new DataManager.PinnedObject
                            {
                                PinData = pinData,
                                ZDOID = pinnedObject.ZDOID
                            });
                            ZNet.instance.GetServerRPC().Invoke("OnClientAddPinnedObject", package);
                        }
                    }
                }
            }
        }

        #region NameInput

        public static void UpdateNameInput()
        {
            if (Minimap.instance.m_namePin == null)
            {
                Minimap.instance.m_wasFocused = false;
            }
            if (Minimap.instance.m_namePin != null)
            {
                panel.SetActive(true);
                nameInput.SetActive(true);
                toggleSaveName.SetActive(true);
                var inputField = nameInput.GetComponent<InputField>();
                var toggleSave = toggleSaveName.GetComponent<Toggle>();
                if (!inputField.isFocused)
                {
                    EventSystem.current.SetSelectedGameObject(nameInput);
                }
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    ValidateNameInput(inputField, toggleSave.isOn);
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CancelNameInput();
                }
                Minimap.instance.m_wasFocused = true;
            }
            else //end
            {
                panel.gameObject.SetActive(value: false);
                IsNaming = false;
                tempOriginalText = null;
                GUIManager.BlockInput(false);
                DestroyGUI();
            }
        }

        private static void CancelNameInput()
        {
            Minimap.instance.m_namePin = null;
            Minimap.instance.m_wasFocused = false;
            panel.gameObject.SetActive(value: false);
            IsNaming = false;
            tempOriginalText = null;
            GUIManager.BlockInput(false);
            DestroyGUI();
        }

        private static void DestroyGUI()
        {
            GameObject.Destroy(nameInput);
            GameObject.Destroy(panel);
            GameObject.Destroy(toggleSaveName);
        }

        private static void ValidateNameInput(InputField inputField, bool save)
        {
            string text = inputField.text;
            text = text.Replace('$', ' ');
            text = text.Replace('<', ' ');
            text = text.Replace('>', ' ');
            string originalText = tempOriginalText;

            Minimap.instance.m_namePin.m_name = text;

            // Persistent save of text value for this pinned object
            if (save)
            {
                QuickPingPlugin.Log.LogInfo($"Save name {Minimap.instance.m_namePin.m_name} for {originalText}");
                SaveName(Minimap.instance.m_namePin.m_name, originalText);
            }
            Minimap.instance.m_namePin = null;


        }
        /// <summary>
        /// Save the name of a pinned object and update PinnedObjects list with new value
        /// </summary>
        /// <param name="originalText"></param>
        /// <param name="newText"></param>
        //private static void UpdatePinnedObject(string originalText)
        //{

        //    for (int i = 0; i < PinnedObjects.Count; i++)
        //    {
        //        var keyValuePair = PinnedObjects.ElementAt(i);
        //        if (keyValuePair.Value.m_name == originalText)
        //        {
        //            // Modify the value here
        //            keyValuePair.Value.m_name = CustomNames[originalText];
        //            var pin = Minimap.instance.GetClosestPin(keyValuePair.Value.m_pos, Settings.ClosestPinRange.Value);
        //            if (pin != null)
        //                pin.m_name = CustomNames[originalText];
        //            // Update the dictionary
        //            PinnedObjects[keyValuePair.Key] = keyValuePair.Value;
        //        }
        //    }
        //}

        /// <summary>
        /// Persistent save original name for this pinned object
        /// </summary>
        /// <param name="m_name"></param>
        /// <param name="originalName"></param>
        private static void SaveName(string m_name, string originalName)
        {
            if (DataManager.CustomNames.ContainsKey(originalName))
            {
                DataManager.CustomNames[originalName] = m_name;
            }
            else
            {
                DataManager.CustomNames.Add(originalName, m_name);
            }
        }

        private static void InitNameInput()
        {
            if (GUIManager.Instance == null)
            {
                QuickPingPlugin.Log.LogError("GUIManager instance is null");
                return;
            }

            if (!GUIManager.CustomGUIFront)
            {
                QuickPingPlugin.Log.LogError("GUIManager CustomGUI is null");
                return;
            }


            IsNaming = true;

            panel = GUIManager.Instance.CreateWoodpanel(
                parent: GUIManager.CustomGUIFront.transform,
                anchorMin: new Vector2(0.5f, 0.5f),
                anchorMax: new Vector2(0.5f, 0.5f),
                position: new Vector2(0f, 0f),
                width: 200f,
                height: 90f,
                draggable: true);

            // Add a vertical layout group to the panel
            var verticalLayoutGroup = panel.gameObject.AddComponent<VerticalLayoutGroup>();

            // Set the spacing between elements
            verticalLayoutGroup.spacing = 10f;
            verticalLayoutGroup.padding = new RectOffset(10, 10, 10, 10);
            verticalLayoutGroup.childControlWidth = true;
            verticalLayoutGroup.childControlHeight = true;


            nameInput = GUIManager.Instance.CreateInputField(
                parent: panel.transform,
                anchorMin: new Vector2(0.5f, 0.9f),
                anchorMax: new Vector2(0.5f, 0.9f),
                position: new Vector2(0, 0),
                contentType: InputField.ContentType.Standard,
                placeholderText: "Pin Name",
                fontSize: 16,
                width: 90f,
                height: 30f
            );
            nameInput.SetActive(IsNaming);

            toggleSaveName = GUIManager.Instance.CreateToggle(
                parent: panel.transform,
                width: 20f,
                height: 20f
                );

            Text saveNameText = toggleSaveName.transform.Find("Label").GetComponent<Text>();
            saveNameText.color = Color.white;
            saveNameText.text = "Save";
            saveNameText.enabled = true;
            toggleSaveName.SetActive(IsNaming);
            toggleSaveName.transform.position += new Vector3(20f, 0, 0);


            //toggleSaveAll = GUIManager.Instance.CreateToggle(
            //    parent: panel.transform,
            //    width: 20f,
            //    height: 20f
            //    );
            //toggleSaveAll.transform.position += new Vector3(20f, 0, 0);
            //Text saveAllText = toggleSaveAll.transform.Find("Label").GetComponent<Text>();
            //saveAllText.color = Color.white;
            //saveAllText.text = "Update all pins.";
            //saveAllText.enabled = true;
            //toggleSaveAll.GetComponent<Toggle>().interactable = false;

        }
        #endregion







        #region Patches

        [HarmonyPatch(typeof(Minimap))]
        [HarmonyPatch(nameof(Minimap.RemovePin), new Type[] { typeof(Minimap.PinData) })]
        [HarmonyPrefix]
        public static bool RemovePin(Minimap __instance, Minimap.PinData pin)
        {
            //checks 
            if (pin == null || pin.m_name == null || pin.m_name == "")
            {
                return true;
            }


            foreach (var p in DataManager.PinnedObjects)
            {
                if (p.Value.Compare(pin))
                {
                    pin = __instance.GetClosestPin(p.Value.m_pos, Settings.ClosestPinRange.Value);

                    KeyValuePair<ZDOID, Minimap.PinData> pinnedObject = DataManager.PinnedObjects.FirstOrDefault((x) => x.Value.Compare(p.Value));
                    if (!ZNet.instance.IsServer())
                        ZNet.instance.GetServerRPC().Invoke("OnClientRemovePinnedObject", DataManager.PackPinnedObject(new DataManager.PinnedObject
                        {
                            ZDOID = pinnedObject.Key,
                            PinData = pinnedObject.Value
                        }));
                    DataManager.PinnedObjects.Remove(pinnedObject.Key);
                    break;
                }

            }
            if ((bool)pin.m_uiElement)
            {
                UnityEngine.Object.Destroy(pin.m_uiElement.gameObject);
            }
            __instance.m_pins.Remove(pin);
            return false;
        }

        #endregion

    }


}
