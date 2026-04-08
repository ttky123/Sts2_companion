using STS2CompanionMod.Analysis;

namespace STS2CompanionMod.Data;

/// <summary>
/// 캐릭터별 덱 아키타입 정의. STS2 Early Access 기준 (지속 업데이트 필요).
/// </summary>
public static class ArchetypeDatabase
{
    private static readonly List<ArchetypeData> All = new()
    {
        // ─── IRONCLAD ────────────────────────────────────────────────────────
        new ArchetypeData(
            Id: "ironclad_strength",
            CharacterId: "Ironclad",
            Name: "힘 빌드",
            NameEn: "Strength Build",
            Description: "힘을 쌓아서 공격 카드들의 피해를 극대화하는 빌드.",
            WinCondition: "Demon Form + Limit Break으로 힘을 무한 증폭, Heavy Blade로 마무리.",
            Difficulty: Difficulty.Medium,
            CoreCards: new[] { "Demon Form", "Limit Break", "Heavy Blade", "Inflame", "Feed" },
            GoodCards: new[] { "Spot Weakness", "Thunderclap", "Twin Strike", "Pommel Strike", "Shockwave" },
            AvoidCards: new[] { "Normality", "Parasite", "Regret" },
            KeyRelics: new[] { "Vajra", "Bag of Marbles", "Red Skull", "Akabeko", "Philosopher's Stone" },
            AscensionViability: new AscensionViability(Low: 5, Mid: 4, High: 3),
            AscensionNotes: new AscensionNotes(
                Low: "자유롭게 힘 카드 쌓을 수 있음. Limit Break 업그레이드 우선.",
                Mid: "Demon Form 전에 버티는 구조 필요. Impervious 1장 이상 확보 권장.",
                High: "A15+에서 Demon Form 나오기 전 죽는 경우 많음. 블록 카드 최소 3장 + Shrug It Off 필수.")
        ),
        new ArchetypeData(
            Id: "ironclad_barricade",
            CharacterId: "Ironclad",
            Name: "바리케이드 빌드",
            NameEn: "Barricade Build",
            Description: "블록을 유지하며 Body Slam으로 피해를 주는 빌드.",
            WinCondition: "Barricade로 블록을 소진 없이 유지, Body Slam으로 블록=피해 전환.",
            Difficulty: Difficulty.Hard,
            CoreCards: new[] { "Barricade", "Body Slam", "Entrench", "Impervious", "Shrug It Off" },
            GoodCards: new[] { "Flame Barrier", "True Grit", "Ghostly Armor", "Power Through", "Sentinel" },
            AvoidCards: new[] { "Thunderclap", "Clash", "Wild Strike" },
            KeyRelics: new[] { "Calipers", "Paper Krane", "Orichalcum", "Torii" },
            AscensionViability: new AscensionViability(Low: 3, Mid: 4, High: 5),
            AscensionNotes: new AscensionNotes(
                Low: "Barricade 없으면 의미 없는 빌드. 엘리트 전에서 Barricade 획득 우선.",
                Mid: "Calipers 렐릭 있으면 극강. Body Slam 업그레이드 필수.",
                High: "A15+에서 가장 안정적인 아이언클래드 빌드 중 하나. 블록 스케일이 우수.")
        ),
        new ArchetypeData(
            Id: "ironclad_exhaust",
            CharacterId: "Ironclad",
            Name: "소진 빌드",
            NameEn: "Exhaust Build",
            Description: "카드 소진 시 발동되는 Dark Embrace, Feel No Pain으로 드로우와 블록을 확보.",
            WinCondition: "Corruption으로 모든 스킬을 소진, Dark Embrace로 드로우, Dead Branch로 무작위 카드 생성.",
            Difficulty: Difficulty.Medium,
            CoreCards: new[] { "Corruption", "Dark Embrace", "Feel No Pain", "True Grit", "Sentinel" },
            GoodCards: new[] { "Fiend Fire", "Dead Branch", "Offering", "Disarm", "Seeing Red" },
            AvoidCards: new[] { "Clash", "Parasite", "Regret" },
            KeyRelics: new[] { "Dead Branch", "Charon's Ashes", "Pocketwatch", "Strange Spoon" },
            AscensionViability: new AscensionViability(Low: 4, Mid: 4, High: 4),
            AscensionNotes: new AscensionNotes(
                Low: "Corruption 전에도 True Grit + Sentinel 조합으로 충분히 효과적.",
                Mid: "Dead Branch 렐릭 + Corruption = 거의 무한 카드 생성. 코어 컴보 우선 완성.",
                High: "Corruption 찾기 전까지 취약. 초반 안정성 위해 방어 카드 확보 필수.")
        ),
        new ArchetypeData(
            Id: "ironclad_perfected_strike",
            CharacterId: "Ironclad",
            Name: "퍼펙티드 스트라이크",
            NameEn: "Perfected Strike",
            Description: "덱의 스트라이크 계열 카드 수만큼 Perfected Strike 피해 증가.",
            WinCondition: "덱을 스트라이크로 채워 Perfected Strike를 초고피해 공격으로 만들기.",
            Difficulty: Difficulty.Easy,
            CoreCards: new[] { "Perfected Strike", "Twin Strike", "Pommel Strike", "Wild Strike", "Sword Boomerang" },
            GoodCards: new[] { "Thunderclap", "Clothesline", "Shrug It Off", "Flex" },
            AvoidCards: new[] { "Barricade", "Corruption", "Fiend Fire" },
            KeyRelics: new[] { "Striking Dummy", "Vajra", "Pen Nib" },
            AscensionViability: new AscensionViability(Low: 5, Mid: 3, High: 1),
            AscensionNotes: new AscensionNotes(
                Low: "A0-7에서 매우 강력. 스트라이크 계열 카드 7개 이상이면 Perfected Strike 30+ 피해.",
                Mid: "A8+에서 후반 보스 상대로 피해량이 부족해짐. 업그레이드 필수.",
                High: "A15+에서 권장하지 않음. 피해 스케일링 부족, 빠른 클리어 불가.")
        ),

        // ─── SILENT ──────────────────────────────────────────────────────────
        new ArchetypeData(
            Id: "silent_poison",
            CharacterId: "Silent",
            Name: "독 빌드",
            NameEn: "Poison Build",
            Description: "독을 쌓고 Catalyst로 폭발적으로 증폭시키는 빌드.",
            WinCondition: "Deadly Poison + Catalyst(업) 콤보로 순식간에 치명적인 독 스택 달성.",
            Difficulty: Difficulty.Easy,
            CoreCards: new[] { "Catalyst", "Deadly Poison", "Bouncing Flask", "Envenom", "Corpse Explosion" },
            GoodCards: new[] { "Crippling Cloud", "A Thousand Cuts", "Blur", "Piercing Wail", "Well-Laid Plans" },
            AvoidCards: new[] { "Clash", "Injury", "Pain" },
            KeyRelics: new[] { "Twisted Funnel", "Snecko Eye", "Nilry's Codex", "Frozen Eye" },
            AscensionViability: new AscensionViability(Low: 5, Mid: 5, High: 4),
            AscensionNotes: new AscensionNotes(
                Low: "가장 쉽게 구성 가능한 Silent 빌드. Catalyst 업그레이드가 가장 중요한 우선순위.",
                Mid: "A8-14에서 독이 느려지는 구간. Crippling Cloud + Blur로 생존성 보완 필요.",
                High: "A15+에서도 강력하지만 독 저항 몬스터 주의. Twisted Funnel 렐릭이 있으면 최강.")
        ),
        new ArchetypeData(
            Id: "silent_shiv",
            CharacterId: "Silent",
            Name: "표창 폭풍",
            NameEn: "Shiv Storm",
            Description: "0코스트 표창(Shiv)을 대량 생성하여 Accuracy + After Image로 가치 극대화.",
            WinCondition: "Infinite Blades + Accuracy로 무한 표창 생성 → 턴당 수십 피해.",
            Difficulty: Difficulty.Hard,
            CoreCards: new[] { "Accuracy", "After Image", "Blade Dance", "Cloak and Dagger", "Infinite Blades" },
            GoodCards: new[] { "Flying Knee", "Quick Slash", "Prepared", "Flechettes", "Noxious Fumes" },
            AvoidCards: new[] { "Catalyst" },
            KeyRelics: new[] { "Nunchaku", "Pen Nib", "Kunai", "Wrist Blade", "Shuriken" },
            AscensionViability: new AscensionViability(Low: 3, Mid: 4, High: 5),
            AscensionNotes: new AscensionNotes(
                Low: "Accuracy 없이는 표창이 너무 약함. Blade Dance로 피해 테스트 먼저.",
                Mid: "After Image 추가 시 표창 = 블록 겸용. 매우 강력한 조합.",
                High: "A15+에서 최고의 Silent 빌드 중 하나. 충분한 표창 생성 수단 확보 필요.")
        ),
        new ArchetypeData(
            Id: "silent_dexterity",
            CharacterId: "Silent",
            Name: "민첩 방어 빌드",
            NameEn: "Dexterity Build",
            Description: "Footwork로 민첩도를 쌓아 Deflect 한 장으로도 대량의 블록 확보.",
            WinCondition: "Footwork 여러 장 + Well-Laid Plans으로 블록 영구 유지.",
            Difficulty: Difficulty.Medium,
            CoreCards: new[] { "Footwork", "Well-Laid Plans", "Deflect", "Blur", "Backflip" },
            GoodCards: new[] { "Tactician", "Concentrate", "Setup", "Dodge and Roll", "Expertise" },
            AvoidCards: new[] { "Crippling Cloud" },
            KeyRelics: new[] { "Paper Crane", "Gremlin Horn", "Ring of the Snake", "Frozen Eye" },
            AscensionViability: new AscensionViability(Low: 4, Mid: 4, High: 3),
            AscensionNotes: new AscensionNotes(
                Low: "Footwork 2장 이상이면 매우 안정적. 방어적 플레이 선호 시 추천.",
                Mid: "공격 수단이 부족해지는 경우 많음. A Thousand Cuts 보완 권장.",
                High: "A15+에서 피해 출력 부족으로 한계. 공격 요소 병행 필수.")
        ),
        new ArchetypeData(
            Id: "silent_discard",
            CharacterId: "Silent",
            Name: "버리기 엔진",
            NameEn: "Discard Engine",
            Description: "Tactician + Reflex로 카드를 버릴 때마다 에너지/드로우 획득하는 엔진 빌드.",
            WinCondition: "Calculated Gamble + Tactician 루프로 무한 드로우 엔진 구성.",
            Difficulty: Difficulty.Hard,
            CoreCards: new[] { "Tactician", "Reflex", "Calculated Gamble", "Acrobatics", "Expertise" },
            GoodCards: new[] { "Dagger Throw", "Setup", "Concentrate", "Glass Knife" },
            AvoidCards: new[] { "Injury", "Pride" },
            KeyRelics: new[] { "Tingsha", "Tough Bandages", "Unceasing Top", "Dead Branch" },
            AscensionViability: new AscensionViability(Low: 2, Mid: 4, High: 5),
            AscensionNotes: new AscensionNotes(
                Low: "초반엔 콤보 완성이 어려움. Tactician 1장만으로도 충분히 테스트 가능.",
                Mid: "Reflex + Tactician 양쪽 갖추면 폭발적 엔진 완성.",
                High: "A15+에서 완성된 버리기 엔진은 거의 무적. Unceasing Top 렐릭 최우선.")
        ),

        // ─── DEFECT ───────────────────────────────────────────────────────────
        new ArchetypeData(
            Id: "defect_lightning",
            CharacterId: "Defect",
            Name: "번개 오브 빌드",
            NameEn: "Lightning Build",
            Description: "번개 오브를 대량 채널링하여 패시브 피해를 극대화.",
            WinCondition: "Electrodynamics + Storm으로 매 카드 사용마다 번개 오브 발동.",
            Difficulty: Difficulty.Easy,
            CoreCards: new[] { "Electrodynamics", "Storm", "Thunder Strike", "Zap", "Charge Battery" },
            GoodCards: new[] { "Ball Lightning", "Darkness", "Amplify", "Defragment", "Loop" },
            AvoidCards: new[] { "Static Discharge" },
            KeyRelics: new[] { "Inserter", "Cracked Core", "Runic Dome", "Frozen Eye" },
            AscensionViability: new AscensionViability(Low: 5, Mid: 4, High: 3),
            AscensionNotes: new AscensionNotes(
                Low: "가장 쉬운 Defect 빌드. Electrodynamics 획득 즉시 강해짐.",
                Mid: "오브 슬롯 확장(Inserter 렐릭) 있으면 대폭 강해짐.",
                High: "A15+에서 대미지 스케일링 부족. Echo Form 조합 필요.")
        ),
        new ArchetypeData(
            Id: "defect_frost",
            CharacterId: "Defect",
            Name: "얼음 오브 방어 빌드",
            NameEn: "Frost Build",
            Description: "얼음 오브를 통해 대량의 블록을 쌓고 Blizzard로 피해 전환.",
            WinCondition: "Glacier + Blizzard로 블록 = 피해 변환, 생존하며 갈아버리기.",
            Difficulty: Difficulty.Medium,
            CoreCards: new[] { "Glacier", "Blizzard", "Cold Snap", "Ice Storm", "Coolheaded" },
            GoodCards: new[] { "Chill", "Equilibrium", "Defragment", "Consume", "Recursion" },
            AvoidCards: new[] { "Static Discharge", "Dark Matter" },
            KeyRelics: new[] { "Frozen Core", "Inserter", "Frozen Eye", "Mango" },
            AscensionViability: new AscensionViability(Low: 3, Mid: 4, High: 5),
            AscensionNotes: new AscensionNotes(
                Low: "방어적 플레이로 안정적이나 클리어가 느림.",
                Mid: "Blizzard 업그레이드 시 공격/방어 겸용으로 매우 강력.",
                High: "A15+에서 가장 안정적인 Defect 빌드. Frozen Core + Inserter = 필수.")
        ),
        new ArchetypeData(
            Id: "defect_echo_form",
            CharacterId: "Defect",
            Name: "에코 폼 파워 빌드",
            NameEn: "Echo Form Build",
            Description: "Echo Form으로 강력한 파워 카드를 2번씩 발동하여 스케일링.",
            WinCondition: "Echo Form + Creative AI/Machine Learning 조합으로 폭발적 가치 확보.",
            Difficulty: Difficulty.Hard,
            CoreCards: new[] { "Echo Form", "Creative AI", "Machine Learning", "Defragment", "Amplify" },
            GoodCards: new[] { "Meteor Strike", "Doom and Gloom", "Consume", "Darkness", "Rainbow" },
            AvoidCards: new[] { "Normality" },
            KeyRelics: new[] { "Runic Pyramid", "Paper Frog", "Inserter", "Snecko Eye" },
            AscensionViability: new AscensionViability(Low: 3, Mid: 4, High: 5),
            AscensionNotes: new AscensionNotes(
                Low: "설정 시간이 필요하므로 초반엔 느림. Amplify 우선 확보.",
                Mid: "Echo Form 이후 Creative AI가 핵심. 스케일링이 매우 강력.",
                High: "A15+에서 완성 시 거의 무적. Echo Form 업그레이드 최우선.")
        ),
        new ArchetypeData(
            Id: "defect_rainbow",
            CharacterId: "Defect",
            Name: "레인보우 빌드",
            NameEn: "Rainbow Build",
            Description: "번개/얼음/암흑/플라즈마 오브를 모두 활용하여 유연하게 대응.",
            WinCondition: "다양한 오브 효과를 상황에 맞게 선택적으로 발동.",
            Difficulty: Difficulty.Medium,
            CoreCards: new[] { "Rainbow", "Dark Matter", "Doom and Gloom", "Amplify", "Defragment" },
            GoodCards: new[] { "Zap", "Cold Snap", "Ball Lightning", "Recursion", "Loop" },
            AvoidCards: new[] { "Normality" },
            KeyRelics: new[] { "Inserter", "Pocketwatch", "Snecko Eye" },
            AscensionViability: new AscensionViability(Low: 4, Mid: 3, High: 3),
            AscensionNotes: new AscensionNotes(
                Low: "유연하고 재미있는 빌드. 다양한 대응 가능.",
                Mid: "오브 슬롯이 충분하지 않으면 효율이 떨어짐. Inserter 렐릭 필수.",
                High: "A15+에서 단일 오브 특화 빌드보다 약함. 권장하지 않음.")
        ),

        // ─── WATCHER ─────────────────────────────────────────────────────────
        new ArchetypeData(
            Id: "watcher_divinity",
            CharacterId: "Watcher",
            Name: "신성 러시",
            NameEn: "Divinity Rush",
            Description: "분노 스탠스를 여러 번 전환하여 신성(Divinity) 상태 진입 → 피해 3배.",
            WinCondition: "Worship/Pray로 경건함 쌓기 → 신성 진입 → 한 턴에 적 처치.",
            Difficulty: Difficulty.Hard,
            CoreCards: new[] { "Worship", "Pray", "Reach Heaven", "Omniscience", "Spirit Shield" },
            GoodCards: new[] { "Indignation", "Evaluate", "Mental Fortress", "Carve Reality", "Talk to the Hand" },
            AvoidCards: new[] { "Doubt", "Shame", "Injury" },
            KeyRelics: new[] { "Emotion Chip", "Paper Frog", "Frozen Eye" },
            AscensionViability: new AscensionViability(Low: 3, Mid: 4, High: 5),
            AscensionNotes: new AscensionNotes(
                Low: "A0-7에서 신성에 도달하기 어려움. Wrath 사이클링 빌드가 더 안정적.",
                Mid: "Worship 3장 이상이면 신성 도달 일관성 확보.",
                High: "A15+에서 가장 강력한 Watcher 빌드. 한 턴 킬 가능.")
        ),
        new ArchetypeData(
            Id: "watcher_rushdown",
            CharacterId: "Watcher",
            Name: "러시다운 드로우",
            NameEn: "Rushdown Draw",
            Description: "Rushdown으로 분노 스탠스 진입 시마다 카드 2장 드로우하여 무한 콤보.",
            WinCondition: "Rushdown + Tantrum 루프로 매 턴 무제한 카드 드로우.",
            Difficulty: Difficulty.Medium,
            CoreCards: new[] { "Rushdown", "Tantrum", "Inner Peace", "Ragnarok", "Vigilance" },
            GoodCards: new[] { "Evaluate", "Carve Reality", "Sanctity", "Battle Hymn", "Worship" },
            AvoidCards: new[] { "Doubt", "Shame" },
            KeyRelics: new[] { "Runic Pyramid", "Paper Frog", "Tingsha" },
            AscensionViability: new AscensionViability(Low: 4, Mid: 5, High: 4),
            AscensionNotes: new AscensionNotes(
                Low: "Rushdown 1장으로도 강력한 드로우 엔진 구성 가능.",
                Mid: "Tantrum 업그레이드 + Rushdown 콤보로 폭발적 효율. 가장 안정적인 구간.",
                High: "A15+에서 매우 강력하나 Curse 카드 주의. Inner Peace로 안정화 필수.")
        ),
        new ArchetypeData(
            Id: "watcher_scry",
            CharacterId: "Watcher",
            Name: "점지/기적 빌드",
            NameEn: "Scry + Miracle",
            Description: "대량의 점지(Scry)로 덱을 완전히 제어하고 기적(Miracle)으로 무제한 에너지.",
            WinCondition: "Alpha → Beta → Omega 연계로 강력한 피해, Foresight로 지속 점지.",
            Difficulty: Difficulty.Easy,
            CoreCards: new[] { "Foresight", "Brilliance", "Pray", "Evaluate", "Pressure Points" },
            GoodCards: new[] { "Alpha", "Collect", "Conjure Blade", "Sanctity", "Weave" },
            AvoidCards: new[] { "Doubt", "Injury" },
            KeyRelics: new[] { "Frozen Eye", "Pocketwatch", "Nunchaku" },
            AscensionViability: new AscensionViability(Low: 5, Mid: 4, High: 3),
            AscensionNotes: new AscensionNotes(
                Low: "초보자에게 가장 추천하는 Watcher 빌드. 안정적이고 직관적.",
                Mid: "Alpha-Beta-Omega 체인 완성 시 매우 강력.",
                High: "A15+에서 Omega 피해 스케일링 부족. Brilliance 업그레이드로 보완.")
        ),
        new ArchetypeData(
            Id: "watcher_blasphemy",
            CharacterId: "Watcher",
            Name: "신성 모독 빌드",
            NameEn: "Blasphemy Build",
            Description: "Blasphemy로 즉시 신성 진입 후 다음 턴 사망 전에 적을 처치.",
            WinCondition: "Blasphemy → 신성 상태 3배 피해 → 한 턴에 처치 또는 Safety 카드로 사망 회피.",
            Difficulty: Difficulty.Hard,
            CoreCards: new[] { "Blasphemy", "Vault", "Spirit Shield", "Worship", "Scrawl" },
            GoodCards: new[] { "Pray", "Mental Fortress", "Omniscience", "Ragnarok" },
            AvoidCards: new[] { "Doubt", "Shame", "Regret" },
            KeyRelics: new[] { "Emotion Chip", "Mark of the Bloom", "Frozen Eye" },
            AscensionViability: new AscensionViability(Low: 2, Mid: 3, High: 4),
            AscensionNotes: new AscensionNotes(
                Low: "리스크가 높아 비추천. Vault 없으면 즉사.",
                Mid: "Vault + Spirit Shield 갖추면 안정적. 하이 리스크/하이 리워드.",
                High: "A15+에서 빠른 보스 처치에 탁월. Vault 업그레이드 필수.")
        ),
    };

    public static IReadOnlyList<ArchetypeData> GetAll() => All;

    public static IReadOnlyList<ArchetypeData> GetForCharacter(string characterId) =>
        All.Where(a => a.CharacterId == characterId).ToList();

    public static ArchetypeData? GetById(string id) =>
        All.FirstOrDefault(a => a.Id == id);
}
