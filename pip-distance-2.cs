using System;
using System.Collections.Generic;
using cAlgo.API;
using cAlgo.API.Internals;

namespace cAlgo.Indicators
{
    public enum RenkoProfile
    {
        Profile_7ATR,   // Renko 4, Lot 0.3, SL 30
        Profile_14ATR,  // Renko 8, Lot 0.15, SL 60
        Profile_28ATR,  // Renko 15, Lot 0.07, SL 130
        Profile_56ATR   // Renko 30, Lot 0.03, SL 300
    }

    public enum ReverseMode
    {
        Short, // SL di atas Base → Entry di bawah SL (profit ke bawah)
        Long   // SL di bawah Base → Entry di atas SL (profit ke atas)
    }

    [Indicator(AccessRights = AccessRights.None, AutoRescale = false, IsOverlay = true)]
    public class DualDistanceLinesDashboard : Indicator
    {
        [Parameter("Renko Profile", DefaultValue = RenkoProfile.Profile_7ATR, Group = "Profile")]
        public RenkoProfile SelectedProfile { get; set; }

        [Parameter("Mode", DefaultValue = ReverseMode.Short, Group = "Profile")]
        public ReverseMode Mode { get; set; }

        [Parameter("Require Ctrl to Delete", DefaultValue = true, Group = "Lines")]
        public bool RequireCtrlToDelete { get; set; }

        // ────────────────────────────────────────────── Visual Customization
        [Parameter("Base Line Style", DefaultValue = LineStyle.Dots, Group = "Visual")]
        public LineStyle BaseLineStyle { get; set; }

        [Parameter("Line Thickness", DefaultValue = 2, MinValue = 1, MaxValue = 5, Group = "Visual")]
        public int LineThickness { get; set; }

        [Parameter("Label Font Size", DefaultValue = 10, MinValue = 8, MaxValue = 14, Group = "Visual")]
        public int LabelFontSize { get; set; }

        [Parameter("Label Offset from Right (bars)", DefaultValue = 12, MinValue = 5, Group = "Visual")]
        public int LabelOffsetBars { get; set; }

        // ────────────────────────────────────────────── Line Colors
        [Parameter("Base Line Color", DefaultValue = "Gold", Group = "Colors")]
        public Color BaseLineColor { get; set; }

        [Parameter("SL Line Color", DefaultValue = "Red", Group = "Colors")]
        public Color SLLineColor { get; set; }

        [Parameter("Entry Line Color", DefaultValue = "LimeGreen", Group = "Colors")]
        public Color EntryLineColor { get; set; }

        [Parameter("TP Line Color", DefaultValue = "DodgerBlue", Group = "Colors")]
        public Color TPLineColor { get; set; }

        [Parameter("Retrace Line Color", DefaultValue = "Orange", Group = "Colors")]
        public Color RetraceLineColor { get; set; }

        // ────────────────────────────────────────────── Dashboard Visibility Toggles
        [Parameter("Show Profile & Mode", DefaultValue = true, Group = "Dashboard Display")]
        public bool ShowProfileMode { get; set; }

        [Parameter("Show Renko Size", DefaultValue = true, Group = "Dashboard Display")]
        public bool ShowRenko { get; set; }

        [Parameter("Show Recommended Lot", DefaultValue = true, Group = "Dashboard Display")]
        public bool ShowLot { get; set; }

        [Parameter("Show SL Distance", DefaultValue = true, Group = "Dashboard Display")]
        public bool ShowSL { get; set; }

        [Parameter("Show Entry Distance", DefaultValue = true, Group = "Dashboard Display")]
        public bool ShowEntry { get; set; }

        [Parameter("Show TP Distance", DefaultValue = true, Group = "Dashboard Display")]
        public bool ShowTP { get; set; }

        [Parameter("Show Risk:Reward Ratio", DefaultValue = true, Group = "Dashboard Display")]
        public bool ShowRR { get; set; }

        [Parameter("Show Instructions [M/Click]", DefaultValue = true, Group = "Dashboard Display")]
        public bool ShowInstructions { get; set; }

        private double renkoSizePips;
        private string recommendedLot;
        private double distanceToSL_Pips;     // Base → SL
        private double distanceToEntry_Pips;  // SL → Entry
        private double distanceToTP_Pips;     // Entry → TP  =  10 × distanceToEntry_Pips

        private ChartHorizontalLine baseLine, slLine, entryLine, tpLine, retraceLine;
        private ChartText labelBase, labelSL, labelEntry, labelTP, labelRetrace;

        private bool linesCreated = false;
        private ChartStaticText dashboardText;
        private RenkoProfile lastProfile;

        protected override void Initialize()
        {
            lastProfile = SelectedProfile;
            UpdateProfileValues();

            dashboardText = Chart.DrawStaticText("Dashboard", "",
                VerticalAlignment.Top, HorizontalAlignment.Left, Color.WhiteSmoke);

            UpdateDashboard();

            Chart.MouseDown += Chart_MouseDown;
            Chart.KeyDown += Chart_KeyDown;
        }

        private void UpdateProfileValues()
        {
            switch (SelectedProfile)
            {
                case RenkoProfile.Profile_7ATR:
                    renkoSizePips = 4;
                    recommendedLot = "0.3";
                    distanceToSL_Pips    = 10;
                    distanceToEntry_Pips = 30;
                    break;

                case RenkoProfile.Profile_14ATR:
                    renkoSizePips = 8;
                    recommendedLot = "0.15";
                    distanceToSL_Pips    = 20;
                    distanceToEntry_Pips = 60;
                    break;

                case RenkoProfile.Profile_28ATR:
                    renkoSizePips = 15;
                    recommendedLot = "0.07";
                    distanceToSL_Pips    = 45;
                    distanceToEntry_Pips = 130;
                    break;

                case RenkoProfile.Profile_56ATR:
                    renkoSizePips = 30;
                    recommendedLot = "0.03";
                    distanceToSL_Pips    = 90;
                    distanceToEntry_Pips = 300;
                    break;

                default:
                    renkoSizePips = 4;
                    recommendedLot = "0.3";
                    distanceToSL_Pips    = 10;
                    distanceToEntry_Pips = 30;
                    break;
            }

            // Hitung TP sesuai permintaan (10 × jarak Entry ke SL)
            distanceToTP_Pips = 10 * distanceToEntry_Pips;
        }

        private void UpdateDashboard()
        {
            if (dashboardText == null) return;

            string modeSymbol = Mode == ReverseMode.Short ? "▼ SHORT" : "▲ LONG";
            string profileName = SelectedProfile.ToString().Replace("Profile_", "");

            var lines = new List<string>();

            // Header pemisah (selalu tampilkan agar rapi)
            lines.Add("────────────────────────");

            // Profile & Mode
            if (ShowProfileMode)
            {
                lines.Add($"Profile : {profileName,-10}");
                lines.Add($"Mode    : {modeSymbol}");
            }

            // Renko size
            if (ShowRenko)
            {
                lines.Add($"Renko   : {renkoSizePips} pips");
            }

            // Lot size
            if (ShowLot)
            {
                lines.Add($"Lot     : {recommendedLot,-12}");
            }

            // SL distance
            if (ShowSL)
            {
                lines.Add($"SL      : {distanceToSL_Pips,5:F1} pips dari Base");
            }

            // Entry distance
            if (ShowEntry)
            {
                lines.Add($"Entry   : {distanceToEntry_Pips,5:F1} pips dari SL");
            }

            // TP distance
            if (ShowTP)
            {
                lines.Add($"TP      : {distanceToTP_Pips,5:F1} pips dari Entry");
            }

            // Risk:Reward
            if (ShowRR && distanceToEntry_Pips > 0)
            {
                double rr = distanceToTP_Pips / distanceToEntry_Pips;
                lines.Add($"R:R     : 1 : {rr:F0}");
            }

            // Footer instructions
            if (ShowInstructions)
            {
                lines.Add("────────────────────────");
                lines.Add("[M] Toggle Direction   [Click] Set Base   [Ctrl+Click] Delete");
            }
            else if (lines.Count > 1)
            {
                lines.Add("────────────────────────");
            }

            dashboardText.Text = string.Join("\n", lines);
        }

        private void Chart_KeyDown(ChartKeyboardEventArgs obj)
        {
            if (obj.Key == Key.M)
            {
                Mode = Mode == ReverseMode.Short ? ReverseMode.Long : ReverseMode.Short;
                Print($"Mode changed to: {(Mode == ReverseMode.Short ? "SHORT" : "LONG")}");
                UpdateDashboard();

                if (linesCreated && baseLine != null)
                    UpdateLines(baseLine.Y);
            }
        }

        private void Chart_MouseDown(ChartMouseEventArgs obj)
        {
            if (linesCreated && RequireCtrlToDelete && obj.CtrlKey)
            {
                RemoveLines();
                return;
            }

            if (!linesCreated && !(RequireCtrlToDelete && obj.CtrlKey))
            {
                CreateLines(obj.YValue);
            }
        }

        private void CreateLines(double basePrice)
        {
            double distSL      = distanceToSL_Pips    * Symbol.PipSize;
            double distEntry   = distanceToEntry_Pips * Symbol.PipSize;
            double distTP      = distanceToTP_Pips    * Symbol.PipSize;
            double distRetrace = renkoSizePips        * Symbol.PipSize;

            double slPrice, entryPrice, tpPrice, retracePrice;

            if (Mode == ReverseMode.Short)
            {
                // Short: SL di atas Base, Entry di bawah SL, TP lebih bawah lagi
                // Minimum Retrace = di ATAS Entry (sebesar renko size)
                slPrice      = basePrice + distSL;
                entryPrice   = slPrice   - distEntry;
                tpPrice      = entryPrice - distTP;
                retracePrice = entryPrice + distRetrace;
            }
            else // Long
            {
                // Long: SL di bawah Base, Entry di atas SL, TP lebih atas lagi
                // Minimum Retrace = di BAWAH Entry (sebesar renko size)
                slPrice      = basePrice - distSL;
                entryPrice   = slPrice   + distEntry;
                tpPrice      = entryPrice + distTP;
                retracePrice = entryPrice - distRetrace;
            }

            // ────────────────────────────────────────────── Gambar garis (menggunakan warna dari parameter)
            baseLine    = Chart.DrawHorizontalLine("BaseLine",     basePrice,    BaseLineColor,    1,                 BaseLineStyle);
            slLine      = Chart.DrawHorizontalLine("SL_Line",      slPrice,      SLLineColor,      LineThickness + 1, LineStyle.Solid);
            entryLine   = Chart.DrawHorizontalLine("Entry_Line",   entryPrice,   EntryLineColor,   LineThickness,     LineStyle.Solid);
            tpLine      = Chart.DrawHorizontalLine("TP_Line",      tpPrice,      TPLineColor,      LineThickness,     LineStyle.Solid);
            retraceLine = Chart.DrawHorizontalLine("Retrace_Line", retracePrice, RetraceLineColor, LineThickness,     LineStyle.DotsRare);

            // ────────────────────────────────────────────── Label (di kanan chart)
            int rightBar = Chart.LastVisibleBarIndex - LabelOffsetBars;

            labelBase    = Chart.DrawText("Label_Base",    $"Base      {basePrice:F5}",                                              rightBar, basePrice,    BaseLineColor);
            labelSL      = Chart.DrawText("Label_SL",      $"SL        {slPrice:F5}  (+{distanceToSL_Pips:F1}p)",                   rightBar, slPrice,      SLLineColor);
            labelEntry   = Chart.DrawText("Label_Entry",   $"Entry     {entryPrice:F5}  ({distanceToEntry_Pips:F1}p dari SL)",     rightBar, entryPrice,   EntryLineColor);
            labelTP      = Chart.DrawText("Label_TP",      $"TP        {tpPrice:F5}  ({distanceToTP_Pips:F1}p dari Entry)",        rightBar, tpPrice,      TPLineColor);
            labelRetrace = Chart.DrawText("Label_Retrace", $"Min Retrace {retracePrice:F5}  ({renkoSizePips:F0}p dari Entry)",     rightBar, retracePrice, RetraceLineColor);

            // Format label sama untuk semua
            var labels = new[] { labelBase, labelSL, labelEntry, labelTP, labelRetrace };
            foreach (var lbl in labels)
            {
                lbl.FontSize = LabelFontSize;
                lbl.HorizontalAlignment = HorizontalAlignment.Right;
            }

            linesCreated = true;
        }

        private void UpdateLines(double basePrice)
        {
            RemoveLines();
            CreateLines(basePrice);
        }

        private void RemoveLines()
        {
            Chart.RemoveObject("BaseLine");
            Chart.RemoveObject("SL_Line");
            Chart.RemoveObject("Entry_Line");
            Chart.RemoveObject("TP_Line");
            Chart.RemoveObject("Retrace_Line");

            Chart.RemoveObject("Label_Base");
            Chart.RemoveObject("Label_SL");
            Chart.RemoveObject("Label_Entry");
            Chart.RemoveObject("Label_TP");
            Chart.RemoveObject("Label_Retrace");

            baseLine = slLine = entryLine = tpLine = retraceLine = null;
            labelBase = labelSL = labelEntry = labelTP = labelRetrace = null;

            linesCreated = false;
        }

        public override void Calculate(int index)
        {
            if (SelectedProfile != lastProfile)
            {
                lastProfile = SelectedProfile;
                UpdateProfileValues();
                UpdateDashboard();

                if (linesCreated && baseLine != null)
                    UpdateLines(baseLine.Y);
            }
        }
    }
}