using System.IO;

namespace Global {
    public static class SaveManager {
        public static void Save() {
            File.Open("zelion.sav",FileMode.Create).WriteByte(GlobalVars.SaveId);
        }
    }
}
