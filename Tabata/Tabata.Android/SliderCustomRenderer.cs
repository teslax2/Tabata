using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Tabata.Droid;
using Tabata.View;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(SliderCustom), typeof(SliderCustomRenderer))]
namespace Tabata.Droid
{
    public class SliderCustomRenderer:SliderRenderer
    {
        public SliderCustomRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                base.OnElementChanged(e);

                if (e.NewElement != null)
                {
                    Control.ProgressDrawable.SetColorFilter(
                    new PorterDuffColorFilter(
                                                Xamarin.Forms.Color.FromHex("#F50F76").ToAndroid(),
                                                PorterDuff.Mode.SrcIn));

                    // Set Progress bar Thumb color
                    Control.Thumb.SetColorFilter(
                        Xamarin.Forms.Color.FromHex("#F50F76").ToAndroid(),
                        PorterDuff.Mode.SrcIn);

                    //Change height
                    GradientDrawable p = new GradientDrawable();
                    p.SetCornerRadius(20);
                    p.SetColor(0x70b23f);
                    ClipDrawable progress = new ClipDrawable(p, GravityFlags.Left, ClipDrawableOrientation.Horizontal);
                    GradientDrawable background = new GradientDrawable();
                    background.SetColor(0xe0e0e0);
                    background.SetCornerRadius(20);
                    LayerDrawable pd = new LayerDrawable(new Drawable[] { background, progress });
                    Control.SetProgressDrawableTiled(pd);
                }
            }
        }
    }
}