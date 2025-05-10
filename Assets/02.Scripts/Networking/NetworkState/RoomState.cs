using UltimateCartFights.UI;

namespace UltimateCartFights.Network {
    public class RoomState : INetworkState {

        public void Start() {
            PanelUI.Instance.SetPanel(PanelUI.Panel.ROOM);
            PanelUI.Instance.InitializeRoom();
        }

        public void Update() { }

        public void Terminate() { 
            PanelUI.Instance.LeaveRoom();
        }
    }
}