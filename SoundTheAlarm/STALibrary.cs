using System;
using System.IO;
using System.Xml;

namespace SoundTheAlarm {
    public class STALibrary {

        private static STALibrary _instance = null;

        public STAConfiguration STAConfiguration { get; set; } = new STAConfiguration();
        public STAAction STAAction { get; set; } = new STAAction();

        public void LoadXML(string path) {
            bool flag = !File.Exists(path);
            if(flag) {
                throw new Exception("Failed to load file: " + path);
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlNode master = doc.DocumentElement.SelectSingleNode("SoundTheAlarm");

            foreach(XmlNode node in doc.DocumentElement.ChildNodes) {
                string text = node.Name.Trim().ToLower();
                if(!string.IsNullOrWhiteSpace(text)) {
                    switch (text) {
                        case "enablefiefpopup":
                            STAConfiguration.EnableFiefPopup = node.InnerText.ToBool();
                            break;
                        case "pauseonfiefpopup":
                            STAConfiguration.PauseOnFiefPopup = node.InnerText.ToBool();
                            break;
                        case "enablewarpopup":
                            STAConfiguration.EnableWarPopup = node.InnerText.ToBool();
                            break;
                        case "enablepeacepopup":
                            STAConfiguration.EnablePeacePopup = node.InnerText.ToBool();
                            break;
                        case "pauseonwarpeacepopup":
                            STAConfiguration.PauseOnWarPeacePopup = node.InnerText.ToBool();
                            break;
                        case "enabledebugmessages":
                            STAConfiguration.EnableDebugMessages = node.InnerText.ToBool();
                            break;
                    }
                }
            }
        }

        public static STALibrary Instance {
            get {
                if(STALibrary._instance == null) {
                    STALibrary._instance = new STALibrary();
                }
                return STALibrary._instance;
            }
        }

    }
}
