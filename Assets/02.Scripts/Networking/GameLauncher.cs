using Fusion;
using System;
using System.Collections.Generic;
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

            Open();
            JoinLobby();
            Close();
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

        #region Network Static Method

        public static async new void Open() {
				    try {
				        stateManager.StopState();
				        await FusionSocket.Open();
				        stateManager.StartState(STATE.LOADING_LOBBY);
				    } catch(NetworkException e) {
				        stateManager.Abort();
				    } catch (Exception e) {
                stateManager.Abort();
            }
				}
				
				public static async new void JoinLobby() {
				    try {
				        stateManager.StopState();
				        await FusionSocket.JoinLobby();
				    } catch(NetworkException e) {
				        stateManager.Abort();
				    } catch (Exception e) {
                stateManager.Abort();
            }
				}
				
				public static async new void Close() {
				    try {
				        stateManager.StopState();
				        await FusionSocket.Close();
				        stateManager.StartState(STATE.CLOSED);
				    } catch(NetworkException e) {
				        stateManager.Abort();
				    } catch (Exception e) {
                stateManager.Abort();
            }
				}

        #endregion
        
        #region Lobby Event Method
				
				public override void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {
				    if (isFirstSessionUpdate) {
						    stateManager.StopState();
						    
						    isFirstSessionUpdate = false;
						    
						    stateManager.StartState(STATE.LOBBY);
						}
				}
				
				#endregion
    }
}