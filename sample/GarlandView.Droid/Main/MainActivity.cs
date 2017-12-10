using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Com.Ramotion.Garlandview;
using Com.Ramotion.Garlandview.Header;
using GarlandView.Droid.Main.Inner;
using GarlandView.Droid.Main.Outer;

namespace GarlandView.Droid.Main
{
    [Activity(Label = "GarlandView.Droid", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AppTheme.NoActionBar")]
    public class MainActivity : AppCompatActivity
    {

        private const int OUTER_COUNT = 10;
        private const int INNER_COUNT = 20;

        public View mProgressBar;
        public TailRecyclerView mTailRecyclerView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView (Resource.Layout.activity_main);

            InitViews();
            InitData();
        }

        private void InitViews()
        {
            mProgressBar = FindViewById<View>(Resource.Id.progressBar);
            mTailRecyclerView = FindViewById<TailRecyclerView>(Resource.Id.recycler_view);
        }

        private void InitData()
        {
            List<List<InnerData>> outerData = new List<List<InnerData>>();

            for (int i = 0; i < OUTER_COUNT; i++)
            {
                List<InnerData> innerData = new List<InnerData>();

                for (int j = 0; j < INNER_COUNT; j++)
                {
                    innerData.Add(CreateInnerData());
                }

                outerData.Add(innerData);
            }

            mProgressBar.Visibility = ViewStates.Gone;

            (mTailRecyclerView.GetLayoutManager() as TailLayoutManager)?.SetPageTransformer(new HeaderTransformer());

            mTailRecyclerView.SetAdapter(new MainOuterAdapter(outerData));

            new TailSnapHelper().AttachToRecyclerView(mTailRecyclerView);
        }

        private InnerData CreateInnerData()
        {
            return new InnerData
            {
                Title = Faker.Beer.Name(),
                Name = Faker.Name.FullName(),
                Address = Faker.Address.City() + ", " + Faker.Address.StateAbbreviation(),
                AvatarUrl = Faker.RoboHash.Image(Faker.Internet.Email(), "150x150", Faker.RoboHashImageFormat.jpg),
                Age = Faker.RandomNumber.Next(20, 50)
            };
        }
    }
}

