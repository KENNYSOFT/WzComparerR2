using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WzComparerR2.CharaSim
{
    public static class ItemStringHelper
    {
        /// <summary>
        /// 获取怪物category属性对应的类型说明。
        /// </summary>
        /// <param Name="category">怪物的category属性的值。</param>
        /// <returns></returns>
        public static string GetMobCategoryName(int category)
        {
            switch (category)
            {
                case 0: return "无形态";
                case 1: return "동물형";
                case 2: return "식물형";
                case 3: return "어류형";
                case 4: return "파충류형";
                case 5: return "정령형";
                case 6: return "악마형";
                case 7: return "불사형";
                case 8: return "무형";
                default: return null;
            }
        }

        public static string GetGearPropString(GearPropType propType, int value)
        {
            return GetGearPropString(propType, value, 0);
        }

        /// <summary>
        /// 获取GearPropType所对应的文字说明。
        /// </summary>
        /// <param Name="propType">表示装备属性枚举GearPropType。</param>
        /// <param Name="Value">表示propType属性所对应的值。</param>
        /// <returns></returns>
        public static string GetGearPropString(GearPropType propType, int value, int signFlag)
        {

            string sign;
            switch (signFlag)
            {
                default:
                case 0: //默认处理符号
                    sign = value > 0 ? "+" : null;
                    break;

                case 1: //固定加号
                    sign = "+";
                    break;

                case 2: //无特别符号
                    sign = "";
                    break;
            }
            switch (propType)
            {
                case GearPropType.incSTR: return "STR : " + sign + value;
                case GearPropType.incSTRr: return "STR : " + sign + value + "%";
                case GearPropType.incDEX: return "DEX : " + sign + value;
                case GearPropType.incDEXr: return "DEX : " + sign + value + "%";
                case GearPropType.incINT: return "INT : " + sign + value;
                case GearPropType.incINTr: return "INT : " + sign + value + "%";
                case GearPropType.incLUK: return "LUK : " + sign + value;
                case GearPropType.incLUKr: return "LUK : " + sign + value + "%";
                case GearPropType.incAllStat: return "올스탯 : " + sign + value;
                case GearPropType.statR: return "올스탯 : " + sign + value + "%";
                case GearPropType.incMHP: return "최대 HP : " + sign + value;
                case GearPropType.incMHPr: return "최대 HP : " + sign + value + "%";
                case GearPropType.incMMP: return "최대 MP : " + sign + value;
                case GearPropType.incMMPr: return "최대 MP : " + sign + value + "%";
                case GearPropType.incMDF: return "MaxDF : " + sign + value;
                case GearPropType.incPAD: return "공격력 : " + sign + value;
                case GearPropType.incPADr: return "공격력 : " + sign + value + "%";
                case GearPropType.incMAD: return "마력 : " + sign + value;
                case GearPropType.incMADr: return "마력 : " + sign + value + "%";
                case GearPropType.incPDD: return "방어력 : " + sign + value;
                case GearPropType.incPDDr: return "방어력 : " + sign + value + "%";
                //case GearPropType.incMDD: return "마법방어력 : " + sign + value;
                //case GearPropType.incMDDr: return "마법방어력 : " + sign + value + "%";
                //case GearPropType.incACC: return "명중치 : " + sign + value;
                //case GearPropType.incACCr: return "명중치 : " + sign + value + "%";
                //case GearPropType.incEVA: return "회피치 : " + sign + value;
                //case GearPropType.incEVAr: return "회피치 : " + sign + value + "%";
                case GearPropType.incSpeed: return "이동속도 : " + sign + value;
                case GearPropType.incJump: return "점프력 : " + sign + value;
                case GearPropType.incCraft: return "손재주 : " + sign + value;
                case GearPropType.damR:
                case GearPropType.incDAMr: return "데미지 : " + sign + value + "%";
                case GearPropType.incCr: return "크리티컬 확률 : " + sign + value + "%";
                case GearPropType.incCDr: return "크리티컬 데미지 : " + sign + value + "%";
                case GearPropType.knockback: return "직접 타격시 " + value + "%의 확률로 넉백";
                case GearPropType.incPVPDamage: return "대난투 시 추가 공격력 " + sign + " " + value;
                case GearPropType.incPQEXPr: return "파티퀘스트 경험치 " + value + "% 증가";
                case GearPropType.incEXPr: return "经验值增加" + value + "%";
                case GearPropType.incBDR:
                case GearPropType.bdR: return "보스 몬스터 공격 시 데미지 +" + value + "%";
                case GearPropType.incIMDR:
                case GearPropType.imdR: return "몬스터 방어율 무시 : +" + value + "%";
                //case GearPropType.limitBreak: return "최대 데미지 제한 증가 : +" + value;
                case GearPropType.reduceReq: return "착용 레벨 감소 : - " + value;
                case GearPropType.nbdR: return "일반 몬스터 공격 시 데미지 : +" + value + "%";

                case GearPropType.only: return value == 0 ? null : "고유 아이템";
                case GearPropType.tradeBlock: return value == 0 ? null : "교환 불가";
                case GearPropType.equipTradeBlock: return value == 0 ? null : "장착시 교환 불가";
                case GearPropType.accountSharable: return value == 0 ? null : "월드 내 나의 캐릭터 간 이동만 가능";
                case GearPropType.sharableOnce: return value == 0 ? null : "월드 내 나의 캐릭터 간 1회 이동 가능\n(이동 후 교환불가)";
                case GearPropType.onlyEquip: return value == 0 ? null : "고유장착 아이템";
                case GearPropType.notExtend: return value == 0 ? null : "유효기간 연장 불가";
                case GearPropType.tradeAvailable:
                    switch (value)
                    {
                        case 1: return "#c카르마의 가위 또는 실버 카르마의 가위를 사용하면 1회 교환이 가능하게 할 수 있습니다.#";
                        case 2: return "#c플래티넘 카르마의 가위를 사용하면 1회 교환이 가능하게 할 수 있습니다.#";
                        default: return null;
                    }
                case GearPropType.accountShareTag:
                    switch (value)
                    {
                        case 1: return "#c쉐어 네임 텍을 사용하면 월드 내 나의 캐릭터 간 1회 이동할 수 있습니다.#";
                        default: return null;
                    }
                case GearPropType.noPotential: return value == 0 ? null : "잠재능력 설정 불가";
                case GearPropType.fixedPotential: return value == 0 ? null : "잠재능력 재설정 불가";
                case GearPropType.superiorEqp: return value == 0 ? null : "아이템 강화 성공시 더욱 높은 효과를 받을 수 있습니다.";
                case GearPropType.nActivatedSocket: return value == 0 ? null : "#c可以镶嵌星岩#";
                case GearPropType.jokerToSetItem: return value == 0 ? null : "#c3개 이상 착용하고 있는 모든 세트 아이템에 포함되는 럭키 아이템! (단, 2개 이상의 럭키 아이템 착용 시 1개만 효과 적용.)#";
                case GearPropType.abilityTimeLimited: return value == 0 ? null : "기간 한정 능력치";
                case GearPropType.blockGoldHammer: return value == 0 ? null : "황금망치 사용 불가";
                case GearPropType.colorvar: return value == 0 ? null : "#c该装备可通过染色颜料来变更颜色.#";
                case GearPropType.cantRepair: return value == 0 ? null : "수리 불가";
                case GearPropType.noLookChange: return value == 0 ? null : "훈장 신비의 모루 사용 불가";

                case GearPropType.incAllStat_incMHP25: return "올스탯: " + sign + value + ", 최대 HP : " + sign + (value * 25);
                case GearPropType.incAllStat_incMHP50_incMMP50: return "올스탯: " + sign + value + ", 최대 HP / 최대 MP : " + sign + (value * 50);
                case GearPropType.incMHP_incMMP: return "최대 HP / 최대 MP : " + sign + value;
                case GearPropType.incMHPr_incMMPr: return "최대 HP / 최대 MP : " + sign + value + "%";
                case GearPropType.incPAD_incMAD:
                case GearPropType.incAD: return "공격력 / 마력 : " + sign + value;
                case GearPropType.incPDD_incMDD: return "방어력 : " + sign + value;
                //case GearPropType.incACC_incEVA: return "명중치 / 회피치：" + sign + value;

                case GearPropType.incARC: return "ARC : " + sign + value;
                case GearPropType.incAUT: return "AUT : " + sign + value;

                case GearPropType.Etuc: return "익셉셔널 강화가 가능합니다. (최대 : " + value + "회)";
                default: return null;
            }
        }


        public static string GetGearPropDiffString(GearPropType propType, int value, int standardValue)
        {
            var propStr = GetGearPropString(propType, value);
            if (value > standardValue)
            {
                string subfix = null;
                switch (propType)
                {
                    case GearPropType.incSTR:
                    case GearPropType.incDEX:
                    case GearPropType.incINT:
                    case GearPropType.incLUK:
                    case GearPropType.incMHP:
                    case GearPropType.incMMP:
                    case GearPropType.incMDF:
                    case GearPropType.incARC:
                    case GearPropType.incAUT:
                    case GearPropType.incPAD:
                    case GearPropType.incMAD:
                    case GearPropType.incPDD:
                    case GearPropType.incMDD:
                    case GearPropType.incSpeed:
                    case GearPropType.incJump:
                        subfix = $"({standardValue} #$+{value - standardValue}#)"; break;

                    case GearPropType.bdR:
                    case GearPropType.incBDR:
                    case GearPropType.imdR:
                    case GearPropType.incIMDR:
                    case GearPropType.damR:
                    case GearPropType.incDAMr:
                    case GearPropType.statR:
                        subfix = $"({standardValue}% #$+{value - standardValue}%#)"; break;
                }
                propStr = "#$" + propStr + "# " + subfix;
            }
            return propStr;
        }

        /// <summary>
        /// 获取gearGrade所对应的字符串。
        /// </summary>
        /// <param Name="rank">表示装备的潜能等级GearGrade。</param>
        /// <returns></returns>
        public static string GetGearGradeString(GearGrade rank)
        {
            switch (rank)
            {
                //case GearGrade.C: return "C级(一般物品)";
                case GearGrade.B: return "(레어 아이템)";
                case GearGrade.A: return "(에픽 아이템)";
                case GearGrade.S: return "(유니크 아이템)";
                case GearGrade.SS: return "(레전드리 아이템)";
                case GearGrade.Special: return "(스페셜 아이템)";
                default: return null;
            }
        }

        /// <summary>
        /// 获取gearType所对应的字符串。
        /// </summary>
        /// <param Name="Type">表示装备类型GearType。</param>
        /// <returns></returns>
        public static string GetGearTypeString(GearType type)
        {
            switch (type)
            {
                //case GearType.body: return "纸娃娃(身体)";
                case GearType.head: return "피부";
                case GearType.face:
                case GearType.face2: return "성형";
                case GearType.hair:
                case GearType.hair2:
                case GearType.hair3: return "헤어";
                case GearType.faceAccessory: return "얼굴장식";
                case GearType.eyeAccessory: return "눈장식";
                case GearType.earrings: return "귀고리";
                case GearType.pendant: return "펜던트";
                case GearType.belt: return "벨트";
                case GearType.medal: return "훈장";
                case GearType.shoulderPad: return "어깨장식";
                case GearType.cap: return "모자";
                case GearType.cape: return "망토";
                case GearType.coat: return "상의";
                case GearType.dragonMask: return "드래곤 모자";
                case GearType.dragonPendant: return "드래곤 펜던트";
                case GearType.dragonWings: return "드래곤 날개장식";
                case GearType.dragonTail: return "드래곤 꼬리장식";
                case GearType.glove: return "장갑";
                case GearType.longcoat: return "한벌옷";
                case GearType.machineEngine: return "메카닉 엔진";
                case GearType.machineArms: return "메카닉 암";
                case GearType.machineLegs: return "메카닉 레그";
                case GearType.machineBody: return "메카닉 프레임";
                case GearType.machineTransistors: return "메카닉 트랜지스터";
                case GearType.pants: return "하의";
                case GearType.ring: return "반지";
                case GearType.shield: return "방패";
                case GearType.shoes: return "신발";
                case GearType.shiningRod: return "샤이닝 로드";
                case GearType.soulShooter: return "소울 슈터";
                case GearType.ohSword: return "한손검";
                case GearType.ohAxe: return "한손도끼";
                case GearType.ohBlunt: return "한손둔기";
                case GearType.dagger: return "단검";
                case GearType.katara: return "블레이드";
                case GearType.magicArrow: return "마법화살";
                case GearType.card: return "카드";
                case GearType.box: return "宝盒";
                case GearType.orb: return "오브";
                case GearType.novaMarrow: return "용의 정수";
                case GearType.soulBangle: return "소울링";
                case GearType.mailin: return "매그넘";
                case GearType.cane: return "케인";
                case GearType.wand: return "완드";
                case GearType.staff: return "스태프";
                case GearType.thSword: return "두손검";
                case GearType.thAxe: return "두손도끼";
                case GearType.thBlunt: return "두손둔기";
                case GearType.spear: return "창";
                case GearType.polearm: return "폴암";
                case GearType.bow: return "활";
                case GearType.crossbow: return "석궁";
                case GearType.throwingGlove: return "아대";
                case GearType.knuckle: return "너클";
                case GearType.gun: return "건";
                case GearType.android: return "안드로이드";
                case GearType.machineHeart: return "기계 심장";
                case GearType.pickaxe: return "채광 도구";
                case GearType.shovel: return "약초채집 도구";
                case GearType.pocket: return "포켓 아이템";
                case GearType.dualBow: return "듀얼 보우건";
                case GearType.handCannon: return "핸드캐논";
                case GearType.badge: return "뱃지";
                case GearType.emblem: return "엠블렘";
                case GearType.soulShield: return "소울실드";
                case GearType.demonShield: return "포스실드";
                case GearType.totem: return "图腾";
                case GearType.petEquip: return "펫장비";
                case GearType.taming:
                case GearType.taming2:
                case GearType.taming3: 
                case GearType.tamingChair: return "라이딩";
                case GearType.saddle: return "안장";
                case GearType.katana: return "武士刀";
                case GearType.fan: return "折扇";
                case GearType.swordZB: return "대검";
                case GearType.swordZL: return "태도";
                case GearType.weapon: return "무기";
                case GearType.subWeapon: return "보조무기";
                case GearType.heroMedal: return "메달";
                case GearType.rosario: return "로자리오";
                case GearType.chain: return "쇠사슬";
                case GearType.book1:
                case GearType.book2:
                case GearType.book3: return "마도서";
                case GearType.bowMasterFeather: return "화살깃";
                case GearType.crossBowThimble: return "활골무";
                case GearType.shadowerSheath: return "단검용 검집";
                case GearType.nightLordPoutch: return "부적";
                case GearType.viperWristband: return "리스트밴드";
                case GearType.captainSight: return "조준기";
                case GearType.connonGunPowder: 
                case GearType.connonGunPowder2: return "화약통";
                case GearType.aranPendulum: return "무게추";
                case GearType.evanPaper: return "문서";
                case GearType.battlemageBall: return "마법구슬";
                case GearType.wildHunterArrowHead: return "화살촉";
                case GearType.cygnusGem: return "보석";
                case GearType.controller: return "컨트롤러";
                case GearType.foxPearl: return "여우 구슬";
                case GearType.chess: return "체스피스";
                case GearType.powerSource: return "파워소스";

                case GearType.energySword: return "에너지소드";
                case GearType.desperado: return "데스페라도";
                case GearType.magicStick: return "驯兽魔法棒";
                case GearType.whistle:
                case GearType.whistle2: return "哨子";
                case GearType.boxingClaw: return "拳爪";
                case GearType.kodachi:
                case GearType.kodachi2:  return "小太刀";
                case GearType.espLimiter: return "ESP 리미터";

                case GearType.GauntletBuster: return "건틀렛 리볼버";
                case GearType.ExplosivePill: return "장약";

                case GearType.chain2: return "체인";
                case GearType.magicGauntlet: return "매직 건틀렛";
                case GearType.transmitter: return "무기 전송장치";
                case GearType.magicWing: return "매직윙";
                case GearType.pathOfAbyss: return "패스 오브 어비스";

                case GearType.relic: return "렐릭";
                case GearType.ancientBow: return "에인션트 보우";

                case GearType.handFan: return "부채";
                case GearType.fanTassel: return "선추";

                case GearType.tuner: return "튜너";
                case GearType.bracelet: return "브레이슬릿";

                case GearType.boxingCannon: return "拳封";
                case GearType.boxingSky: return "拳天";

                case GearType.breathShooter: return "브레스 슈터";
                case GearType.weaponBelt: return "웨폰 벨트";

                case GearType.ornament: return "노리개";

                case GearType.chakram: return "차크람";
                case GearType.hexSeeker: return "헥스시커";

                default: return null;
            }
        }

        /// <summary>
        /// 获取武器攻击速度所对应的字符串。
        /// </summary>
        /// <param Name="attackSpeed">表示武器的攻击速度，通常为2~9的数字。</param>
        /// <returns></returns>
        public static string GetAttackSpeedString(int attackSpeed)
        {
            switch (attackSpeed)
            {
                case 2:
                case 3: return "매우빠름";
                case 4:
                case 5: return "빠름";
                case 6: return "보통";
                case 7:
                case 8: return "느림";
                case 9: return "매우느림";
                default:
                    return attackSpeed.ToString();
            }
        }

        /// <summary>
        /// 获取套装装备类型的字符串。
        /// </summary>
        /// <param Name="Type">表示套装装备类型的GearType。</param>
        /// <returns></returns>
        public static string GetSetItemGearTypeString(GearType type)
        {
            return GetGearTypeString(type);
        }

        /// <summary>
        /// 获取装备额外职业要求说明的字符串。
        /// </summary>
        /// <param Name="Type">表示装备类型的GearType。</param>
        /// <returns></returns>
        public static string GetExtraJobReqString(GearType type)
        {
            switch (type)
            {
                //0xxx
                case GearType.heroMedal: return "히어로 직업군 착용 가능";
                case GearType.rosario: return "팔라딘 직업군 착용 가능";
                case GearType.chain: return "다크나이트 직업군 착용 가능";
                case GearType.book1: return "불,독 계열 마법사 착용 가능";
                case GearType.book2: return "얼음,번개 계열 마법사 착용 가능";
                case GearType.book3: return "비숍 계열 마법사 착용 가능";
                case GearType.bowMasterFeather: return "보우마스터 직업군 착용 가능";
                case GearType.crossBowThimble: return "신궁 직업군 착용 가능";
                case GearType.shadowerSheath: return "섀도어 직업군 착용 가능";
                case GearType.nightLordPoutch: return "나이트로드 직업군 착용 가능";
                case GearType.katara: return "듀얼블레이드 직업군 착용 가능";
                case GearType.viperWristband: return "바이퍼 직업군 착용 가능";
                case GearType.captainSight: return "캡틴 직업군 착용 가능";
                case GearType.connonGunPowder: 
                case GearType.connonGunPowder2: return "캐논 슈터 직업군 착용 가능";
                case GearType.box:
                case GearType.boxingClaw: return "龙的传人可穿戴装备";
                case GearType.relic: return "패스파인더 직업군 착용 가능";

                //1xxx
                case GearType.cygnusGem: return "시그너스 기사단 착용 가능";

                //2xxx
                case GearType.aranPendulum: return GetExtraJobReqString(21);
                case GearType.dragonMask:
                case GearType.dragonPendant:
                case GearType.dragonWings:
                case GearType.dragonTail:
                case GearType.evanPaper: return GetExtraJobReqString(22);
                case GearType.magicArrow: return GetExtraJobReqString(23);
                case GearType.card: return GetExtraJobReqString(24);
                case GearType.foxPearl: return GetExtraJobReqString(25);
                case GearType.orb:
                case GearType.shiningRod: return GetExtraJobReqString(27);

                //3xxx
                case GearType.demonShield: return GetExtraJobReqString(31);
                case GearType.desperado: return "데몬 어벤져 착용 가능";
                case GearType.battlemageBall: return "배틀메이지 착용 가능";
                case GearType.wildHunterArrowHead: return "와일드헌터 착용 가능";
                case GearType.machineEngine:
                case GearType.machineArms:
                case GearType.machineLegs:
                case GearType.machineBody:
                case GearType.machineTransistors:
                case GearType.mailin: return "메카닉 착용 가능";
                case GearType.controller:
                case GearType.powerSource:
                case GearType.energySword: return GetExtraJobReqString(36);
                case GearType.GauntletBuster:
                case GearType.ExplosivePill: return GetExtraJobReqString(37);

                //4xxx
                case GearType.katana:
                case GearType.kodachi:
                case GearType.kodachi2: return "剑豪可穿戴装备";
                case GearType.fan: return "阴阳师可穿戴装备";

                //5xxx
                case GearType.soulShield: return "미하일 착용 가능";

                //6xxx
                case GearType.novaMarrow: return GetExtraJobReqString(61);
                case GearType.weaponBelt:
                case GearType.breathShooter: return GetExtraJobReqString(63);
                case GearType.chain2:
                case GearType.transmitter: return GetExtraJobReqString(64);
                case GearType.soulBangle:
                case GearType.soulShooter: return GetExtraJobReqString(65);

                //10xxx
                case GearType.swordZB:
                case GearType.swordZL: return GetExtraJobReqString(101);

                case GearType.whistle:
                case GearType.whistle2:
                case GearType.magicStick: return GetExtraJobReqString(112);

                case GearType.espLimiter:
                case GearType.chess: return GetExtraJobReqString(142);

                case GearType.magicGauntlet: 
                case GearType.magicWing: return GetExtraJobReqString(152);

                case GearType.pathOfAbyss: return GetExtraJobReqString(155);
                case GearType.handFan:
                case GearType.fanTassel: return GetExtraJobReqString(164);

                case GearType.tuner:
                case GearType.bracelet: return GetExtraJobReqString(151);

                case GearType.boxingCannon:
                case GearType.boxingSky: return GetExtraJobReqString(175);

                case GearType.ornament: return GetExtraJobReqString(162);

                case GearType.chakram:
                case GearType.hexSeeker: return GetExtraJobReqString(154);
                default: return null;
            }
        }

        /// <summary>
        /// 获取装备额外职业要求说明的字符串。
        /// </summary>
        /// <param Name="specJob">表示装备属性的reqSpecJob的值。</param>
        /// <returns></returns>
        public static string GetExtraJobReqString(int specJob)
        {
            switch (specJob)
            {
                case 21: return "아란 착용 가능";
                case 22: return "에반 착용 가능";
                case 23: return "메르세데스 착용가능";
                case 24: return "팬텀 착용 가능";
                case 25: return "은월 착용 가능";
                case 27: return "루미너스 착용 가능";
                case 31: return "데몬 직업군 착용 가능";
                case 36: return "제논 착용 가능";
                case 37: return "블래스터 착용 가능";
                case 41: return "剑豪可穿戴装备";
                case 42: return "阴阳师可穿戴装备";
                case 51: return "미하일 착용 가능";
                case 61: return "카이저 착용 가능";
                case 63: return "카인 착용 가능";
                case 64: return "카데나 착용 가능";
                case 65: return "엔젤릭 버스터 착용 가능";
                case 101: return "제로 착용 가능";
                case 112: return "林之灵可穿戴装备";
                case 142: return "키네시스 착용 가능";
                case 151: return "아델 착용 가능";
                case 152: return "일리움 착용 가능";
                case 154: return "칼리 착용 가능";
                case 155: return "아크 착용 가능";
                case 162: return "라라 착용 가능";
                case 164: return "호영 착용 가능";
                case 175: return "墨玄可穿戴装备";
                default: return null;
            }
        }

        public static string GetItemPropString(ItemPropType propType, int value)
        {
            switch (propType)
            {
                case ItemPropType.tradeBlock:
                    return GetGearPropString(GearPropType.tradeBlock, value);
                case ItemPropType.useTradeBlock:
                    return value == 0 ? null : "사용시 교환 불가";
                case ItemPropType.tradeAvailable:
                    return GetGearPropString(GearPropType.tradeAvailable, value);
                case ItemPropType.only:
                    return GetGearPropString(GearPropType.only, value);
                case ItemPropType.accountSharable:
                    return GetGearPropString(GearPropType.accountSharable, value);
                case ItemPropType.sharableOnce:
                    return GetGearPropString(GearPropType.sharableOnce, value);
                case ItemPropType.exchangeableOnce:
                    return value == 0 ? null : "1회 교환가능 (사용 또는 거래 후 교환불가)";
                case ItemPropType.quest:
                    return value == 0 ? null : "퀘스트 아이템";
                case ItemPropType.pquest:
                    return value == 0 ? null : "파티 퀘스트 아이템";
                case ItemPropType.multiPet:
                    return value == 0 ? "일반펫 (다른 일반펫과 중복 사용불가)" : "멀티펫 (다른 펫과 최대 3개 중복 사용가능)";
                case ItemPropType.permanent:
                    return value == 0 ? null : "마법의 시간이 끝나지 않는 미라클 펫입니다.";
                default:
                    return null;
            }
        }

        public static string GetItemCoreSpecString(ItemCoreSpecType coreSpecType, int value, string desc)
        {
            bool hasCoda = false;
            if (desc?.Length > 0)
            {
                char lastCharacter = desc.Last();
                hasCoda = lastCharacter >= '가' && lastCharacter <= '힣' && (lastCharacter - '가') % 28 != 0;
            }
            switch (coreSpecType)
            {
                case ItemCoreSpecType.Ctrl_mobLv:
                    return value == 0 ? null : "몬스터 레벨 " + value + " 증가";
                case ItemCoreSpecType.Ctrl_mobHPRate:
                    return value == 0 ? null : "몬스터 HP " + value + "% 증가";
                case ItemCoreSpecType.Ctrl_mobRate:
                    return value == 0 ? null : "몬스터 개체 수 " + value + "% 증가";
                case ItemCoreSpecType.Ctrl_mobRateSpecial:
                    return value == 0 ? null : "몬스터 개체 수 " + value + "% 추가 증가";
                case ItemCoreSpecType.Ctrl_change_Mob:
                    return desc == null ? null : desc + (hasCoda ? "으" : "") + "로 몬스터 이미지 변경";
                case ItemCoreSpecType.Ctrl_change_BGM:
                    return desc == null ? null : desc + (hasCoda ? "으" : "") + "로 배경 음악 변경";
                case ItemCoreSpecType.Ctrl_change_BackGrnd:
                    return desc == null ? null : desc + (hasCoda ? "으" : "") + "로 배경 이미지 변경";
                case ItemCoreSpecType.Ctrl_partyExp:
                    return value == 0 ? null : "파티 경험치 " + value + "% 증가";
                case ItemCoreSpecType.Ctrl_partyExpSpecial:
                    return value == 0 ? null : "파티 경험치 " + value + "% 추가 증가";
                case ItemCoreSpecType.Ctrl_addMob:
                    return value == 0 || desc == null ? null : desc + ", 링크" + value + " 지역에 추가";
                case ItemCoreSpecType.Ctrl_dropRate:
                    return value == 0 ? null : "드롭률 " + value + "% 증가";
                case ItemCoreSpecType.Ctrl_dropRateSpecial:
                    return value == 0 ? null : "드롭률 " + value + "% 추가 증가";
                case ItemCoreSpecType.Ctrl_dropRate_Herb:
                    return value == 0 ? null : "약초 드롭률 " + value + "% 증가";
                case ItemCoreSpecType.Ctrl_dropRate_Mineral:
                    return value == 0 ? null : "광물 드롭률 " + value + "% 증가";
                case ItemCoreSpecType.Ctrl_dropRareEquip:
                    return value == 0 ? null : "장비 아이템이 미확인 상태로 드롭";
                case ItemCoreSpecType.Ctrl_reward:
                case ItemCoreSpecType.Ctrl_addMission:
                    return desc;
                default:
                    return null;
            }
        }

        public static string GetSkillReqAmount(int skillID, int reqAmount)
        {
            switch (skillID / 10000)
            {
                case 11200: return "[需要巨熊技能点: " + reqAmount + "]";
                case 11210: return "[需要雪豹技能点: " + reqAmount + "]";
                case 11211: return "[需要猛禽技能点: " + reqAmount + "]";
                case 11212: return "[需要猫咪技能点: " + reqAmount + "]";
                default: return "[需要？？技能点: " + reqAmount + "]";
            }
        }

        public static string GetJobName(int jobCode)
        {
            switch (jobCode)
            {
                case 0: return "초보자";
                case 100: return "검사";
                case 110: return "파이터";
                case 111: return "크루세이더";
                case 112: return "히어로";
                case 114: return "히어로(6차)";
                case 120: return "페이지";
                case 121: return "나이트";
                case 122: return "팔라딘";
                case 124: return "팔라딘(6차)";
                case 130: return "스피어맨";
                case 131: return "드래곤나이트";
                case 132: return "다크나이트";
                case 134: return "다크나이트(6차)";
                case 200: return "매지션";
                case 210: return "위자드(불,독)";
                case 211: return "메이지(불,독)";
                case 212: return "아크메이지(불,독)";
                case 214: return "아크메이지(불,독)(6차)";
                case 220: return "위자드(썬,콜)";
                case 221: return "메이지(썬,콜)";
                case 222: return "아크메이지(썬,콜)";
                case 224: return "아크메이지(썬,콜)(6차)";
                case 230: return "클레릭";
                case 231: return "프리스트";
                case 232: return "비숍";
                case 234: return "비숍(6차)";
                case 300: return "아처";
                case 301: return "아처";
                case 310: return "헌터";
                case 311: return "레인저";
                case 312: return "보우마스터";
                case 314: return "보우마스터(6차)";
                case 320: return "사수";
                case 321: return "저격수";
                case 322: return "신궁";
                case 324: return "신궁(6차)";
                case 330: return "에인션트 아처";
                case 331: return "체이서";
                case 332: return "패스파인더";
                case 334: return "패스파인더(6차)";
                case 400: return "로그";
                case 410: return "어쌔신";
                case 411: return "허밋";
                case 412: return "나이트로드";
                case 414: return "나이트로드(6차)";
                case 420: return "시프";
                case 421: return "시프마스터";
                case 422: return "섀도어";
                case 424: return "섀도어(6차)";
                case 430: return "세미듀어러";
                case 431: return "듀어러";
                case 432: return "듀얼마스터";
                case 433: return "슬래셔";
                case 434: return "듀얼블레이더";
                case 436: return "듀얼블레이더(6차)";
                case 500: return "해적";
                case 501: return "해적";
                case 510: return "인파이터";
                case 511: return "버커니어";
                case 512: return "바이퍼";
                case 514: return "바이퍼(6차)";
                case 520: return "건슬링거";
                case 521: return "발키리";
                case 522: return "캡틴";
                case 524: return "캡틴(6차)";
                case 530: return "캐논슈터";
                case 531: return "캐논블래스터";
                case 532: return "캐논마스터";
                case 534: return "캐논마스터(6차)";


                case 800:
                case 900: return "운영자";


                case 1000: return "노블레스";
                case 1100: return "소울마스터(1차)";
                case 1110: return "소울마스터(2차)";
                case 1111: return "소울마스터(3차)";
                case 1112: return "소울마스터(4차)";
                case 1114: return "소울마스터(6차)";
                case 1200: return "플레임위자드(1차)";
                case 1210: return "플레임위자드(2차)";
                case 1211: return "플레임위자드(3차)";
                case 1212: return "플레임위자드(4차)";
                case 1214: return "플레임위자드(6차)";
                case 1300: return "윈드브레이커(1차)";
                case 1310: return "윈드브레이커(2차)";
                case 1311: return "윈드브레이커(3차)";
                case 1312: return "윈드브레이커(4차)";
                case 1314: return "윈드브레이커(6차)";
                case 1400: return "나이트워커(1차)";
                case 1410: return "나이트워커(2차)";
                case 1411: return "나이트워커(3차)";
                case 1412: return "나이트워커(4차)";
                case 1414: return "나이트워커(6차)";
                case 1500: return "스트라이커(1차)";
                case 1510: return "스트라이커(2차)";
                case 1511: return "스트라이커(3차)";
                case 1512: return "스트라이커(4차)";
                case 1514: return "스트라이커(6차)";

                case 2000: return "레전드";
                case 2001: return "에반";
                case 2002: return "메르세데스";
                case 2003: return "팬텀";
                case 2004: return "루미너스";
                case 2005: return "은월";
                case 2100: return "아란(1차)";
                case 2110: return "아란(2차)";
                case 2111: return "아란(3차)";
                case 2112: return "아란(4차)";
                case 2114: return "아란(6차)";
                case 2200:
                case 2210: return "에반(1차)";
                case 2211:
                case 2212:
                case 2213: return "에반(2차)";
                case 2214:
                case 2215:
                case 2216: return "에반(3차)";
                case 2217:
                case 2218: return "에반(4차)";
                case 2220: return "에반(6차)";
                case 2300: return "메르세데스(1차)";
                case 2310: return "메르세데스(2차)";
                case 2311: return "메르세데스(3차)";
                case 2312: return "메르세데스(4차)";
                case 2314: return "메르세데스(6차)";
                case 2400: return "팬텀(1차)";
                case 2410: return "팬텀(2차)";
                case 2411: return "팬텀(3차)";
                case 2412: return "팬텀(4차)";
                case 2414: return "팬텀(6차)";
                case 2500: return "은월(1차)";
                case 2510: return "은월(2차)";
                case 2511: return "은월(3차)";
                case 2512: return "은월(4차)";
                case 2514: return "은월(6차)";
                case 2700: return "루미너스(1차)";
                case 2710: return "루미너스(2차)";
                case 2711: return "루미너스(3차)";
                case 2712: return "루미너스(4차)";
                case 2714: return "루미너스(6차)";


                case 3000: return "시티즌";
                case 3001: return "데몬";
                case 3100: return "데몬슬레이어(1차)";
                case 3110: return "데몬슬레이어(2차)";
                case 3111: return "데몬슬레이어(3차)";
                case 3112: return "데몬슬레이어(4차)";
                case 3114: return "데몬슬레이어(6차)";
                case 3101: return "데몬어벤져(1차)";
                case 3120: return "데몬어벤져(2차)";
                case 3121: return "데몬어벤져(3차)";
                case 3122: return "데몬어벤져(4차)";
                case 3124: return "데몬어벤져(6차)";
                case 3200: return "배틀메이지(1차)";
                case 3210: return "배틀메이지(2차)";
                case 3211: return "배틀메이지(3차)";
                case 3212: return "배틀메이지(4차)";
                case 3214: return "배틀메이지(6차)";
                case 3300: return "와일드헌터(1차)";
                case 3310: return "와일드헌터(2차)";
                case 3311: return "와일드헌터(3차)";
                case 3312: return "와일드헌터(4차)";
                case 3314: return "와일드헌터(6차)";
                case 3500: return "메카닉(1차)";
                case 3510: return "메카닉(2차)";
                case 3511: return "메카닉(3차)";
                case 3512: return "메카닉(4차)";
                case 3514: return "메카닉(6차)";
                case 3002: return "제논";
                case 3600: return "제논(1차)";
                case 3610: return "제논(2차)";
                case 3611: return "제논(3차)";
                case 3612: return "제논(4차)";
                case 3614: return "제논(6차)";
                case 3700: return "블래스터";
                case 3710: return "블래스터(2차)";
                case 3711: return "블래스터(3차)";
                case 3712: return "블래스터(4차)";
                case 3714: return "블래스터(6차)";

                case 4001: return "剑豪";
                case 4002: return "阴阳师";
                case 4100: return "剑豪(1次)";
                case 4110: return "剑豪(2次)";
                case 4111: return "剑豪(3次)";
                case 4112: return "剑豪(4次)";
                case 4114: return "剑豪(6次)";
                case 4200: return "阴阳师(1次)";
                case 4210: return "阴阳师(2次)";
                case 4211: return "阴阳师(3次)";
                case 4212: return "阴阳师(4次)";
                case 4214: return "阴阳师(6次)";


                case 5000: return "미하일";
                case 5100: return "미하일(1차)";
                case 5110: return "미하일(2차)";
                case 5111: return "미하일(3차)";
                case 5112: return "미하일(4차)";
                case 5114: return "미하일(6차)";


                case 6000: return "카이저";
                case 6001: return "엔젤릭버스터";
                case 6002: return "카데나";
                case 6003: return "카인";
                case 6100: return "카이저(1차)";
                case 6110: return "카이저(2차)";
                case 6111: return "카이저(3차)";
                case 6112: return "카이저(4차)";
                case 6114: return "카이저(6차)";
                case 6300: return "카인(1차)";
                case 6310: return "카인(2차)";
                case 6311: return "카인(3차)";
                case 6312: return "카인(4차)";
                case 6314: return "카인(6차)";
                case 6400: return "카데나(1차)";
                case 6410: return "카데나(2차)";
                case 6411: return "카데나(3차)";
                case 6412: return "카데나(4차)";
                case 6414: return "카데나(6차)";
                case 6500: return "엔젤릭버스터(1차)";
                case 6510: return "엔젤릭버스터(2차)";
                case 6511: return "엔젤릭버스터(3차)";
                case 6512: return "엔젤릭버스터(4차)";
                case 6514: return "엔젤릭버스터(6차)";


                case 7000: return "어빌리티";
                case 7100: return "유니온";
                case 7200: return "몬스터라이프";


                case 9100: return "길드";
                case 9200:
                case 9201:
                case 9202:
                case 9203:
                case 9204: return "전문기술";


                case 10000: return "제로";
                case 10100: return "제로(1차)";
                case 10110: return "제로(2차)";
                case 10111: return "제로(3차)";
                case 10112: return "제로(4차)";
                case 10114: return "제로(6차)";

                case 11000: return "林之灵";
                case 11200: return "林之灵(1次)";
                case 11210: return "林之灵(2次)";
                case 11211: return "林之灵(3次)";
                case 11212: return "林之灵(4次)";

                case 13000: return "핑크빈";
                case 13001: return "예티";
                case 13100: return "핑크빈";
                case 13500: return "예티";

                case 14000: return "키네시스";
                case 14200: return "키네시스(1차)";
                case 14210: return "키네시스(2차)";
                case 14211: return "키네시스(3차)";
                case 14212: return "키네시스(4차)";
                case 14214: return "키네시스(6차)";

                case 15000: return "일리움";
                case 15001: return "아크";
                case 15002: return "아델";
                case 15003: return "칼리";
                case 15100: return "아델(1차)";
                case 15110: return "아델(2차)";
                case 15111: return "아델(3차)";
                case 15112: return "아델(4차)";
                case 15114: return "아델(6차)";
                case 15200: return "일리움(1차)";
                case 15210: return "일리움(2차)";
                case 15211: return "일리움(3차)";
                case 15212: return "일리움(4차)";
                case 15214: return "일리움(6차)";
                case 15400: return "칼리(1차)";
                case 15410: return "칼리(2차)";
                case 15411: return "칼리(3차)";
                case 15412: return "칼리(4차)";
                case 15414: return "칼리(6차)";
                case 15500: return "아크(1차)";
                case 15510: return "아크(2차)";
                case 15511: return "아크(3차)";
                case 15512: return "아크(4차)";
                case 15514: return "아크(6차)";

                case 16000: return "호영";
                case 16001: return "라라";
                case 16200: return "라라(1차)";
                case 16210: return "라라(2차)";
                case 16211: return "라라(3차)";
                case 16212: return "라라(4차)";
                case 16214: return "라라(6차)";
                case 16400: return "호영(1차)";
                case 16410: return "호영(2차)";
                case 16411: return "호영(3차)";
                case 16412: return "호영(4차)";
                case 16414: return "호영(6차)";

                case 17000: return "墨玄";
                case 17500: return "墨玄(1차)";
                case 17510: return "墨玄(2차)";
                case 17511: return "墨玄(3차)";
                case 17512: return "墨玄(4차)";
                case 17514: return "墨玄(6차)";

                    
                case 40000: return "5차";
                case 40001: return "5차(전사)";
                case 40002: return "5차(마법사)";
                case 40003: return "5차(궁수)";
                case 40004: return "5차(도적)";
                case 40005: return "5차(해적)";


                case 50000: return "6차";
                case 50006: return "6차(강화코어)";
                case 50007: return "6차(헥사스탯)";
            }
            return null;
        }
    }
}
