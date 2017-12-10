using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Transitions;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using GarlandView.Droid.Details;
using Newtonsoft.Json;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using GarlandView.Droid.Utils;

namespace GarlandView.Droid.Profile
{
    [Activity(Label = "ProfileActivity", Theme = "@style/AppTheme.NoActionBar")]
    public class ProfileActivity : AppCompatActivity, AppBarLayout.IOnOffsetChangedListener, Transition.ITransitionListener
    {

        private const string BUNDLE_NAME = "BUNDLE_NAME";
        private const string BUNDLE_INFO = "BUNDLE_INFO";
        private const string BUNDLE_STATUS = "BUNDLE_STATUS";
        private const string BUNDLE_AVATAR_URL = "BUNDLE_AVATAR_URL";
        private const string BUNDLE_LIST_DATA = "BUNDLE_LIST_DATA";

        int avatarHOffset;
        int avatarVOffset;
        int avatarSize;
        int textHOffset;
        int textVMinOffset;
        int textVMaxOffset;
        int textVDiff;
        int header160;

        int toolBarHeight;

        private Toolbar toolbar;

        private TextView tvTitle;
        private TextView tvName;
        private TextView tvInfo;
        private TextView tvStatus;

        private RecyclerView recyclerView;

        private ImageView ivAvatar;

        private AppBarLayout appBar;

        private View headerImage;
        private View headerInfo;
        private View avatarBorder;

        private LinearLayout texts;

        private List<float> textStart = new List<float>();

        bool isStarting = true;

        public static void Start(Activity activity,
            String url, String name, String info, String status,
            View avatar, View card, View image, View list,
            List<DetailsData> listData)
        {
            Intent intent = new Intent(activity, typeof(ProfileActivity));

            intent.PutExtra(BUNDLE_NAME, name);
            intent.PutExtra(BUNDLE_INFO, info);
            intent.PutExtra(BUNDLE_STATUS, status);
            intent.PutExtra(BUNDLE_AVATAR_URL, url);
            intent.PutExtra(BUNDLE_LIST_DATA, JsonConvert.SerializeObject(listData));

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {

                Pair p1 = Pair.Create(avatar, activity.GetString(Resource.String.transition_avatar_border));
                Pair p2 = Pair.Create(card, activity.GetString(Resource.String.transition_card));
                Pair p3 = Pair.Create(image, activity.GetString(Resource.String.transition_background));
                Pair p4 = Pair.Create(list, activity.GetString(Resource.String.transition_list));

                ActivityOptions options = ActivityOptions.MakeSceneTransitionAnimation(activity, p1, p2, p3, p4);
                activity.StartActivity(intent, options.ToBundle());
            } else {
                activity.StartActivity(intent);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_profile);

            InitViews();
            InitData();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Window.SharedElementEnterTransition.AddListener(this);
            }
        }

        private void InitViews()
        {
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            tvTitle = FindViewById<TextView>(Resource.Id.tv_title);
            tvName = FindViewById<TextView>(Resource.Id.tv_name);
            tvInfo = FindViewById<TextView>(Resource.Id.tv_info);
            tvStatus = FindViewById<TextView>(Resource.Id.tv_status);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);

            ivAvatar = FindViewById<ImageView>(Resource.Id.avatar);

            appBar = FindViewById<AppBarLayout>(Resource.Id.app_bar);

            headerImage = FindViewById<View>(Resource.Id.header_image);
            headerInfo = FindViewById<View>(Resource.Id.header_info);
            avatarBorder = FindViewById<View>(Resource.Id.avatar_border);

            texts = FindViewById<LinearLayout>(Resource.Id.texts);
        }

        private void InitData()
        {
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            string fullName = Intent.GetStringExtra(BUNDLE_NAME);
            string title = fullName.Split(' ')[0] + GetString(Resource.String.profile);

            List<DetailsData> listData = JsonConvert.DeserializeObject<List<DetailsData>>(Intent.GetStringExtra(BUNDLE_LIST_DATA));
            recyclerView.SetAdapter(new ProfileAdapter(listData));

            Glide.With(this)
                .Load(Intent.GetStringExtra(BUNDLE_AVATAR_URL))
                .Placeholder(Resource.Drawable.avatar_placeholder)
                .Transform(new ImageCircleTransformation(this))
                .Into(ivAvatar);

            avatarHOffset = Resources.GetDimensionPixelSize(Resource.Dimension.profile_avatar_h_offset);
            avatarVOffset = Resources.GetDimensionPixelSize(Resource.Dimension.profile_avatar_v_offset);
            avatarSize = Resources.GetDimensionPixelSize(Resource.Dimension.profile_avatar_size);
            textHOffset = Resources.GetDimensionPixelSize(Resource.Dimension.profile_texts_h_offset);
            textVMinOffset = Resources.GetDimensionPixelSize(Resource.Dimension.profile_texts_v_min_offset);
            textVMaxOffset = Resources.GetDimensionPixelSize(Resource.Dimension.profile_texts_v_max_offset);
            textVDiff = textVMaxOffset - textVMinOffset;
            header160 = Resources.GetDimensionPixelSize(Resource.Dimension.dp160);

            TypedArray styledAttributes = Theme.ObtainStyledAttributes(new int[] { Android.Resource.Attribute.ActionBarSize });

            toolBarHeight = (int)styledAttributes.GetDimension(0, 0) + Commons.GetStatusBarHeight(this);
            styledAttributes.Recycle();

            avatarBorder.PivotX = 0;
            avatarBorder.PivotY = 0;
            texts.PivotX = 0;
            texts.PivotY = 0;

            appBar.AddOnOffsetChangedListener(this);
        }

        public void OnOffsetChanged(AppBarLayout appBarLayout, int verticalOffset)
        {
            int diff = toolBarHeight + verticalOffset;
            int y = diff < 0 ? header160 - diff : header160;

            headerInfo.Top = y;

            FrameLayout.LayoutParams lp = (FrameLayout.LayoutParams)headerImage.LayoutParameters;

            lp.Height = y;

            headerImage.LayoutParameters = lp;

            int totalScrollRange = appBarLayout.TotalScrollRange;
            float ratio = ((float)totalScrollRange + verticalOffset) / totalScrollRange;

            int avatarHalf = avatarBorder.MeasuredHeight / 2;
            int avatarRightest = appBarLayout.MeasuredWidth / 2 - avatarHalf - avatarHOffset;
            int avatarTopest = avatarHalf + avatarVOffset;

            avatarBorder.SetX(avatarHOffset + avatarRightest * ratio);
            avatarBorder.SetY(avatarVOffset - avatarTopest * ratio);
            avatarBorder.ScaleX = 0.5f + 0.5f * ratio;
            avatarBorder.ScaleY = 0.5f + 0.5f * ratio;

            if (!textStart.Any() && verticalOffset == 0)
            {
                for (int i = 0; i < texts.ChildCount; i++)
                {
                    textStart.Add(texts.GetChildAt(i).GetX());
                }
            }

            texts.SetX(textHOffset + (avatarSize * 0.5f - avatarVOffset) * (1f - ratio));
            texts.SetY(textVMinOffset + textVDiff * ratio);
            texts.ScaleX = 0.8f + 0.2f * ratio;
            texts.ScaleY = 0.8f + 0.2f * ratio;

            for (int i = 0; i < textStart.Count; i++)
            {
                texts.GetChildAt(i).SetX(textStart[i] * ratio);
            }
        }

        public void OnTransitionStart(Transition transition)
        {
            if (isStarting)
            {
                isStarting = false;

                ViewCompat.SetTransitionName(headerImage, null);
                ViewCompat.SetTransitionName(recyclerView, null);
            }
        }

        public override bool OnSupportNavigateUp()
        {
            base.OnSupportNavigateUp();

            return true;
        }

        public void OnTransitionCancel(Transition transition) {}

        public void OnTransitionEnd(Transition transition) {}

        public void OnTransitionPause(Transition transition) {}

        public void OnTransitionResume(Transition transition){}

    }
}