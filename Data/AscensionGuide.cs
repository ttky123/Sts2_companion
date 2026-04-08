namespace STS2CompanionMod.Data;

/// <summary>
/// 승천 단계별 공통 전략 가이드 및 지도 선택 조언.
/// </summary>
public static class AscensionGuide
{
    public record AscensionTip(
        int MinLevel,
        int MaxLevel,
        string Label,
        string[] GeneralAdvice,
        MapAdvice Map,
        string CardPickPhilosophy,
        string RelicPickPhilosophy
    );

    public record MapAdvice(
        string Elites,
        string Shops,
        string RestSites,
        string Events
    );

    public static readonly IReadOnlyList<AscensionTip> Tips = new[]
    {
        new AscensionTip(
            MinLevel: 0, MaxLevel: 7,
            Label: "입문 (A0–A7)",
            GeneralAdvice: new[]
            {
                "다양한 아키타입을 시도해보며 캐릭터 학습에 집중.",
                "보상에서 업그레이드보다 카드 추가를 우선하는 시기.",
                "보스 상대로 1~2장의 강한 피해 카드가 있으면 충분.",
                "렐릭은 어떤 것이든 일단 가져가도 됨 (저주 렐릭 제외).",
                "죽더라도 덱 구성 실험이 더 중요한 단계.",
            },
            Map: new MapAdvice(
                Elites: "HP 75% 이상이면 엘리트 전투 도전. 렐릭 획득 가치가 높음.",
                Shops: "카드 제거(Remove) 우선. 스트라이크/디펜드 제거가 덱 얇게 만드는 핵심.",
                RestSites: "HP 50% 이하면 휴식. 그 이상이면 업그레이드.",
                Events: "이벤트 선택지에서 HP 손실보다 카드/렐릭 획득을 선택."
            ),
            CardPickPhilosophy: "덱에 필요한 역할(피해/블록/드로우)을 보완하는 방향으로 픽.",
            RelicPickPhilosophy: "모든 렐릭이 가치 있음. 저주 렐릭(Calling Bell 등)은 신중하게."
        ),
        new AscensionTip(
            MinLevel: 8, MaxLevel: 14,
            Label: "중급 (A8–A14)",
            GeneralAdvice: new[]
            {
                "A8부터 보스가 추가 어려움을 부여. 초반 전략이 중요.",
                "카드 드로우와 에너지 효율에 집중. 코스트 낭비 카드 지양.",
                "덱 30장 이상이면 일관성 하락. 카드 제거를 더 적극적으로.",
                "엘리트 전투에서 렐릭 획득이 중후반 스케일링의 핵심.",
                "시너지 없는 카드는 건너뛰기. 얇고 효율적인 덱 유지.",
            },
            Map: new MapAdvice(
                Elites: "HP 60% 이상이면 도전. 엘리트 렐릭이 빌드 방향 결정.",
                Shops: "카드 제거 최우선. 그 다음 핵심 카드 구매.",
                RestSites: "HP 60% 이하면 휴식. 핵심 카드 업그레이드는 전투 전에.",
                Events: "리스크가 있는 이벤트 선택지는 신중하게. HP 여유분 확인."
            ),
            CardPickPhilosophy: "현재 아키타입의 코어 카드를 우선. 없으면 범용 가치 카드.",
            RelicPickPhilosophy: "S/A 티어 렐릭 중심. 빌드와 시너지 있는 렐릭 우선."
        ),
        new AscensionTip(
            MinLevel: 15, MaxLevel: 20,
            Label: "고급 (A15–A20)",
            GeneralAdvice: new[]
            {
                "A15+부터 보스가 디버프/추가 행동 부여. 완성도 높은 빌드 필수.",
                "카드 1장 1장이 전부 덱에 기여해야 함. 애매한 카드 없이.",
                "업그레이드 우선순위: 코어 카드 > 블록 카드 > 나머지.",
                "불사신 몬스터 처리 방법 미리 준비 (Shiv 누적, 독 누적 등).",
                "보스 렐릭 선택이 런의 방향을 결정. 신중하게 선택.",
                "패턴 외우기: 엘리트/보스 행동 패턴 숙지 필요.",
            },
            Map: new MapAdvice(
                Elites: "빌드 완성도와 HP 여유 동시 확인. 잘못된 타이밍의 엘리트 = 런 실패.",
                Shops: "카드 제거 + 포션 구매. 핵심 렐릭이 있으면 골드 투자 가치 있음.",
                RestSites: "스미스(Smith) 이벤트 활용. 핵심 카드 업그레이드 = 승리 가능성 대폭 상승.",
                Events: "리스크 이벤트 회피 권장. 안정적인 진행이 우선."
            ),
            CardPickPhilosophy: "코어 카드 없으면 건너뛰기. 덱 일관성이 최우선.",
            RelicPickPhilosophy: "S 티어 렐릭만 적극 픽. 빌드 방향 바꾸는 렐릭은 현재 덱 상태 고려."
        ),
    };

    public static AscensionTip GetTip(int ascensionLevel)
    {
        foreach (var tip in Tips)
        {
            if (ascensionLevel >= tip.MinLevel && ascensionLevel <= tip.MaxLevel)
                return tip;
        }
        return Tips[^1]; // 기본값: 고급
    }

    // 승천 단계별 보스 보상 렐릭 픽 가이드
    public static string GetBossRelicAdvice(int ascensionLevel, string characterId) =>
        ascensionLevel switch
        {
            <= 7 => "보스 렐릭은 대부분 가치 있음. Snecko Eye, Runic Dome, Philosopher's Stone 최우선.",
            <= 14 => "빌드 시너지 확인 필수. Runic Pyramid(Watcher), Inserter(Defect) 등 캐릭터 특화 우선.",
            _ => characterId switch
            {
                "Ironclad" => "Runic Dome, Philosopher's Stone, Coffee Dripper 우선. Snecko Eye는 코스트 의존도 높으면 위험.",
                "Silent" => "Snecko Eye, Runic Dome, Empty Cage 우선. Unceasing Top도 버리기 엔진에 강력.",
                "Defect" => "Inserter, Runic Pyramid, Snecko Eye 우선. Echo Form 있으면 Philosopher's Stone도 강력.",
                "Watcher" => "Runic Pyramid, Paper Frog, Emotion Chip 우선. 신성 빌드엔 Philosopher's Stone.",
                _ => "S 티어 렐릭 중 빌드 시너지 확인 후 픽.",
            }
        };
}
