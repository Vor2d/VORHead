using UnityEngine;
using System.IO;

namespace ParameterSystem
{
    public class LoadGroup : MonoBehaviour  //Manager class for loading function;
    {
        [SerializeField] private InfoIF infoClass;  //Object for storing the parameters;

        public static LoadGroup IS; //Instance;

        private void Awake()
        {
            IS = this;
        }

        #region Button

        public void load_button()
        {
            SimpleFileBrowser.FileBrowser.ShowLoadDialog(
                (path) => load_json(path), () => { }, false, ".", "Load", "Select");    //Using package SimpleFileBrowser; See github description;

        }

        #endregion

        private void load_json(string path) //Loading from json;
        {
            string json_string = File.ReadAllText(path);
            //Debug.Log(json_string);
            JsonUtility.FromJsonOverwrite(json_string, infoClass);
        }

        #region Utilities

        public void set_infoClass(InfoIF _infoClass)
        {
            infoClass = _infoClass;
        }

        #endregion

    }
}

