using UnityEngine;

/// <summary>
/// 인력 상점 임시 스크립트.
/// 
/// 현재 개발 단계에서는 인력 고용 버튼을 꺼둔 상태이고,
/// 게임 시작 시 GameInitializer가 학사생 1명을 자동 생성한다.
/// 
/// 예전 StaffShop.cs는 StaffWorker.moneyPerTick 같은
/// 오래된 자동 생산 변수를 사용해서 컴파일 에러를 만들고 있었기 때문에
/// 일단 비워둔 형태로 유지한다.
/// 
/// 나중에 인력 고용 시스템을 다시 만들 때 이 파일을 새 구조로 작성한다.
/// </summary>
public class StaffShop : MonoBehaviour
{
    /// <summary>
    /// 현재는 사용하지 않는다.
    /// 나중에 인력 고용 시스템 구현 시 다시 작성한다.
    /// </summary>
    public void HireStaff(int index)
    {
        Debug.Log("StaffShop은 현재 비활성 개발 단계입니다. 나중에 새 구조로 구현합니다.");
    }
}