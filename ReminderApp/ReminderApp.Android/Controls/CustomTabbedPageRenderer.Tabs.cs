using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using Com.Ittianyu.Bottomnavigationviewex;
using ReminderApp.Droid.Utils;
using Xamarin.Forms;

namespace ReminderApp.Droid.Controls
{
    public partial class CustomTabbedPageRenderer : BottomNavigationViewEx.IOnNavigationItemSelectedListener
    {
        public static int? ItemBackgroundResource;
        public static ColorStateList ItemIconTintList;
        public static ColorStateList ItemTextColor;
        public static Android.Graphics.Color? BackgroundColor;
        public static Typeface Typeface;
        public static float? IconSize;
        public static float? FontSize;
        public static float ItemSpacing;
        public static ItemAlignFlags ItemAlign;
        public static Thickness ItemPadding;
        public static bool? VisibleTitle;

        internal int CurrentMenuItemId = 0;

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            this.SwitchPage(item);
            return true;
        }

        internal void SetupTabItems()
        {
            this.SetupTabItems(bottomNav);
        }

        internal void SetupBottomBar()
        {
            bottomNav = this.SetupBottomBar(rootLayout, bottomNav, barId);
        }
    }

    public enum ItemAlignFlags
    {
        Default, Center, Top, Bottom
    }

}