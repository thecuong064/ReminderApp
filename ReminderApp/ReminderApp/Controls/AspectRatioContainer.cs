using ReminderApp.Interfaces;
using Xamarin.Forms;

namespace ReminderApp.Controls
{
    public class AspectRatioContainer : ContentView
    {

        protected override SizeRequest OnSizeRequest(double widthConstraint, double heightConstraint)
        {
            return new SizeRequest(new Size(widthConstraint, widthConstraint * this.AspectRatio));
        }

        public static BindableProperty AspectRatioProperty = BindableProperty.Create(nameof(AspectRatio), typeof(double), typeof(AspectRatioContainer), (double)1);

        public double AspectRatio
        {
            get { return (double)this.GetValue(AspectRatioProperty); }
            set
            {
                this.SetValue(AspectRatioProperty, value);
            }
        }

        public static BindableProperty ImageToSizeProperty = BindableProperty.Create(nameof(ImageToSize), typeof(string), typeof(AspectRatioContainer), null, propertyChanged: OnImageToSizeChanged);

        private static async void OnImageToSizeChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (newvalue == null && oldvalue == null) return;
            var size = new Size();
            if (newvalue != null)
                size = await DependencyService.Get<IImageSizer>().GetSizeForImage((string)newvalue);
            else
                size = await DependencyService.Get<IImageSizer>().GetSizeForImage((string)oldvalue);
            if (size.Width == 0)
                return;
            (bindable as AspectRatioContainer).AspectRatio = size.Height / size.Width;
        }

        public string ImageToSize
        {
            get { return (string)this.GetValue(ImageToSizeProperty); }
            set { this.SetValue(ImageToSizeProperty, value); }
        }
    }
}
