using System.IO;

namespace Global {
    public class SaveManager {
        public static void Save(byte saveID) {
            File.Open("zelion.sav",FileMode.Create).WriteByte(saveID);
        }
    }
}
