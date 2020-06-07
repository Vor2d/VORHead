using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

namespace ParameterSystem
{
    public class SaveGroup : MonoBehaviour  //Manager class for save parameter to file;
    {
        [SerializeField] private InfoIF infoClass;  //Object storing the parameters;
        [SerializeField] private bool usingTimeAsAdd;   //True: using the time stamp as the file name; False: read from the input field;
        [SerializeField] private bool usingInputField;
        [SerializeField] private InputField address_IF; //Input field for file address;

        public static SaveGroup IS; //Instance;

        private string alternate_path;

        private void Awake()
        {
            IS = this;

            alternate_path = "";
        }

        #region Button

        public void save_button()
        {
            string json_string = JsonUtility.ToJson(infoClass); //Serialize to json string;
            string address = "";
            if (usingTimeAsAdd) { address = DateTime.Now.ToString("yyyyMMdd_hhmmss"); }
            if (usingInputField) { address += alternate_path; }
            else { address += address_IF.text; }
            address += ".json";
            File.WriteAllText(address, json_string);    //Write to file;
        }

        #endregion

        #region Utilities

        public void set_infoClass(InfoIF _infoClass)
        {
            infoClass = _infoClass;
        }

        public void set_path(string _alternate_path)
        {
            alternate_path = _alternate_path;
        }

        #endregion
    }
}