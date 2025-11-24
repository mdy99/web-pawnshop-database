using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;

// ======================================================
// ENUM 정의
// ======================================================
public enum AffectedPrice
{
    AskingPrice = 0,     // 최초 제시가
    PurchasePrice = 1,   // 구매가
    AppraisedPrice = 2,  // 감정가
    SellingPrice = 3     // 최종 판매가
}

public enum ItemState
{
    Created = 0,         // 생성됨
    OnDisplay = 1,       // 전시 중
    UnderRestoration = 2,// 복원 중
    OnAuction = 3,       // 경매 중
    Sold = 4,             // 판매 완료
    AfterRestoration = 5  // 복원 완료
}

public enum Grade
{
    Common = 0,      // 일반
    Rare = 1,        // 레어
    Unique = 2,      // 유니크
    Legendary = 3    // 레전더리
}

public enum Authenticity
{
    Unknown = -1, // 미확정
    Real = 1, // 진품
    Fake = 0 // 가품
}

// ======================================================
// 플레이어 및 세션 관련
// ======================================================

[System.Serializable]
public class PlayerRegisterLoginRequest
{
    public string playerId;
    public string password;
}

[System.Serializable]
public class LoginResponse
{
    public string sessionToken;
    public string hasGameSession; // "Y", "N"
}


// POST /game-session/new
[System.Serializable]
public class NewGameSessionRequest
{
    public string nickname;
    public string shopName;
}


// POST /player/login
// POST /game-session/new
// POST /game-session/latest
[System.Serializable]
public class GameSessionData
{
    public string sessionToken;  
    public string playerId;      
    public int dayCount;         
    public int money;            
    public int personalDebt;     
    public int pawnshopDebt;     
    public int unlockedShowcaseCount; 
    public string nickname;      
    public string shopName;      
    public int gameEndDayCount;  
    public string gameEndDate;   
}
// 중앙에서 static으로 관리할 데이터

// ======================================================
// 카탈로그 데이터 (초기 로드)
// ======================================================
[System.Serializable]
public class ItemCatalogData
{
    public int itemCatalogKey;
    public string itemCatalogName;
    public string imgId;
    public string categoryName; 
}

[System.Serializable]
public class CustomerCatalogData
{
    public int customerKey;
    public string customerName;
    public string favoriteCategoryName;
    public string imgId;
}

[System.Serializable]
public class InitialCatalogResponse
{
    public List<ItemCatalogData> itemCatalogs;
    public List<CustomerCatalogData> customerCatalogs;
}
// 중앙에서 static으로 관리할 데이터

// ======================================================
// 전시장 아이템 조회
// GET /display/currentAll
// ======================================================
[System.Serializable]
public class DisplayedItemData
{
    public int displayPositionKey;
    public int askingPrice;
    public int purchasePrice;
    public int appraisedPrice;
    public int boughtDate;
    public string sellerName;
    public Grade foundGrade;
    public int foundFlawEa;
    public int foundAuthenticity; // 1 진품 0 가품 -1 미확정
    public ItemState itemState;
    public int itemKey;
    public int itemCatalogKey;
}

[System.Serializable]
public class ItemDisplaysWrapData
{
    public List<DisplayedItemData> displays;
}

// ======================================================
// 뉴스 (당일 이벤트)
// GET /news/current
// ======================================================
[System.Serializable]
public class NewsData
{
    public string newsDescription;
    public int affectedPrice;
    public string affectedCategoryName;
    public string amount;
}

[System.Serializable]
public class NewsWrapData
{
    public List<NewsData> newsList;
}

// ======================================================
// 고객 정보 공개
// PATCH /customer/reveal
// ======================================================
[System.Serializable]
public class RevealCustomerRequest
{
    public int customerKey;
    public string attribute; // FRAUD, WELL_COLLECT, CLUMSY
}

[System.Serializable]
public class RevealCustomerResponse
{
    public string attribute; // FRAUD, WELL_COLLECT, CLUMSY
    public float value; // 0.0~1.0
    public int leftMoney; // 액션 후 남은 돈
}

// ======================================================
// 아이템 힌트
// GET /item/getHints
// ======================================================
[System.Serializable]
public class ItemHintRequest
{
    public int itemKey;
}

[System.Serializable]
public class ItemHintResponse
{
    public string hintName;
    public string hintValue;
    public int leftMoney;
}

// ======================================================
// 거래 생성 및 액션
// POST /deal/generateDailyDeals
// POST /deal/action
// ======================================================
[System.Serializable]
public class DealData
{
    public int drcKey;
    public int askingPrice;
    public int purchasePrice;
    public int appraisedPrice;
    public int itemKey;
    public int itemCatalogKey;
    public int foundGrade;
    public int foundFlawEa;
    public int foundAuthenticity; // 1 진품 0 가품 -1 미확정
    public int customerKey;
    public float revealedFraud; // -1 미열람, float 일반이면 열람 된거
    public float revealedWellCollect;
    public float revealedClumsy;
}

[System.Serializable]
public class DailyDealsWrapData
{
    public List<DealData> dailyDeals;
}

[System.Serializable]
public class DealActionRequest
{
    public int drcKey;
    public string actionType; // FINDFLAW, AUTHCHECK, APPRAISE
    public int actionLevel;   // 1~3
}

[System.Serializable]
public class DealActionResponse
{
    public int totalPurchasePrice;
    public int totalAppraisedPrice;
    public string changedPurchasedPriceByAction;
    public string changedAppraisedPriceByAction;
    public int foundGrade;
    public int foundFlawEa;
    public int foundAuthenticity; // 1 진품 0 가품 -1 미확정
    public int leftMoney;
}

// ======================================================
// 거래 성사 / 거부
// POST /deal/complete
// POST /deal/cancel
// ======================================================
[System.Serializable]
public class DealCompleteRequest
{
    public int drcKey;
    public int itemKey;
}

[System.Serializable]
public class DealCompleteResponse
{
    public string dealSuccess; // "Y", "N"
    public int leftMoney;
    public DisplayedItemData displayedItem;
    public string isGameOvered; // "Y" | "N" // 아직 게임 진행 중이면 N
    public string isDayNext; //  "Y" | "N" // 아직 거래가 남았으면, N.
    public DayNextData dayNext; // 다음 날 넘어가는 정보들
    public DayFinalizeData dayFinalize; // 정산 정보
    public WorldRecordData worldRecord; // 게임 종료 정보
}

[System.Serializable]
public class DenyDealResponse
{
    public string isGameOvered; // "Y" | "N" // 아직 게임 진행 중이면 N
    public string isDayNext; //  "Y" | "N" // 아직 거래가 남았으면, N.
    public DayNextData dayNext; // 다음 날 넘어가는 정보들
    public DayFinalizeData dayFinalize; // 정산 정보
    public WorldRecordData worldRecord; // 게임 종료 정보
}

// ======================================================
// 아이템 복원 / 경매
// POST /item/action
// POST /item/result
// ======================================================
[System.Serializable]
public class ItemActionRequest
{
    public string actionType; // restore, auction
    public int itemKey;
}

[System.Serializable]
public class ItemActionResponse
{
    public ItemState itemState;
}

[System.Serializable]
public class ActionItemData
{
    public int itemKey;
    public int itemState;
    public string resultMoney; // 변화값. 경매면은 벌은 돈. 복원이면 복원 비용
    public int appraisedPrice; // 감정가 변화비용
}

[System.Serializable]
public class ItemActionResultResponse
{
    public List<ActionItemData> actionResults;
    public int leftMoney;
    public string isGameOvered; // "Y" | "N" . 게임 오버 되었으면, "Y"
    public WorldRecordData worldRecord; // 게임 종료 정보
}

// ======================================================
// 아이템 판매
// POST /item/sellStart
// POST /item/sellComplete
// ======================================================
[System.Serializable]
public class SellStartRequest
{
    public int itemKey;
    public int customerKey;
}

[System.Serializable]
public class SellStartResponse
{
    public int sellingPrice;
}

[System.Serializable]
public class SellCompleteRequest
{
    public int itemKey;
    public int customerKey;
}

[System.Serializable]
public class SellCompleteResponse
{
    public string earnedAmount;
    public int leftMoney;
    public int displayedPositionKey;
}

// ======================================================
// 대출 / 상환
// POST /loan/update
// ======================================================
[System.Serializable]
public class LoanUpdateRequest
{
    public string debtType; // PERSONAL / PAWNSHOP
    public int amount;
}

[System.Serializable]
public class LoanUpdateResponse
{
    public string debtType;
    public int leftDebtAmount;
    public int leftMoney;
    public string isGameCleared; // 게임 클리어 여부 "Y" , "N"
    public WorldRecordData worldRecord; // 게임 종료 기록
}

// ======================================================
// 하루 종료 (정산)
// POST /day/next
// ======================================================
[System.Serializable]
public class DayFinalizeData
{
    public int startMoney;
    public int todayEndMoney;
    public int interest;
    public int weeklyInterest;
    public int finalMoney;
}

[System.Serializable]
public class DayNextData
{
    public int dayCount;
    public int leftMoney;
    public int personalDebt;
    public int pawnshopDebt;
}

// ======================================================
// 세계 기록 조회 (랭킹)
// GET /worldRecords
// ======================================================
[System.Serializable]
public class WorldRecordData
{
    public string playerId;
    public string nickname;
    public string pawnshopName;
    public int gameEndDayCount;
    public string gameEndDate;
}

[System.Serializable]
public class WorldRecordResponse
{
    public List<WorldRecordData> worldRecords;
}

// ======================================================
// 에러 응답
// 모든 400 Bad Request 공통
// ======================================================
[System.Serializable]
public class ErrorResponse
{
    public string error;
}
