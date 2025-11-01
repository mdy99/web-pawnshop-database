using UnityEngine;
using System.Collections.Generic;

public enum affectedPrice{
    Asking_Price =0, // 최초 제시가
    Purchase_Price =1, // 구매가
    Appraised_Price =2, // 감정가
    Selling_Price =3  // 최종 판매가
};

public enum ItemState
{
    Created = 0,         // 생성됨
    OnDisplay = 1,       // 전시 중
    UnderRestoration = 2,// 복원 중
    OnAuction = 3,       // 경매 중
    Sold = 4             // 판매됨
};

public enum Grade
{
    Common = 0,      // 일반
    Rare = 1,        // 레어
    Unique = 2,      // 유니크
    Legendary = 3    // 레전더리
};
// 서버 -> 클라 전송용
[System.Serializable]
public class GameSessionData{
    // 플레이어 테이블
    public string sessionToken; // 세션 토큰 << 구분자
    public string playerId; // 플레이어 고유 ID
    // 게임 세션 테이블
    public int dayCount; // 게임 진행 일수
    public int money; // 잔금
    public int personalDebt; // 남은 개인 빚
    public int pawnshopDebt; // 남은 전당포 빚
    public int unlockedShowcaseCount; // 해금된 쇼케이스 수
    public string nickname; // 플레이어 닉네임
    public string shopName; // 상점 이름
    public int gameEndDayCount; // 게임 종료 시 일수
    public string gameEndDate; // 게임 종료 시 날짜
}

[System.Serializable]
public class ItemData
{
    public int itemKey; // 아이템 키 << 구분자
    // ITEM_CATALOG 테이블
    public string itemName; // 아이템 이름
    public string imgId; // 아이템 이미지 아이디 ITM00001~ITM99999
    public string itemCategoryName; // 아이템 카테고리 이름
    // EXISTING_ITEM 테이블
    public int grade; // 아이템 등급
    public int foundGrade; // 발견된 아이템 등급
    public int flawEa; // 아이템 흠 개수
    public int foundFlawEa; // 발견된 흠 개수
    public float suspiciousFlawAura; // 수상한 흠 기운
    public bool authenticity; // 진위 여부
    public bool isAuthenticityFound; // 진위 여부 발견 여부
    public int itemState; // 아이템 상태
}

[System.Serializable]
public class CustomerData{
    // CUSTOMER_CATALOG 테이블
    public int customerKey; // 고객 키
    public string customerName; // 고객 이름
    public string favoriteCategoryName; // 고객 선호 카테고리 이름
    public string imgId; // 고객 이미지 아이디 CUS00001~CUS99999
    public float fraud; // 사기꾼 기질
    public float wellCollect; // 수집력
    public float clumsy; // 서투름
    // CUSTOMER_HIDDEN_DISCOVERED_IN_GAME_SESSION 테이블
    public bool isFraudDiscovered; // 사기꾼 기질 발견 여부 << 서버에서 000 파싱해서 bool 변환해서 줌
    public bool isWellCollectDiscovered; // 수집력 발견 여부
    public bool isClumsyDiscovered; // 서투름 발견 여부
}

[System.Serializable]
public class DealData{
    public int drcKey; // 거래 키 << 구분자
    public int askingPrice; // 최초 제시가
    public int purchasePrice; // 구매가
    public int appraisedPrice; // 감정가
    public int sellingPrice; // 최종 판매가
    public int boughtDate; // 구매한 게임 내 일수
    public int soldDate; // 판매한 게임 내 일수
    public CustomerData sellerData; // 판매자 고객 데이터
    public CustomerData buyerData; // 구매자 고객 데이터
    public ItemData itemData; // 아이템 데이터
}

[System.Serializable]
public class ItemDisplayData{
    public int displayPositionKey; // 전시 위치 키 << 구분자
    public ItemData itemData; // 전시된 아이템 데이터
}

[System.Serializable]
public class NewsData{
    public string newsDescription;
    public int affectedPrice;
    public string affectedCategoryName; // 영향을 받은 카테고리 이름
    public int amount; // +/- amount 만큼. 서버에서 +30, -70 처리한 후 보내야 함
}

[System.Serializable]
public class LoanData{ // 클라 -> 서버 전송용. (for Update)
    public string debtType; // 빚 종류 (PERSONAL/PAWNSHOP)
    public int Amount; // 대출/상환 금액 (+/-)
}

[System.Serializable]
public class ServerTransferData{ // 클라 -> 서버 전송용
    public string sessionToken; // 세션 토큰 << 구분자
    public GameSessionData gameSessionData; // 게임 세션 데이터
    public ItemData itemDataList;// 아이템 데이터
    public CustomerData customerDataList; // 고객 데이터
    public DealData dealDataList; // 거래 내역
    public LoanData loanDataList; // 대출/상환 내역
    public List<ItemDisplayData> itemDisplayDataList; // 전시된 아이템 데이터 리스트
    public List<NewsData> newsDataList; // 뉴스 데이터 리스트 0~3개
}

