using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundTheAlarm {
    public class STAConfiguration {

        public bool EnableFiefPopup { get; set; } = true;
        public bool PauseOnFiefPopup { get; set; } = true;
        public bool EnableWarPopup { get; set; } = true;
        public bool EnablePeacePopup { get; set; } = true;
        public bool PauseOnWarPeacePopup { get; set; } = false;
        public bool EnableDebugMessages { get; set; } = false;

    }
}
