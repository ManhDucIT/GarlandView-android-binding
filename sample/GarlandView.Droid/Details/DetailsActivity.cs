using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GarlandView.Droid.Main;
using GarlandView.Droid.Main.Inner;
using Android.Util;
using Com.Bumptech.Glide;
using GarlandView.Droid.Profile;

namespace GarlandView.Droid.Details
{
    [Activity(Label = "DetailsActivity", Theme = "@style/AppTheme.NoActionBar")]
    public class DetailsActivity : AppCompatActivity
    {

        private const int ITEM_COUNT = 4;

        private const string BUNDLE_NAME = "BUNDLE_NAME";
        private const string BUNDLE_INFO = "BUNDLE_INFO";
        private const string BUNDLE_AVATAR_URL = "BUNDLE_AVATAR_URL";

        private TextView tvName;
        private TextView tvInfo;
        private TextView tvStatus;

        private ImageView ivAvatar;

        private View card;

        private ImageView ivBackground;

        private RecyclerView recyclerView;

        private LinearLayout linearLayout2;
        private LinearLayout detailsParentLayout;

        private List<DetailsData> mListData = new List<DetailsData>();

        public static void Start(MainActivity activity, 
            string name, string address, string url, 
            View card, View avatar)
        {
            Intent intent = new Intent(activity, typeof(DetailsActivity));

            intent.PutExtra(BUNDLE_NAME, name);
            intent.PutExtra(BUNDLE_INFO, address);
            intent.PutExtra(BUNDLE_AVATAR_URL, url);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {
                Pair p1 = Pair.Create(card, activity.GetString(Resource.String.transition_card));
                Pair p2 = Pair.Create(avatar, activity.GetString(Resource.String.transition_card));

                ActivityOptions options = ActivityOptions.MakeSceneTransitionAnimation(activity, p1, p2);
                activity.StartActivity(intent, options.ToBundle());
            } else {
                activity.StartActivity(intent);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_details);

            InitViews();
            InitData();
        }

        private void InitViews()
        {
            tvName = FindViewById<TextView>(Resource.Id.tv_name);
            tvInfo = FindViewById<TextView>(Resource.Id.tv_info);
            tvStatus = FindViewById<TextView>(Resource.Id.tv_status);

            ivAvatar = FindViewById<ImageView>(Resource.Id.avatar);

            card = FindViewById<View>(Resource.Id.card);

            ivBackground = FindViewById<ImageView>(Resource.Id.iv_background);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);

            linearLayout2 = FindViewById<LinearLayout>(Resource.Id.linearLayout2);
            detailsParentLayout = FindViewById<LinearLayout>(Resource.Id.ll_detailsParent);
        }

        private void InitData()
        {
            tvName.Text = Intent.GetStringExtra(BUNDLE_NAME);
            tvInfo.Text = Intent.GetStringExtra(BUNDLE_INFO);
            tvStatus.Text = Faker.Beer.Alcohol();

            linearLayout2.Click += LinearLayout_Click;
            detailsParentLayout.Click += LinearLayout_Click;

            Glide.With(this)
                .Load(Intent.GetStringExtra(BUNDLE_AVATAR_URL))
                .Placeholder(Resource.Drawable.avatar_placeholder)
                .Transform(new ImageCircleTransformation(this))
                .Into(ivAvatar);

            for (int i = 0; i < ITEM_COUNT; i++)
            {
                mListData.Add(new DetailsData
                {
                    Title = Faker.Beer.Alcohol(),
                    Text = Faker.Name.FullName()
                });
            }

            recyclerView.SetAdapter(new DetailsAdapter(mListData));
        }

        public void LinearLayout_Click(object sender, EventArgs e)
        {
            switch ((sender as View)?.Id)
            {
                case Resource.Id.linearLayout2:
                    base.OnBackPressed();

                    break;

                case Resource.Id.ll_detailsParent:
                    ProfileActivity.Start(this,
                        Intent.GetStringExtra(BUNDLE_AVATAR_URL),
                        Intent.GetStringExtra(BUNDLE_NAME),
                        Intent.GetStringExtra(BUNDLE_INFO),
                        tvStatus.Text,
                        ivAvatar,
                        card,
                        ivBackground,
                        recyclerView,
                        mListData);

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                    {
                        FinishAfterTransition();
                    }
                    else
                    {
                        Finish();
                    }

                    break;
            }
        }

    }
}