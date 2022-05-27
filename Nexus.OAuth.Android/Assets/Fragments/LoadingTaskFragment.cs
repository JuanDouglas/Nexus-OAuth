using Android.OS;
using Android.Views;
using Android.Views.Animations;
using AndroidX.AppCompat.App;
using AndroidX.CardView.Widget;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Nexus.OAuth.Android.Assets.Fragments
{
    public class LoadingTaskFragment : AppCompatDialogFragment
    {
        private const int AnimationDuration = 700;
        private const int MinLoadingTime = 2500;
        public const string TAG = "LoadingTaskFragment";

        public event EventHandler TaskComplet;
        public Task Task { get; set; }
        public override int Theme => Resource.Style.AppTheme_Dialog_FullScreen;
        public LoadingTaskFragment(Task task, bool exitable = false)
        {
            Task = task;
            Cancelable = exitable;
            stopwatch = new Stopwatch();
            TaskComplet += (object sender, EventArgs args) => { };
        }

        private CardView card;
        private View background;
        private Stopwatch stopwatch;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.dialog_loading_task, container, false);

            card = view.FindViewById<CardView>(Resource.Id.cardLoading);
            background = view.FindViewById(Resource.Id.bgLoading);
            Task.ContinueWith(EndTask);

            AlphaAnimation alpAnim = new AlphaAnimation(0f, 1f)
            {
                Duration = AnimationDuration
            };

            Animation transAnim = AnimationUtils.LoadAnimation(Activity, Resource.Animation.card_slide_in_bottom);
            transAnim.Interpolator = new AccelerateDecelerateInterpolator();
            transAnim.AnimationEnd += (object sender, Animation.AnimationEndEventArgs args) =>
            {
                stopwatch.Start();
                Task.Start();
            };

            card.StartAnimation(transAnim);
            background.StartAnimation(alpAnim);
            return view;
        }
        private void EndTask(Task task)
        {
            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds < MinLoadingTime)
            {
                Task ts = new Task(()
                    => Thread.Sleep((int)(MinLoadingTime - stopwatch.ElapsedMilliseconds)));
                ts.ContinueWith(t => Dismiss());
                ts.Start();
                return;
            }

            Dismiss();
        }
        public override void Dismiss()
        {
            AlphaAnimation alpAnim = new AlphaAnimation(1f, 0f)
            {
                Duration = AnimationDuration
            };

            Animation transAnim = AnimationUtils.LoadAnimation(Activity, Resource.Animation.card_slide_out_bottom);
            transAnim.Interpolator = new AccelerateDecelerateInterpolator();
            transAnim.AnimationEnd += (object sender, Animation.AnimationEndEventArgs args)
                 => Activity.RunOnUiThread(()
                    => base.Dismiss());

            background.StartAnimation(alpAnim);
            card.StartAnimation(transAnim);
        }
    }
}