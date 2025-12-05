// using UnityEngine;
// using System.Collections.Generic;
// using UnityEngine.UI;


// public class LoanActionManager : MonoBehaviour
// {
//     [SerializeField] private GameObject loanObjs; // 대출 관련 패널 오브젝트
//     [SerializeField] private GameObject loanPersonalGoldTog;
//     [SerializeField] private GameObject loanShopGoldTog;
//     [SerializeField] private GameObject loanShopActionTog;

//     List<DisplayedItemData> actionItemList;
//     private int currentPersonalClickedAmount;
//     private int currentShopClickedAmount;


//     public void OnPersonalLoanGoldClicked(bool isOn, int goldIndex){
//         if(isOn){
//             switch (goldIndex)
//             {
//                 case 0:
//                     currentPersonalClickedAmount = 2000;
//                     break;
//                 case 1:
//                     currentPersonalClickedAmount = 1000;                
//                     break;
//                 case 2:
//                     currentPersonalClickedAmount = 500;
//                     break;
//                 case 3:
//                     currentPersonalClickedAmount = 100;
//                     break;
//                 default:
//                     Debug.LogError("OnPersonalLoanGoldClicked Error: "+ goldIndex);
//                     return;
//             }
//         }
//     }

//     public void OnShopLoanGoldClicked(bool isOn, int goldIndex){
//         if(isOn){
//             switch (goldIndex)
//             {
//                 case 0:
//                     currentShopClickedAmount = 2000;
//                     break;
//                 case 1:
//                     currentShopClickedAmount = 1000;                
//                     break;
//                 case 2:
//                     currentShopClickedAmount = 500;
//                     break;
//                 case 3:
//                     currentShopClickedAmount = 100;
//                     break;
//                 default:
//                     Debug.LogError("OnShopLoanGoldClicked Error: "+ goldIndex);
//                     return;
//             }
//         }
//     }



//     public void OnPersonalLoanButtonClicked()
//     {
        
//     }

//     public void OnShopLoanButtonClicked()
//     {
//         // 요청 데이터 세팅
//         LoanUpdateRequest requestData = new LoanUpdateRequest();
//         requestData.debtType = "PAWNSHOP";
//         // 만약 복원에 체크 되어있다면
//         if (loanShopActionTog.transform.GetChild(0).GetComponent<Toggle>().isOn) // 대출
//         {
//             requestData.amount = currentShopClickedAmount; // 양수는 대출
//         }
//         else // 상환
//         {
//             requestData.amount = -1 *currentShopClickedAmount; // 음수는 상환
//         }
//         TransmissionManager.Instance.RequestToServer<LoanUpdateRequest,LoanUpdateResponse>(
//             RequestType.LOAN_UPDATE,
//             requestData,
//             (responseCode, responseData) =>
//             {
//                 // 통신 오류 체크
//                 if((ResponseCode)responseCode != ResponseCode.OK)
//                 {
//                     string errorMessage = "통신 오류: 아이템 복원/경매 처리 요청 실패.";
//                     TransmissionManager.Instance.OnHandleErrorResponseCode(responseCode, errorMessage);
//                 }
//                 else
//                 {
                    
//                 }
//             });
//         //요청하고 결과값 받기 -> 서버 있어야 받을 수 있음

//         TextAsset jsonFile = Resources.Load<TextAsset>("Mocks/18loanUpdate.json");
//         LoanUpdateResponse responseData =JsonUtility.FromJson<LoanUpdateResponse>(jsonFile.text);

//         // 디스플레이 아이템 매니저도 업데이트
        
//         // displayManager.SetItemState(currentClickedItem.displayPositionKey, responseData.itemState);
        
//         // // 여기 아이템액션 매니저의 창도 업데이트
//         // OnItemActionTogClicked(itemActionTog.transform.GetChild(0).GetComponent<Toggle>().isOn);
//     }
// }
