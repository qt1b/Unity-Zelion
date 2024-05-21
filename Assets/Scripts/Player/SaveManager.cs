using System;
using System.IO;
using Unity.Netcode;

namespace Player {
    public class SaveManager : NetworkBehaviour
    {
/* The format of the lookup table
 * 0 : X position (int)
 * 1 : Y pos
 * 2 : Life (max value, uint)
 * 3 : Stamina
 * 4 : Mana
 * 5 : Sword is unlocked (bool, formatted as 0 for false, 1 for true)
 * 6 : Bow
 * 7 : Poison
 * 8 : Dash
 * 9 : Slowdown
 * 10: TimeFreeze
 */
        public void GetSaveId() {
            if (IsServer)
                GetSaveIdServer();
            else GetSaveIdServerRpc();
        }
        private void GetSaveIdServer() {
            if (!File.Exists(SaveData.SavePath)) {
                File.CreateText(SaveData.SavePath).Write((byte)0);
            }
            int tryRead = File.OpenRead(SaveData.SavePath).ReadByte();
            if (tryRead < 0) throw new ArgumentException("invalid save file");
            SaveData.SaveId.Value = (byte)tryRead;
        }
        [ServerRpc]
        private void GetSaveIdServerRpc() {
            GetSaveIdServer();
        }
        
        // should also include the host ?
        [ClientRpc]
        public void WriteSaveDataClientRpc(byte id) {
            File.WriteAllText(SaveData.SavePath,((char)id).ToString());
        }
    }
}