using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using CodeJay.Module;

namespace CodeJay
{
    public class SaveManager : IManager, IModuleRegistrar<ISaveModule>
    {
        private const string SaveFileKey = "SaveFile";
        private SaveData _saveData;
        private Dictionary<Type, ISaveModule> _saveModules = new();

        public void Dispose()
        {
            _saveModules?.Clear();
        }

        public void Load()
        {
            PlayerPrefLoad();
        }

        public void Register(ISaveModule registry)
        {
            _saveModules.Add(registry.GetType(), registry);
        }

        public void Save()
        {
            PlayerPrefsSave();
        }

        #region ## Temporary Methods ##
        private void PlayerPrefLoad()
        {
            var saveData = PlayerPrefs.GetString(SaveFileKey, string.Empty);
            if (string.IsNullOrEmpty(saveData))
            {
                _saveData = new SaveData();
            }
            else
            {
                try
                {
                    _saveData = JsonConvert.DeserializeObject<SaveData>(saveData);
                }
                catch (JsonException)
                {
                    Debug.LogError("Failed to deserialize save data. Creating a new save data instance.");
                    _saveData = new SaveData();
                }
            }

            foreach (var saveHandler in _saveModules.Values)
            {
                saveHandler.Initialize(_saveData);
            }
        }
        private void PlayerPrefsSave()
        {
            foreach (var saveHandler in _saveModules.Values)
            {
                saveHandler.Save(_saveData);
            }

            PlayerPrefs.SetString(SaveFileKey, JsonConvert.SerializeObject(_saveData));
            PlayerPrefs.Save();
        }


        #endregion
    }
}