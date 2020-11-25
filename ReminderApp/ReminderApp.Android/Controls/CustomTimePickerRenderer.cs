using System;
using System.ComponentModel;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ReminderApp.Droid.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Xamarin.Forms.TimePicker), typeof(CustomTimePickerRenderer))]
namespace ReminderApp.Droid.Controls
{
    public class CustomTimePickerRenderer : TimePickerRenderer
    {
        TimePickerDialogIntervals timePickerDlg;

        public CustomTimePickerRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.TimePicker> e)
        {
            base.OnElementChanged(e);

            var is24HourFormat = Android.Text.Format.DateFormat.Is24HourFormat(Android.App.Application.Context);

            timePickerDlg = new TimePickerDialogIntervals(this.Context, new EventHandler<TimePickerDialogIntervals.TimeSetEventArgs>(UpdateDuration),
            Element.Time.Hours, Element.Time.Minutes, is24HourFormat);

            var control = new EditText(this.Context);
            control.Focusable = false;
            control.FocusableInTouchMode = false;
            control.Clickable = false;
            control.Click += (sender, ea) => ShowTimePickerDiaglog(is24HourFormat);

            if (!is24HourFormat)
            {
                control.Text = ToTwelveHourFormat(Element.Time.Hours, Element.Time.Minutes);
            }
            else
            {
                control.Text = Element.Time.Hours.ToString("00") + ":" + Element.Time.Minutes.ToString("00");
            }

            control.TextSize = 16;
            SetNativeControl(control);
        }

        private void ShowTimePickerDiaglog(bool is24HourFormat)
        {
            timePickerDlg = new TimePickerDialogIntervals(this.Context, new EventHandler<TimePickerDialogIntervals.TimeSetEventArgs>(UpdateDuration),
            Element.Time.Hours, Element.Time.Minutes, is24HourFormat);

            if (Control.Text.Trim().Equals(""))
            {
                timePickerDlg.UpdateTime(DateTime.Now.Hour, DateTime.Now.Minute);
            }
            else
            {
                timePickerDlg.UpdateTime(Element.Time.Hours, Element.Time.Minutes);
            }
            timePickerDlg.Show();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var element = (Xamarin.Forms.TimePicker)sender;
            timePickerDlg.UpdateTime(DateTime.Now.Hour, DateTime.Now.Minute);
        }

        void UpdateDuration(object sender, Android.App.TimePickerDialog.TimeSetEventArgs e)
        {
            var is24h = Android.Text.Format.DateFormat.Is24HourFormat(Android.App.Application.Context);
            Element.Time = new TimeSpan(e.HourOfDay, e.Minute, 0);
            if (!is24h)
            {
                Control.Text = ToTwelveHourFormat(Element.Time.Hours, Element.Time.Minutes);
            }
            else
            {
                Control.Text = Element.Time.Hours.ToString("00") + ":" + Element.Time.Minutes.ToString("00");
            }
        }

        private string ToTwelveHourFormat(int hours, int minutes)
        {
            var hourFormat = " AM";
            string outputHours = hours.ToString("00");

            if (hours == 0)
            {
                outputHours = "12";
            }
            else if (hours >= 12)
            {
                if (Element.Time.Hours > 12)
                {
                    outputHours = (hours - 12).ToString("00");
                }
                hourFormat = " PM";
            }

            return (outputHours + ":" + minutes.ToString("00") + hourFormat);
        }
    }

    public class TimePickerDialogIntervals : TimePickerDialog
    {
        public const int TimePickerInterval = 1;
        private bool _ignoreEvent = false;

        public TimePickerDialogIntervals(Context context, EventHandler<TimePickerDialog.TimeSetEventArgs> callBack, int hourOfDay, int minute, bool is24HourView)
            : base(context, TimePickerDialog.ThemeHoloLight, (sender, e) =>
            {
                callBack(sender, new TimePickerDialog.TimeSetEventArgs(e.HourOfDay, e.Minute));
            }, hourOfDay, minute / TimePickerInterval, is24HourView)
        {
            FixSpinner(context, hourOfDay, minute, is24HourView);
        }
        protected TimePickerDialogIntervals(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void SetView(Android.Views.View view)
        {
            ((ViewGroup)view).DescendantFocusability = DescendantFocusability.BlockDescendants;
            SetupMinutePicker(view);
            base.SetView(view);
        }

        void SetupMinutePicker(Android.Views.View view)
        {
            var numberPicker = FindMinuteNumberPicker(view as ViewGroup);
            if (numberPicker != null)
            {
                //numberPicker.MinValue = 0;
                //numberPicker.MaxValue = 59;
                //numberPicker.SetDisplayedValues(new String[] { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
                //                                                "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
                //                                                "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
                //                                                "30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
                //                                                "40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
                //                                                "50", "51", "52", "53", "54", "55", "56", "57", "58", "59",});
            }
        }

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        private NumberPicker FindMinuteNumberPicker(ViewGroup viewGroup)
        {
            for (var i = 0; i < viewGroup.ChildCount; i++)
            {
                var child = viewGroup.GetChildAt(i);
                var numberPicker = child as NumberPicker;
                if (numberPicker != null)
                {
                    if (numberPicker.MaxValue == 59)
                    {
                        return numberPicker;
                    }
                }

                var childViewGroup = child as ViewGroup;
                if (childViewGroup != null)
                {
                    var childResult = FindMinuteNumberPicker(childViewGroup);
                    if (childResult != null)
                        return childResult;
                }
            }

            return null;
        }

        private void FixSpinner(Context context, int hourOfDay, int minute, bool is24HourView)
        {
            if (Build.VERSION.SdkInt > BuildVersionCodes.N)
                return;
            try
            {
                // Get the theme's android:timePickerMode
                int MODE_SPINNER = 1;
                var styleableClass = Java.Lang.Class.ForName("com.android.internal.R$styleable");
                var timePickerStyleableField = styleableClass.GetField("TimePicker");
                int[] timePickerStyleable = (int[])timePickerStyleableField.Get(null);
                var a = context.ObtainStyledAttributes(null, timePickerStyleable, Android.Resource.Attribute.TimePickerStyle, 0);
                var timePickerModeStyleableField = styleableClass.GetField("TimePicker_timePickerMode");
                int timePickerModeStyleable = timePickerModeStyleableField.GetInt(null);
                int mode = a.GetInt(timePickerModeStyleable, MODE_SPINNER);
                a.Recycle();

                Android.Widget.TimePicker timePicker = (Android.Widget.TimePicker)findField(Java.Lang.Class.FromType(typeof(TimePickerDialog)), Java.Lang.Class.FromType(typeof(Android.Widget.TimePicker)), "mTimePicker").Get(this);
                var delegateClass = Java.Lang.Class.ForName("android.widget.TimePicker$TimePickerDelegate");
                var delegateField = findField(Java.Lang.Class.FromType(typeof(Android.Widget.TimePicker)), delegateClass, "mDelegate");
                var delegatee = delegateField.Get(timePicker);
                Java.Lang.Class spinnerDelegateClass;
                if (Build.VERSION.SdkInt != BuildVersionCodes.Lollipop)
                {
                    spinnerDelegateClass = Java.Lang.Class.ForName("android.widget.TimePickerSpinnerDelegate");
                }
                else
                {
                    // TimePickerSpinnerDelegate was initially misnamed TimePickerClockDelegate in API 21!
                    spinnerDelegateClass = Java.Lang.Class.ForName("android.widget.TimePickerClockDelegate");
                }

                // In 7.0 Nougat for some reason the timePickerMode is ignored and the delegate is TimePickerClockDelegate
                if (delegatee.Class != spinnerDelegateClass)
                {
                    delegateField.Set(timePicker, null); // throw out the TimePickerClockDelegate!
                    timePicker.RemoveAllViews(); // remove the TimePickerClockDelegate views
                    var spinnerDelegateConstructor = spinnerDelegateClass.GetConstructors()[0];
                    spinnerDelegateConstructor.Accessible = true;
                    // Instantiate a TimePickerSpinnerDelegate
                    delegatee = spinnerDelegateConstructor.NewInstance(timePicker, context, null, Android.Resource.Attribute.TimePickerStyle, 0);
                    delegateField.Set(timePicker, delegatee); // set the TimePicker.mDelegate to the spinner delegate
                                                              // Set up the TimePicker again, with the TimePickerSpinnerDelegate
                    timePicker.SetIs24HourView(Java.Lang.Boolean.ValueOf(is24HourView));
                    timePicker.Hour = hourOfDay;
                    timePicker.Minute = minute;
                    timePicker.SetOnTimeChangedListener(this);
                }
                // set interval
                SetupMinutePicker(timePicker);
            }
            catch (Exception e)
            {

            }
        }

        private static Java.Lang.Reflect.Field findField(Java.Lang.Class objectClass, Java.Lang.Class fieldClass, String expectedName)
        {
            try
            {
                var field = objectClass.GetDeclaredField(expectedName);
                field.Accessible = true;
                return field;
            }
            catch (Java.Lang.NoSuchFieldException e) { } // ignore
                                                         // search for it if it wasn't found under the expected ivar name
            foreach (var searchField in objectClass.GetDeclaredFields())
            {
                if (Java.Lang.Class.FromType(searchField.GetType()) == fieldClass)
                {
                    searchField.Accessible = true;
                    return searchField;
                }
            }
            return null;
        }
    }
}