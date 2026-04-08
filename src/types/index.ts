export type CharacterId = 'ironclad' | 'silent' | 'defect' | 'watcher';
export type CardType = 'attack' | 'skill' | 'power' | 'curse' | 'status';
export type CardRarity = 'starter' | 'common' | 'uncommon' | 'rare' | 'curse';
export type AscensionTier = 'beginner' | 'mid' | 'high';

export interface Character {
  id: CharacterId;
  name: string;
  nameKo: string;
  color: string;
  bgClass: string;
  borderClass: string;
  textClass: string;
  description: string;
  startingRelic: string;
  startingRelicDesc: string;
  hp: number;
}

export interface Card {
  id: string;
  name: string;
  type: CardType;
  cost: number | 'X';
  rarity: CardRarity;
  characterId: CharacterId | 'colorless';
  archetypeIds: string[];
  description: string;
  upgraded?: string;
}

export interface ArchetypeCard {
  cardId: string;
  priority: 'core' | 'good' | 'situational';
  note?: string;
}

export interface Archetype {
  id: string;
  name: string;
  nameKo: string;
  characterId: CharacterId;
  description: string;
  playstyle: string;
  winCondition: string;
  coreCards: ArchetypeCard[];
  goodCards: ArchetypeCard[];
  keyRelics: string[];
  avoidRelics: string[];
  difficulty: 'Easy' | 'Medium' | 'Hard';
  ascensionViability: {
    low: number;    // 0-7  viability 1-5
    mid: number;    // 8-14
    high: number;   // 15-20
  };
  ascensionNotes: {
    low: string;
    mid: string;
    high: string;
  };
  tips: string[];
}

export interface RewardRecommendation {
  characterId: CharacterId;
  cardPickAdvice: {
    alwaysTake: string[];
    goodIfNeeded: string[];
    skip: string[];
  };
  relicTiers: {
    s: string[];
    a: string[];
    b: string[];
    skip: string[];
  };
  mapAdvice: {
    elites: string;
    shops: string;
    restSites: string;
    events: string;
  };
  ascensionSpecific: Array<{
    range: [number, number];
    advice: string[];
  }>;
}

export interface DeckCard {
  card: Card;
  upgraded: boolean;
  count: number;
}

export interface ArchetypeMatch {
  archetype: Archetype;
  score: number;
  matchedCoreCards: string[];
  missingCoreCards: string[];
  suggestedPicks: string[];
}
