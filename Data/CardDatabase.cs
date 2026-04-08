namespace STS2CompanionMod.Data;

/// <summary>
/// 카드 이름 → 소속 아키타입 매핑.
/// 아키타입 점수 계산 시 이 DB를 참조하지 않고 ArchetypeDatabase의 CoreCards/GoodCards를 직접 사용.
/// 여기서는 카드별 일반 평가(범용성)를 관리.
/// </summary>
public static class CardDatabase
{
    // 캐릭터별 항상 좋은 범용 카드 목록
    public static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> UniversallyGoodCards =
        new Dictionary<string, IReadOnlyList<string>>
        {
            ["Ironclad"] = new[]
            {
                "Shrug It Off",     // 범용 블록
                "Pommel Strike",    // 블록 + 드로우
                "Impervious",       // 대량 블록
                "Spot Weakness",    // 엘리트 전용 조건부 강카드
                "Power Through",    // 블록 대량
                "Seeing Red",       // 에너지 가속
                "Offering",         // 에너지 + 드로우 (단 HP 손실)
            },
            ["Silent"] = new[]
            {
                "Backflip",         // 블록 + 드로우
                "Acrobatics",       // 드로우
                "Dodge and Roll",   // 블록 + 리테인
                "Blur",             // 블록 유지
                "Piercing Wail",    // 모든 적 힘 감소
                "Expertise",        // 핸드 가득 채울 때 드로우
            },
            ["Defect"] = new[]
            {
                "Coolheaded",       // 드로우 + 얼음 오브
                "Skim",             // 드로우
                "Compile Driver",   // 드로우 + 피해
                "Defragment",       // 집중도
                "Capacitor",        // 오브 슬롯 확장
            },
            ["Watcher"] = new[]
            {
                "Inner Peace",      // 평온 진입 드로우
                "Evaluate",         // 블록 + 점지
                "Sanctity",         // 스킬 카드 시 드로우
                "Mental Fortress",  // 스탠스 전환 시 블록
                "Talk to the Hand", // 공격 후 블록
            },
        };

    // 항상 피해야 하는 범용 저평가 카드
    public static readonly IReadOnlyList<string> UniversallyBadCards = new[]
    {
        "Normality",    // 코스트 3+ 카드 사용 불가 저주
        "Parasite",     // 제거 시 최대 HP 손실
        "Regret",       // 소지 시 매 전투 HP 손실
        "Shame",        // 소지 시 허약 부여
        "Doubt",        // 소지 시 매 턴 허약
        "Injury",       // 소지 시 상처
        "Pain",         // 소지 시 매 턴 HP 손실
        "Pride",        // 매 턴 덱에 추가되는 저주
        "Clumsy",       // 매 턴 드로우 방해
        "Writhe",       // 항상 핸드에서 시작하는 부담
    };

    // 렐릭 등급 (캐릭터 공통 + 캐릭터 전용)
    public static readonly IReadOnlyDictionary<string, RelicTier> RelicTiers =
        new Dictionary<string, RelicTier>
        {
            // S 티어
            ["Snecko Eye"]          = RelicTier.S,
            ["Philosopher's Stone"] = RelicTier.S,
            ["Runic Dome"]          = RelicTier.S,
            ["Inserter"]            = RelicTier.S,   // Defect 전용
            ["Nunchaku"]            = RelicTier.S,
            ["Dead Branch"]         = RelicTier.S,
            ["Unceasing Top"]       = RelicTier.S,
            ["Runic Pyramid"]       = RelicTier.S,
            ["Velvet Choker"]       = RelicTier.C,   // 6장 제한은 치명적
            ["Ectoplasm"]           = RelicTier.B,   // 골드 획득 불가
            ["Cursed Key"]          = RelicTier.B,
            ["Calling Bell"]        = RelicTier.C,
            // A 티어
            ["Frozen Eye"]          = RelicTier.A,
            ["Mango"]               = RelicTier.A,
            ["Pocketwatch"]         = RelicTier.A,
            ["Pen Nib"]             = RelicTier.A,
            ["Preserved Insect"]    = RelicTier.A,
            ["Vajra"]               = RelicTier.A,
            ["Calipers"]            = RelicTier.A,
            ["Paper Krane"]         = RelicTier.A,
            ["Paper Frog"]          = RelicTier.A,
            ["Emotion Chip"]        = RelicTier.A,
            ["Nilry's Codex"]       = RelicTier.A,
            ["Tingsha"]             = RelicTier.A,
            ["Tough Bandages"]      = RelicTier.A,
            // B 티어
            ["Gremlin Horn"]        = RelicTier.B,
            ["Torii"]               = RelicTier.B,
            ["Odd Mushroom"]        = RelicTier.B,
            ["Orichalcum"]          = RelicTier.B,
            ["Ring of the Snake"]   = RelicTier.B,  // Silent 시작 렐릭
            ["Burning Blood"]       = RelicTier.B,  // Ironclad 시작 렐릭
            ["Cracked Core"]        = RelicTier.B,  // Defect 시작 렐릭
            ["Pure Water"]          = RelicTier.B,  // Watcher 시작 렐릭
        };
}

public enum RelicTier { S, A, B, C }
