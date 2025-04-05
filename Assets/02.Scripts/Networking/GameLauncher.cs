using Fusion;
using System;
using System.Collections.Generic;
using UltimateCartFights.UI;
using UnityEngine;

namespace UltimateCartFights.Network {
    public class GameLauncher : FusionSocket {

        #region Unity Basic Method

        private void Start() {
            // 게임 환경 설정
			Application.runInBackground = true;
			QualitySettings.vSyncCount = 1;
			DontDestroyOnLoad(this.gameObject);
			
			// 로비 대기 씬으로 이동
			UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
			
			// FSM 실행
			stateManager.Start();

            // Open();
            // JoinLobby();
            // Close();
        }
        
        private void Update() {
		        stateManager.Update();
		    }

        #endregion
        
        #region Network State Method

		public static bool IsNetworked() {
			switch(stateManager.State) {
				case STATE.CLOSED:
				case STATE.LOADING_LOBBY:
					return false;
		
				default:
					return true;
			} 
		}
		
		#endregion

		#region Network Method

		public static async new void Open() {
					try {
						stateManager.StopState();
						await FusionSocket.Open();
						stateManager.StartState(STATE.LOADING_LOBBY);
					} catch(NetworkException e) {
						stateManager.Abort();
						PopupUI.Instance.OpenError(GetShutdownMessage(e.ShutdownReason));
					} catch (Exception e) {
				stateManager.Abort();
				PopupUI.Instance.OpenError(e.Message);
			}
				}
				
				public static async new void JoinLobby() {
					try {
						stateManager.StopState();
						await FusionSocket.JoinLobby();
					} catch(NetworkException e) {
						stateManager.Abort();
						PopupUI.Instance.OpenError(GetShutdownMessage(e.ShutdownReason));
					} catch (Exception e) {
				stateManager.Abort();
				PopupUI.Instance.OpenError(e.Message);
			}
				}

				public static async new void CreateRoom(RoomInfo room) {
				    try {
				        stateManager.StopState();
				        await FusionSocket.CreateRoom(room);
				        stateManager.StartState(STATE.ROOM);
				    } catch (NetworkException e) {
				        stateManager.Abort();
				        PopupUI.Instance.OpenError(GetShutdownMessage(e.ShutdownReason));
				    }  catch (Exception e) {
                stateManager.Abort();
                PopupUI.Instance.OpenError(e.Message);
            }
				}

				public static async new void JoinRoom(RoomInfo room) {
				    try {
				        stateManager.StopState();
				        await FusionSocket.JoinRoom(room);
				        stateManager.StartState(STATE.ROOM);
				    } catch (NetworkException e) {
				        stateManager.Abort();
				        PopupUI.Instance.OpenError(GetShutdownMessage(e.ShutdownReason));
				    }
				}
				
				public static async new void Close() {
					try {
						stateManager.StopState();
						await FusionSocket.Close();
						stateManager.StartState(STATE.CLOSED);
					} catch(NetworkException e) {
						stateManager.Abort();
						PopupUI.Instance.OpenError(GetShutdownMessage(e.ShutdownReason));
					} catch (Exception e) {
				stateManager.Abort();
				PopupUI.Instance.OpenError(e.Message);
			}
				}

		#endregion

		#region Connection Event Method

		// 네트워크가 종료될 때 실행되는 함수
		public override void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {
			base.OnShutdown(runner, shutdownReason);

			// 정상적인 종료가 아니라면 오류 메세지를 띄운다
			if (shutdownReason != ShutdownReason.Ok)
				PopupUI.Instance.OpenError(GetShutdownMessage(shutdownReason));
			
			// 아직 Closed 혹은 LOADING_LOBBY 상태가 아니라면 로비로 돌아간다
						if (stateManager.State == STATE.NONE) return;
						if (stateManager.State == STATE.CLOSED) return;
						if (stateManager.State == STATE.LOADING_LOBBY) return;
						Open();
		}

		// 네트워크 종료 이유 메세지를 반환한다
		private static string GetShutdownMessage(ShutdownReason shutdownReason) {
			switch(shutdownReason) {
				case ShutdownReason.GameClosed:
					return "방과의 접속이 끊겼습니다!";

				case ShutdownReason.GameNotFound:
					return "해당 방이 없습니다!";

				case ShutdownReason.GameIsFull:
					return "인원이 꽉 찼습니다!";

				default:
					return "오류가 발생했습니다!";
			}
		}

		#endregion
			
		#region Lobby Event Method
				
		public static List<SessionInfo> sessions = new List<SessionInfo>();
				
		public override void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {
			sessions = sessionList;
		
			if (isFirstSessionUpdate) {
				stateManager.StopState();
		
				PanelUI.Instance.RefreshRoomList();
				isFirstSessionUpdate = false;
		
				stateManager.StartState(STATE.LOBBY);
			}
		}
		
		#endregion
    }
}