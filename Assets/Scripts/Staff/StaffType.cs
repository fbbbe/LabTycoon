/// <summary>
/// 인력 종류를 구분하는 enum.
/// 
/// 나중에 인력이 의자에 앉으면 화면 이미지는
/// "의자+인력 합성 이미지"로 바뀐다.
/// 
/// 하지만 게임 로직에서는 이 인력이 학사생인지, 석사생인지, 박사생인지
/// 계속 추적해야 한다.
/// </summary>
public enum StaffType
{
    Undergraduate, // 학사생
    Master,        // 석사생
    PhD            // 박사생
}