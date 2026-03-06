using System.Drawing;
using System.Windows.Forms;

namespace Client.Constants
{
    /// <summary>
    /// Centralized dark "gaming" theme applied consistently across all WinForms.
    /// Usage: call UITheme.Apply(this) from any Form constructor after InitializeComponent().
    /// </summary>
    public static class UITheme
    {
        // ── Colour palette ────────────────────────────────────────────────────
        public static readonly Color BgDark       = Color.FromArgb(18,  24,  38);   // near-black navy
        public static readonly Color BgMedium     = Color.FromArgb(24,  34,  52);   // dark navy
        public static readonly Color BgPanel      = Color.FromArgb(32,  44,  68);   // panel / sidebar
        public static readonly Color AccentBlue   = Color.FromArgb(52, 152, 219);   // primary action
        public static readonly Color AccentGreen  = Color.FromArgb(39, 174,  96);   // success / join
        public static readonly Color AccentRed    = Color.FromArgb(192, 57,  43);   // danger / leave
        public static readonly Color AccentAmber  = Color.FromArgb(230, 126,  34);  // matchmaking
        public static readonly Color BtnSecondary = Color.FromArgb(44,  62,  88);   // back / cancel
        public static readonly Color TextPrimary  = Color.White;
        public static readonly Color TextMuted    = Color.FromArgb(149, 165, 166);
        public static readonly Color TextAccent   = Color.FromArgb(100, 180, 255);
        public static readonly Color InputBg      = Color.FromArgb(26,  38,  58);
        public static readonly Color BorderColor  = Color.FromArgb(52,  73,  94);

        // Cell colours (3×3 game boards)
        public static readonly Color CellEmpty  = Color.FromArgb(32,  44,  68);
        public static readonly Color CellX      = Color.FromArgb(52, 152, 219);   // player X — blue
        public static readonly Color CellO      = Color.FromArgb(192, 57,  43);   // player O / bot — red
        public static readonly Color CellBlink1 = Color.FromArgb(80,  50,  30);   // blinking warn (dark)
        public static readonly Color CellBlink2 = Color.FromArgb(40,  70,  45);   // blinking warn (alt)

        // ── Typography ────────────────────────────────────────────────────────
        public static readonly Font TitleFont   = new Font("Segoe UI",          22F, FontStyle.Bold);
        public static readonly Font HeadingFont = new Font("Segoe UI Semibold", 13F, FontStyle.Bold);
        public static readonly Font BodyFont    = new Font("Segoe UI",         10.5F);
        public static readonly Font ButtonFont  = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        public static readonly Font CellFont    = new Font("Segoe UI",          18F, FontStyle.Bold);
        public static readonly Font SmallFont   = new Font("Segoe UI",           9F);

        // ── Master apply ──────────────────────────────────────────────────────
        /// <summary>
        /// Recursively applies the dark theme to every control in <paramref name="form"/>.
        /// Call after InitializeComponent().
        /// </summary>
        public static void Apply(Form form)
        {
            form.BackColor = BgDark;
            form.ForeColor = TextPrimary;
            ApplyControls(form.Controls);
        }

        private static void ApplyControls(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                switch (c)
                {
                    case Button     btn: StyleButton(btn);     break;
                    case Label      lbl: StyleLabel(lbl);      break;
                    case LinkLabel   ll: StyleLinkLabel(ll);   break;
                    case TextBox    txt: StyleTextBox(txt);    break;
                    case RichTextBox rb: StyleRichTextBox(rb); break;
                    case ListBox     lb: StyleListBox(lb);     break;
                    case ListView    lv: StyleListView(lv);    break;
                    case ComboBox    cb: StyleComboBox(cb);    break;
                    case Panel      pnl: StylePanel(pnl);      break;
                }
                if (c.Controls.Count > 0)
                    ApplyControls(c.Controls);
            }
        }

        // ── Per-control helpers ───────────────────────────────────────────────

        public static void StyleButton(Button btn)
        {
            Color bg = GetButtonColor(btn);
            btn.BackColor = bg;
            btn.ForeColor = TextPrimary;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = btn.Parent?.Name == "panelGame" ? CellFont : ButtonFont;
            btn.Cursor = Cursors.Hand;
            btn.UseVisualStyleBackColor = false;
        }

        private static Color GetButtonColor(Button btn)
        {
            // Game-board cells always get the cell colour
            if (btn.Parent?.Name == "panelGame") return CellEmpty;

            string name = (btn.Name ?? "").ToLowerInvariant();
            string text = (btn.Text ?? "").ToLowerInvariant();

            // Back / cancel  
            if (name.Contains("back") || name.Contains("cancel") || name.Contains("quay") ||
                text == "back" || text == "quay lại" || text == "hủy")
                return BtnSecondary;

            // Leave / danger
            if (name.Contains("leave") || name.Contains("quit") ||
                text == "rời phòng" || text == "thoát")
                return AccentRed;

            // Reset / refresh / success
            if (name.Contains("reset") || name.Contains("refresh") || name == "btnrefresh")
                return AccentGreen;

            // Matchmaking
            if (name.Contains("match") || name.Contains("ghep"))
                return AccentAmber;

            return AccentBlue;
        }

        public static void StyleLabel(Label lbl)
        {
            lbl.BackColor = Color.Transparent;
            string n = (lbl.Name ?? "").ToLowerInvariant();
            bool isTitle = n.Contains("title") || n == "lbtitle" || n == "lbltitle";
            lbl.ForeColor = isTitle ? TextAccent : TextPrimary;
            if (isTitle) lbl.Font = TitleFont;
        }

        public static void StyleLinkLabel(LinkLabel ll)
        {
            ll.BackColor = Color.Transparent;
            ll.ForeColor = TextMuted;
            ll.LinkColor = TextMuted;
            ll.ActiveLinkColor = AccentBlue;
        }

        public static void StyleTextBox(TextBox txt)
        {
            txt.BackColor = InputBg;
            txt.ForeColor = TextPrimary;
            txt.BorderStyle = BorderStyle.FixedSingle;
        }

        public static void StyleRichTextBox(RichTextBox rtb)
        {
            rtb.BackColor = InputBg;
            rtb.ForeColor = TextPrimary;
            rtb.BorderStyle = BorderStyle.None;
        }

        public static void StyleListBox(ListBox lb)
        {
            lb.BackColor = BgPanel;
            lb.ForeColor = TextPrimary;
            lb.BorderStyle = BorderStyle.FixedSingle;
        }

        public static void StyleListView(ListView lv)
        {
            lv.BackColor = BgPanel;
            lv.ForeColor = TextPrimary;
            lv.BorderStyle = BorderStyle.FixedSingle;
        }

        public static void StyleComboBox(ComboBox cb)
        {
            cb.BackColor = InputBg;
            cb.ForeColor = TextPrimary;
            cb.FlatStyle = FlatStyle.Flat;
        }

        public static void StylePanel(Panel pnl)
        {
            pnl.BackColor = pnl.Name == "panelGame" ? BgPanel : BgMedium;
        }
    }
}
