namespace Brierley.AEModules.RequestCredit.Components
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    //using Brierley.DNNModules.PortalModuleSDK;
    using Brierley.FrameWork.Data.DomainModel;
    #endregion

    /// <summary>
    /// Display types
    /// </summary>
    public enum DisplayType
    {
        /// <summary>
        /// Represents the selection for dropdownlist
        /// </summary>
        DropDownList = 0,

        /// <summary>
        /// Represents the selection for radiobuttion
        /// </summary>
        RadioButton = 1
    }

    /// <summary>
    /// Module configuration 
    /// </summary>
    public class ModuleConfiguration
    {
        /// <summary>
        /// Holds content
        /// </summary>
        private string content = string.Empty;
        private string name = string.Empty;
        private string descriptiveText = string.Empty;
        private string format = string.Empty;
        private string displayText = string.Empty;
        private string message = string.Empty;
        private bool isRequired = false;

        #region Properties
        /// <summary>
        /// Search fields
        /// </summary>
        private Dictionary<string, string> searchFields = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets Search fields
        /// </summary>
        public Dictionary<string, string> SearchFields
        {
            get { return this.searchFields; }
            set { this.searchFields = value; }
        }

        /// <summary>
        /// Gets or sets Module Content
        /// </summary>
        public string Content
        {
            get { return this.content; }
            set { this.content = value; }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        public string DescriptiveText
        {
            get { return this.descriptiveText; }
            set { this.descriptiveText = value; }
        }
        public string Format
        {
            get { return this.format; }
            set { this.format = value; }
        }
        public string DisplayText
        {
            get { return this.displayText; }
            set { this.displayText = value; }
        }
        public string Message
        {
            get { return this.message; }
            set { this.message = value; }
        }
        public bool IsRequired
        {
            get { return this.isRequired; }
            set { this.isRequired = value; }
        }
        #endregion

        #region Public Helpers

        /// <summary>
        /// Method to get configuration
        /// </summary>
        /// <param name="moduleId">Current usercontrols moduleid used in a particular page of DNN</param>
        /// <returns>Retunrns the stored configuration values</returns>
        public static ModuleConfiguration GetConfiguration(int moduleId)
        {
            ModuleConfiguration config = new ModuleConfiguration();
            string key = "CSPortal:RequestCreditConfig:" + moduleId.ToString();
            //ClientConfiguration cfg = PortalState.DataManager.GetClientConfiguration(key);
            //if (cfg != null)
            //{
            //    config.Content = cfg.Value;
            //}

            return config;
        }

        /// <summary>
        /// Method to save configuration
        /// </summary>
        /// <param name="moduleId">Current usercontrols moduleid used in a particular page of DNN</param>
        /// <param name="content">Content to be saved if any</param>
        public static void SaveConfiguration(int moduleId, string content)
        {
            string key = "CSPortal:RequestCreditConfig:" + moduleId.ToString();
            ClientConfiguration ccfg = new ClientConfiguration();
            ccfg.Key = key;
            ccfg.Value = content;
            ccfg.ExternalValue = false;
            //ClientConfiguration existing = PortalState.DataManager.GetClientConfiguration(key);
            //if (existing != null)
            //{
            //    PortalState.DataManager.UpdateClientConfiguration(ccfg);
            //}
            //else
            //{
            //    PortalState.DataManager.CreateClientConfiguration(ccfg);
            //}
        }

        #endregion
    }
}