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
using SaveOurShip2;

// *Uncomment for Harmony*
// using System.Reflection;
using HarmonyLib;

namespace EvaPatcher
{
    internal static class DefValue
    {
        internal const String EvaTagName = "EVA";
        internal const String StatsDecompressionResistance = "DecompressionResistance";
        internal const float ValueDecompressionResistanceArmor = 0.75f;
        internal const float ValueDecompressionResistanceHelmet = 0.25f;
        // internal const float Va
        internal const String StatsVacuumSpeedMultiplier = "VacuumSpeedMultiplier";
        internal const String StatsHypoxiaResistance = "HypoxiaResistance";

        internal static Dictionary<StatDef, float> ArmorStat = new Dictionary<StatDef, float>
        {
            { ResourceBank.StatDefOf.DecompressionResistanceOffset, 0.75f },
            { ResourceBank.StatDefOf.VacuumSpeedMultiplier, 4f },
        };

        internal static Dictionary<StatDef, float> HelmetStat = new Dictionary<StatDef, float>
        {
            { ResourceBank.StatDefOf.DecompressionResistanceOffset, 0.25f },
            { ResourceBank.StatDefOf.HypoxiaResistanceOffset, 1f },
        };

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
        public List<ThingDef> eva = new List<ThingDef>();
        #endregion

        public override void ExposeData()
        {
            base.ExposeData();
            if (!this.inited)
            {
                this.InitData();
            }
            List<string> list = this.eva?.Select(selector: td => td.defName).ToList() ?? new List<string>();
            // load config
            Scribe_Collections.Look(list: ref list, label: "EvaPatchedList");
            Scribe_Values.Look(value: ref inited, label: "EvaPatchedInited", defaultValue: false);
            this.eva = list.Select(selector: DefDatabase<ThingDef>.GetNamedSilentFail).Where(predicate: td => td != null).ToList();
        }

        public void InitData()
        {
            this.inited = true;
            this.enabled = false;
            this.patchEvaTag = false;
            this.eva = new List<ThingDef>();
        }
    }

    internal class Sos2EvaPatchMod : Mod
    {
        #region variables
        public static Sos2EvaPatchSettings settings;
        public static Sos2EvaPatchMod Instance;
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
            // Instance = this;
        }

        public override string SettingsCategory() => "EvaPatcherGeneralSettings";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect: inRect);
            Text.Font = GameFont.Medium;

            List<ThingDef> allApparel = GetAllArmorAndHelmet();

            Rect topRect = inRect.TopPart(pct: 0.05f);
            Rect leftRect = inRect.BottomPart(pct: 0.45f).LeftPart(pct: 0.5f);
            Rect bottomRect = inRect.BottomPart(pct: 0.9f);
            Rect rightRect = inRect.BottomPart(pct: 0.45f).RightPart(pct: 0.5f);

            #region topRect
            Listing_Standard ls = new Listing_Standard();
            ls.Begin(topRect);
            ls.Label("EvaPatcherGeneralSettings".Translate());
            ls.GapLine(20f);
            // enable eva patcher
            ls.Label("EnableEvaPatcher".Translate());
            ls.CheckboxLabeled("EnableEvaPatcher".Translate(), ref settings.enabled, "EnableEvaPatcher".Translate());
            // set all appearel has eva tag to eva suit
            ls.Label("UseEvatag".Translate());
            ls.CheckboxLabeled("UseEvatag".Translate(), ref settings.enabled, "UseEvatag".Translate());
            ls.GapLine(20f);
            this.searchTerm = Widgets.TextField(rect: topRect.RightPart(pct: 0.95f).LeftPart(pct: 0.95f), text: this.searchTerm);

            ls.End();
            #endregion

            #region leftRect

            GUI.BeginGroup(position: leftRect, style: new GUIStyle(other: GUI.skin.box));
            // get left list item
            List<ThingDef> leftList = allApparel.Where(x => !settings.eva.Contains(x))
            .Where(x => x.label.Contains(this.searchTerm))
            .ToList();

            float leftItemY_Position = 3f;
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
            float rightItemY_Position = 3f;
            Widgets.BeginScrollView(outRect: rightRect.AtZero(), scrollPosition: ref this.rightScrollPosition,
                                    viewRect: new Rect(x: 0f, y: 0f, width: rightRect.width / 10 * 9, height: settings.eva.Count * 32f));
            if (!settings.eva.NullOrEmpty())
            {
                foreach (ThingDef def in settings.eva)
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

            GUI.EndGroup();
            #endregion

            #region buttons
            #region buttons

            if (Widgets.ButtonImage(butRect: bottomRect.BottomPart(pct: 0.6f).TopPart(pct: 0.1f).RightPart(pct: 0.525f).LeftPart(pct: 0.1f), tex: TexUI.ArrowTexRight) &&
                this.leftSelectedItem != null)
            {
                settings.eva.Add(item: this.leftSelectedItem);
                settings.eva = settings.eva.OrderBy(keySelector: td => td.LabelCap.RawText ?? td.defName).ToList();
                this.rightSelectedItem = this.leftSelectedItem;
                this.leftSelectedItem = null;
                // TODO MinifyEverything.RemoveMinifiedFor(def: this.rightSelectedDef);
            }

            if (Widgets.ButtonImage(butRect: bottomRect.BottomPart(pct: 0.4f).TopPart(pct: 0.15f).RightPart(pct: 0.525f).LeftPart(pct: 0.1f), tex: TexUI.ArrowTexLeft) &&
                this.leftSelectedItem != null)
            {
                settings.eva.Remove(item: this.rightSelectedItem);
                this.leftSelectedItem = this.rightSelectedItem;
                this.rightSelectedItem = null;
                // TODO MinifyEverything.AddMinifiedFor(def: this.leftSelectedDef);
            }

            #endregion
            #endregion

            settings.Write();
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

            // *Uncomment for Harmony*
            Harmony harmony = new Harmony("rimworld.nightz.sos2EvaPatcher");
            // patch apparel def 
            harmony.Patch(original: AccessTools.Method(type: typeof(ThingDef), name: nameof(ThingDef.PostLoad)),
                prefix: null, postfix: new HarmonyMethod(typeof(EvaPatcher).GetMethod(nameof(EvaApparelPostfix))));

        }

        static void AddEvaPatchFor(ThingDef def)
        {
            // is armor
            if (def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso))
            {

                DefValue.ArmorStat.ToList().ForEach(x =>
                {
                    if (def.statBases.Any(predicate: y => y.stat.defName == x.Key.defName))
                    {
                        def.statBases.First(predicate: y => y.stat == x.Key).value = x.Value;
                    }
                    else
                    {
                        var sm = new StatModifier
                        {
                            stat = x.Key,
                            value = x.Value
                        };
                        def.statBases.Add(sm);
                    }
                });
            }
            // is helmet
            else if (def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead) || def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.UpperHead))
            {
                DefValue.HelmetStat.ToList().ForEach(x =>
                {
                    if (def.statBases.Any(predicate: y => y.stat.defName == x.Key.defName))
                    {
                        def.statBases.First(predicate: y => y.stat == x.Key).value = x.Value;
                    }
                    else
                    {
                        var sm = new StatModifier
                        {
                            stat = x.Key,
                            value = x.Value
                        };
                        def.statBases.Add(sm);
                    }
                });
            }
            // is other
            else
            {
                // out put rimworld dev error log
                Log.Error("EvaPatcher: " + def.defName + " is not armor or helmet");
            }
        }

        static void RemoveEvaPatchFor(ThingDef def)
        {
            // is armor
            if (def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso))
            {
                DefValue.ArmorStat.ToList().ForEach(x =>
                {
                    if (def.statBases.Any(predicate: y => y.stat.defName == x.Key.defName))
                    {
                        def.statBases.Remove(item: def.statBases.First(predicate: y => y.stat == x.Key));
                    }
                });

            }
            // is helmet
            else if (def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead) || def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.UpperHead))
            {
                DefValue.HelmetStat.ToList().ForEach(x =>
                {
                    if (def.statBases.Any(predicate: y => y.stat.defName == x.Key.defName))
                    {
                        def.statBases.Remove(item: def.statBases.First(predicate: y => y.stat == x.Key));
                    }
                });
            }
            // is other
            else
            {

            }
        }

        static void EvaApparelPostfix(ThingDef th)
        {

            if (Sos2EvaPatchMod.settings.enabled)
            {
                // is apparel and in defName in settings eva
                if (th.IsApparel && Sos2EvaPatchMod.settings.eva.Any(predicate: x => x.defName == th.defName))
                {
                    AddEvaPatchFor(th);
                }
                else if (th.IsApparel && Sos2EvaPatchMod.settings.patchEvaTag && th.apparel.tags.Contains(DefValue.EvaTagName))
                {
                    AddEvaPatchFor(th);
                }
            }



        }
    }


}
