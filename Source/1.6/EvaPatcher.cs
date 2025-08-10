using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using Verse.Noise;
using Verse.Grammar;
using RimWorld;
using RimWorld.Planet;

// *Uncomment for Harmony*
// using System.Reflection;
// using HarmonyLib;

namespace EvaPatcher
{
    internal static class DefValue
    {
        internal const string EvaTagName = "EVA";
        internal const string StatsDecompressionResistance = "DecompressionResistance";
        internal const float ValueDecompressionResistanceArmor = 0.75f;
        internal const float ValueDecompressionResistanceHelmet = 0.25f;
        // internal const float Va
        internal const string StatsVacuumSpeedMultiplier = "VacuumSpeedMultiplier";
        internal const string StatsHypoxiaResistance = "HypoxiaResistance";

        internal static Dictionary<StatDef, float> ArmorStatFinal = null;
        internal static Dictionary<StatDef, float> HelmetStatFinal = null;
        internal static Dictionary<StatDef, float> EVAStatFinal = null;

        // Base stats
        internal static Dictionary<string, float> ArmorStatBase = new Dictionary<string, float>
        {
            { "Insulation_Cold", 75f }
        };

        internal static Dictionary<string, float> HelmetStatBase = new Dictionary<string, float>
        {
            { "Insulation_Cold", 25f }
        };

        internal static Dictionary<string, float> EVAStatBase = new Dictionary<string, float>
        {
            { "Insulation_Cold", 100f }
        };

        // 奥德赛
        internal static Dictionary<string, float> ArmorStatOdessey = new Dictionary<string, float>
        {
            { "ToxicEnvironmentResistance", 0.25f },
            { "VacuumResistance", 0.32f },
        };

        internal static Dictionary<string, float> HelmetStatOdessey = new Dictionary<string, float>
        {
            { "ToxicEnvironmentResistance", 0.75f },
            { "VacuumResistance", 0.7f },
        };

        internal static Dictionary<string, float> EVAStatOdessey = new Dictionary<string, float>
        {
            { "ToxicEnvironmentResistance", 1f },
            { "VacuumResistance", 1f },
        };

        // Save Our Ship 2
        internal static Dictionary<string, float> ArmorStatSos2 = new Dictionary<string, float>
                {
                    { "DecompressionResistance", 0.75f },
                    { "VacuumSpeedMultiplier", 4f },
                };

        internal static Dictionary<string, float> HelmetStatSos2 = new Dictionary<string, float>
        {
            { "DecompressionResistance", 0.25f },
            { "HypoxiaResistance", 1f },
        };

        internal static Dictionary<string, float> EVAStatSos2 = new Dictionary<string, float>
        {
            { "DecompressionResistance", 1f },
            { "HypoxiaResistance", 1f },
            { "VacuumSpeedMultiplier", 4f },
        };


        public static void InitStats()
        {
            if (ArmorStatFinal == null)
            {
                ArmorStatFinal = new Dictionary<StatDef, float>();
                foreach (var kvp in ArmorStatBase)
                {
                    StatDef target = DefDatabase<StatDef>.GetNamed(kvp.Key);
                    if (target != null)
                    {
                        ArmorStatFinal.Add(target, kvp.Value);
                    }
                    else
                    {
                        Log.Warning($"EvaPatcher: StatDef not found for {kvp.Key}");
                    }
                }
                if (IsOdesseyDlcEnabled())
                {
                    foreach (var kvp in ArmorStatOdessey)
                    {
                        StatDef target = DefDatabase<StatDef>.GetNamed(kvp.Key);
                        if (target != null)
                        {
                            ArmorStatFinal[target] = kvp.Value;
                        }
                        else
                        {
                            Log.Warning($"EvaPatcher: StatDef not found for {kvp.Key}");
                        }
                    }
                }
                if (IsSos2ModEnabled())
                {
                    foreach (var kvp in ArmorStatSos2)
                    {
                        StatDef target = DefDatabase<StatDef>.GetNamed(kvp.Key);
                        if (target != null)
                        {
                            ArmorStatFinal[target] = kvp.Value;
                        }
                        else
                        {
                            Log.Warning($"EvaPatcher: StatDef not found for {kvp.Key}");
                        }
                    }
                }
            }
            if (HelmetStatFinal == null)
            {
                HelmetStatFinal = new Dictionary<StatDef, float>();
                foreach (var kvp in HelmetStatBase)
                {
                    StatDef target = DefDatabase<StatDef>.GetNamed(kvp.Key);
                    if (target != null)
                    {
                        HelmetStatFinal.Add(target, kvp.Value);
                    }
                    else
                    {
                        Log.Warning($"EvaPatcher: StatDef not found for {kvp.Key}");
                    }
                }
                if (IsOdesseyDlcEnabled())
                {
                    foreach (var kvp in HelmetStatOdessey)
                    {
                        StatDef target = DefDatabase<StatDef>.GetNamed(kvp.Key);
                        if (target != null)
                        {
                            HelmetStatFinal[target] = kvp.Value;
                        }
                        else
                        {
                            Log.Warning($"EvaPatcher: StatDef not found for {kvp.Key}");
                        }
                    }
                }
                if (IsSos2ModEnabled())
                {
                    foreach (var kvp in HelmetStatSos2)
                    {
                        StatDef target = DefDatabase<StatDef>.GetNamed(kvp.Key);
                        if (target != null)
                        {
                            HelmetStatFinal[target] = kvp.Value;
                        }
                        else
                        {
                            Log.Warning($"EvaPatcher: StatDef not found for {kvp.Key}");
                        }
                    }
                }
            }
            if (EVAStatFinal == null)
            {
                EVAStatFinal = new Dictionary<StatDef, float>();
                foreach (var kvp in EVAStatBase)
                {
                    StatDef target = DefDatabase<StatDef>.GetNamed(kvp.Key);
                    if (target != null)
                    {
                        EVAStatFinal.Add(target, kvp.Value);
                    }
                    else
                    {
                        Log.Warning($"EvaPatcher: StatDef not found for {kvp.Key}");
                    }
                }
                if (IsOdesseyDlcEnabled())
                {
                    foreach (var kvp in EVAStatOdessey)
                    {
                        StatDef target = DefDatabase<StatDef>.GetNamed(kvp.Key);
                        if (target != null)
                        {
                            EVAStatFinal[target] = kvp.Value;
                        }
                        else
                        {
                            Log.Warning($"EvaPatcher: StatDef not found for {kvp.Key}");
                        }
                    }
                }
                if (IsSos2ModEnabled())
                {
                    foreach (var kvp in EVAStatSos2)
                    {
                        StatDef target = DefDatabase<StatDef>.GetNamed(kvp.Key);
                        if (target != null)
                        {
                            EVAStatFinal[target] = kvp.Value;
                        }
                        else
                        {
                            Log.Warning($"EvaPatcher: StatDef not found for {kvp.Key}");
                        }
                    }
                }
            }

        }

        public static bool IsSos2ModEnabled()
        {
            return ModLister.AllInstalledMods.Any(mod =>
        mod.PackageId.ToString() == "kentington.saveourship2" && mod.Active);
        }

        public static bool IsOdesseyDlcEnabled()
        {
            return ModsConfig.OdysseyActive;
        }
    }

    internal class Sos2EvaPatchSettings : ModSettings
    {

        #region modConfig
        // first inited in game
        public bool inited = false;

        // enable eva patcher
        public bool enabled = false;

        // patch all apparel with eva tag to eva suit
        public bool patchEvaTag = false;
        // all patched eva suit
        public List<string> eva = new List<string>();
        #endregion

        public override void ExposeData()
        {
            base.ExposeData();

            // load config
            Scribe_Collections.Look(list: ref eva, label: "EvaPatchedList");
            Scribe_Values.Look(value: ref inited, label: "EvaPatchedInited", defaultValue: false);
            Scribe_Values.Look(value: ref enabled, label: "EvaPatchedEnabled", defaultValue: false);
            Scribe_Values.Look(value: ref patchEvaTag, label: "EvaPatchedPatchEvaTag", defaultValue: false);

            if (!this.inited)
            {
                this.InitData();
            }
        }

        public void InitData()
        {
            Log.Message("EvaPatcher: InitData");
            this.inited = true;
            this.enabled = false;
            this.patchEvaTag = false;
            this.eva = new List<string>();
        }
    }

    [StaticConstructorOnStartup]
    internal class Sos2EvaPatchMod : Mod
    {
        #region variables
        public static Sos2EvaPatchSettings settings;
        public static Sos2EvaPatchMod Instance { get; private set; }
        #endregion

        #region ui components
        private string searchTerm = "";

        private ThingDef leftSelectedItem;
        private ThingDef rightSelectedItem;

        private Vector2 leftScrollPosition;
        private Vector2 rightScrollPosition;
        #endregion

        public Sos2EvaPatchMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<Sos2EvaPatchSettings>();
            Instance = this;
        }

        public override string SettingsCategory() => "EvaPatcher.Name".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect: inRect);
            Text.Font = GameFont.Medium;

            List<ThingDef> allApparel = GetAllArmorAndHelmet();
            List<ThingDef> evaApparel = allApparel.Where(x => settings.eva.Contains(x.defName)).ToList();

            Rect topRect = inRect.TopPart(0.25f);
            float panelHeight = inRect.height - topRect.height;
            Rect leftRect = new Rect(inRect.x, topRect.yMax, inRect.width * 0.45f, panelHeight);
            Rect rightRect = new Rect(inRect.x + inRect.width * 0.55f, topRect.yMax, inRect.width * 0.45f, panelHeight);
            Rect bottomRect = new Rect(inRect.x, topRect.yMax, inRect.width, panelHeight);

            #region topRect
            Listing_Standard ls = new Listing_Standard();
            ls.Begin(topRect);
            // ls.GapLine();
            // enable eva patcher
            ls.CheckboxLabeled("EvaPatcher.Enable".Translate(), ref settings.enabled, "EvaPatcher.Enable.Desc".Translate());
            // set all appearel has eva tag to eva suit
            ls.CheckboxLabeled("EvaPatcher.Evatag".Translate(), ref settings.patchEvaTag, "EvaPatcher.EvaTag.Desc".Translate());
            ls.GapLine();
            // 搜索框放在topRect底部，使用Listing_Standard分配空间
            Rect searchRect = ls.GetRect(28f);
            this.searchTerm = Widgets.TextField(searchRect, this.searchTerm);
            ls.End();
            #endregion

            #region leftRect

            GUI.BeginGroup(position: leftRect, style: new GUIStyle(other: GUI.skin.box));
            // get left list item
            List<ThingDef> leftList = allApparel.Where(x => !settings.eva.Contains(x.defName))
            .Where(x => x.label.Contains(this.searchTerm))
            .ToList();

            float leftItemY_Position = 3f;
            Widgets.Label(rect: rightRect, label: "EvaPatcher.Available".Translate()); // FIXME not show
            Widgets.BeginScrollView(outRect: leftRect.AtZero(), scrollPosition: ref this.leftScrollPosition,
                                    viewRect: new Rect(x: 0f, y: 0f, width: leftRect.width / 10 * 9, height: leftList.Count * 32f));
            if (!leftList.NullOrEmpty())
            {
                foreach (ThingDef def in leftList)
                {
                    Rect rowRect = new Rect(x: 5, y: leftItemY_Position, width: leftRect.width - 6, height: 30);
                    Widgets.DrawHighlightIfMouseover(rect: rowRect);
                    if (def == this.leftSelectedItem)
                        Widgets.DrawHighlightSelected(rect: rowRect);
                    Widgets.Label(rect: rowRect, label: def.LabelCap.RawText ?? def.defName);
                    if (Widgets.ButtonInvisible(butRect: rowRect))
                        this.leftSelectedItem = def;

                    leftItemY_Position += 32f;
                }
            }

            Widgets.EndScrollView();
            GUI.EndGroup();
            #endregion

            #region rightRect

            GUI.BeginGroup(position: rightRect, style: new GUIStyle(other: GUI.skin.box));
            List<ThingDef> rightList = allApparel.Where(x => settings.eva.Contains(x.defName))
             .Where(x => x.label.Contains(this.searchTerm))
            .ToList();
            float rightItemY_Position = 3f;
            Widgets.Label(rect: rightRect, label: "EvaPatcher.Patched".Translate()); // FIXME not show
            Widgets.BeginScrollView(outRect: rightRect.AtZero(), scrollPosition: ref this.rightScrollPosition,
                                    viewRect: new Rect(x: 0f, y: 0f, width: rightRect.width / 10 * 9, height: rightList.Count * 32f));
            if (!rightList.NullOrEmpty())
            {
                foreach (ThingDef def in rightList)
                {
                    Rect rowRect = new Rect(x: 5, y: rightItemY_Position, width: rightRect.width - 6, height: 30);
                    Widgets.DrawHighlightIfMouseover(rect: rowRect);
                    if (def == this.rightSelectedItem)
                        Widgets.DrawHighlightSelected(rect: rowRect);
                    Widgets.Label(rect: rowRect, label: def.LabelCap.RawText ?? def.defName);
                    if (Widgets.ButtonInvisible(butRect: rowRect))
                        this.rightSelectedItem = def;

                    rightItemY_Position += 32f;
                }
            }
            Widgets.EndScrollView();

            GUI.EndGroup();
            #endregion

            #region buttons

            if (Widgets.ButtonImage(butRect: bottomRect.BottomPart(pct: 0.6f).TopPart(pct: 0.1f).RightPart(pct: 0.525f).LeftPart(pct: 0.1f), tex: TexUI.ArrowTexRight) &&
                this.leftSelectedItem != null)
            {
                rightList.Add(item: this.leftSelectedItem);
                settings.eva = rightList.OrderBy(keySelector: td => td.LabelCap.RawText ?? td.defName).Select(selector: td => td.defName).ToList();
                this.rightSelectedItem = this.leftSelectedItem;
                this.leftSelectedItem = null;
                EvaPatcher.AddEvaPatchFor(def: this.rightSelectedItem);
                // TODO MinifyEverything.RemoveMinifiedFor(def: this.rightSelectedDef);
            }

            if (Widgets.ButtonImage(butRect: bottomRect.BottomPart(pct: 0.4f).TopPart(pct: 0.15f).RightPart(pct: 0.525f).LeftPart(pct: 0.1f), tex: TexUI.ArrowTexLeft) &&
                this.rightSelectedItem != null)
            {
                settings.eva.Remove(item: this.rightSelectedItem.defName);
                this.leftSelectedItem = this.rightSelectedItem;
                this.rightSelectedItem = null;
                EvaPatcher.RemoveEvaPatchFor(def: this.leftSelectedItem);
                // TODO MinifyEverything.AddMinifiedFor(def: this.leftSelectedDef);
            }

            #endregion

            settings.Write();
            base.DoSettingsWindowContents(inRect);
        }

        private static List<ThingDef> GetAllArmorAndHelmet()
        {
            // get all armor or helmet
            return DefDatabase<ThingDef>.AllDefs.Where(predicate: x => x.IsApparel)
            .Where(x =>
            (x.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso) && x.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs)) ||
             x.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead) ||
              x.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.UpperHead))
            .ToList();
        }

    }

    [StaticConstructorOnStartup]
    public static class EvaPatcher
    {
        static EvaPatcher()
        {
            Log.Message("Mod template loaded successfully!");

            // add patch after all thingdef loaded
            LongEventHandler.ExecuteWhenFinished(action: () =>
            {
                if (Sos2EvaPatchMod.settings.enabled)
                {
                    DefDatabase<ThingDef>.AllDefs.Where(predicate: x => x.IsApparel)
                    .ToList().ForEach(action: x => EvaApparelPostfix(x));

                    Log.Message("EvaPatcher: Patched all apparel");
                }
            });

            // *Uncomment for Harmony*
            // Harmony harmony = new Harmony("rimworld.nightz.sos2EvaPatcher");
            // patch apparel def 
            // harmony.Patch(original: AccessTools.Method(type: typeof(ThingDef), name: nameof(ThingDef.PostLoad)),
            //     prefix: null, postfix: new HarmonyMethod(typeof(EvaPatcher).GetMethod(nameof(EvaApparelPostfix))));

        }

        public static void AddEvaPatchFor(ThingDef def)
        {
            DefValue.InitStats();
            try
            {
                if (def.equippedStatOffsets == null)
                {
                    def.equippedStatOffsets = new List<StatModifier>();
                }
                // is armor and helmet
                if (def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso) && def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs) &&
                    (def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead) || def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.UpperHead)))
                {
                    AddStatModifiers(def, DefValue.EVAStatFinal);
                }


                // is armor
                else if (def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso))
                {
                    AddStatModifiers(def, DefValue.ArmorStatFinal);
                }
                // is helmet
                else if (def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead) || def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.UpperHead))
                {
                    AddStatModifiers(def, DefValue.HelmetStatFinal);
                }
                // is other
                else
                {
                    // out put rimworld dev error log
                    Log.Error("EvaPatcher: " + def.defName + " is not armor or helmet");
                }
            }
            catch (Exception e)
            {
                Log.Error("EvaPatcher: " + e.StackTrace);
            }
        }

        public static void RemoveEvaPatchFor(ThingDef def)
        {
            DefValue.InitStats();
            if (def.equippedStatOffsets == null)
            {
                return;
            }
            // remove all eva patch stats
            if (def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso) && def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs) &&
                    (def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead) || def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.UpperHead)))
            {
                DelStatModifiers(def, DefValue.EVAStatFinal);
            }
            // remove armor stats
            else if (def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso))
            {
                DelStatModifiers(def, DefValue.ArmorStatFinal);
            }
            // remove helmet stats
            else if (def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead) || def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.UpperHead))
            {
                DelStatModifiers(def, DefValue.HelmetStatFinal);
            }
        }

        /// <summary>
        /// 依次应用基础、DLC、MOD三组StatModifier，自动判断DLC和MOD是否启用。
        /// </summary>
        public static void AddStatModifiers(ThingDef def, Dictionary<StatDef, float> stats)
        {
            // 基础
            foreach (var x in stats)
            {
                if (def.equippedStatOffsets.Any(y => y.stat.defName == x.Key.defName))
                    def.equippedStatOffsets.First(y => y.stat == x.Key).value = x.Value;
                else
                    def.equippedStatOffsets.Add(new StatModifier { stat = x.Key, value = x.Value });
            }

        }

        public static void DelStatModifiers(ThingDef def, Dictionary<StatDef, float> stats)
        {
            if (def.equippedStatOffsets == null)
            {
                return;
            }
            foreach (var statDef in stats)
            {
                if (def.equippedStatOffsets.Any(predicate: x => x.stat.defName == statDef.Key.defName))
                {
                    def.equippedStatOffsets.Remove(item: def.equippedStatOffsets.First(predicate: x => x.stat == statDef.Key));
                }
            }
        }

        public static void EvaApparelPostfix(ThingDef th)
        {
            if (Sos2EvaPatchMod.settings.enabled && th.IsApparel)
            {
                if (Sos2EvaPatchMod.settings.eva.Any(predicate: x => x == th.defName) ||
                    (Sos2EvaPatchMod.settings.patchEvaTag && th.apparel.tags.Contains(DefValue.EvaTagName)))
                {
                    Log.Message("EvaPatcher: Patching " + th.defName + ":" + th.label + " to EVA suit");
                    AddEvaPatchFor(th);
                }
            }
        }



    }
}
