using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using Com.Ramotion.Garlandview.Header;
using Com.Ramotion.Garlandview.Inner;
using GarlandView.Droid.Main.Inner;

namespace GarlandView.Droid.Main.Outer
{
    public class MainOuterItem : HeaderItem
    {

        private const float AVATAR_RATIO_START = 1f;
        private const float AVATAR_RATIO_MAX = 0.25f;
        private const float AVATAR_RATIO_DIFF = AVATAR_RATIO_START - AVATAR_RATIO_MAX;

        private const float ANSWER_RATIO_START = 0.75f;
        private const float ANSWER_RATIO_MAX = 0.35f;
        private const float ANSWER_RATIO_DIFF = ANSWER_RATIO_START - ANSWER_RATIO_MAX;

        private const float MIDDLE_RATIO_START = 0.7f;
        private const float MIDDLE_RATIO_MAX = 0.1f;
        private const float MIDDLE_RATIO_DIFF = MIDDLE_RATIO_START - MIDDLE_RATIO_MAX;

        private const float FOOTER_RATIO_START = 1.1f;
        private const float FOOTER_RATIO_MAX = 0.35f;
        private const float FOOTER_RATIO_DIFF = FOOTER_RATIO_START - FOOTER_RATIO_MAX;

        private readonly View itemView;

        private readonly View mHeader;
        private readonly View mHeaderAlpha;

        private readonly InnerRecyclerView mRecyclerView;

        private readonly ImageView mAvatar;
        private readonly TextView mHeaderCaption1;
        private readonly TextView mHeaderCaption2;
        private readonly TextView mName;
        private readonly TextView mInfo;

        private readonly View mMiddle;
        private readonly View mMiddleAnswer;
        private readonly View mFooter;

        private readonly IList<View> mMiddleCollapsible = new List<View>(2);

        private readonly int m10dp;
        private readonly int m120dp;
        private readonly int mTitleSize1;
        private readonly int mTitleSize2;

        public bool mIsScrolling;

        public MainOuterItem(View p0, RecyclerView.RecycledViewPool pool) : base(p0)
        {
            this.itemView = p0;

            // Init header
            m10dp = p0.Context.Resources.GetDimensionPixelSize(Resource.Dimension.dp10);
            m120dp = p0.Context.Resources.GetDimensionPixelSize(Resource.Dimension.dp120);
            mTitleSize1 = p0.Context.Resources.GetDimensionPixelSize(Resource.Dimension.header_title2_text_size);
            mTitleSize2 = p0.Context.Resources.GetDimensionPixelSize(Resource.Dimension.header_title2_name_text_size);

            mHeader = p0.FindViewById<View>(Resource.Id.header);
            mHeaderAlpha = p0.FindViewById<View>(Resource.Id.header_alpha);

            mHeaderCaption1 = p0.FindViewById<TextView>(Resource.Id.header_text_1);
            mHeaderCaption2 = p0.FindViewById<TextView>(Resource.Id.header_text_2);
            mName = p0.FindViewById<TextView>(Resource.Id.tv_name);
            mInfo = p0.FindViewById<TextView>(Resource.Id.tv_info);
            mAvatar = p0.FindViewById<ImageView>(Resource.Id.avatar);

            mMiddle = p0.FindViewById<View>(Resource.Id.header_middle);
            mMiddleAnswer = p0.FindViewById<View>(Resource.Id.header_middle_answer);
            mFooter = p0.FindViewById<View>(Resource.Id.header_footer);

            mMiddleCollapsible.Add(mAvatar.Parent as View);
            mMiddleCollapsible.Add(mName.Parent as View);

            // Init RecyclerView
            mRecyclerView = (InnerRecyclerView)p0.FindViewById(Resource.Id.recycler_view);
            mRecyclerView.SetRecycledViewPool(pool);
            mRecyclerView.SetAdapter(new MainInnerAdapter());

            mRecyclerView.AddOnScrollListener(new ScrollListener(this));

            mRecyclerView.AddItemDecoration(new HeaderDecorator(
                itemView.Context.Resources.GetDimensionPixelSize(Resource.Dimension.inner_item_height),
                itemView.Context.Resources.GetDimensionPixelSize(Resource.Dimension.inner_item_offset)));
        }

        public override bool IsScrolling
        {
            get
            {
                return mIsScrolling;
            }
        }

        public override Android.Views.ViewGroup ViewGroup
        {
            get
            {
                return mRecyclerView;
            }
        }

        public void SetContent(List<InnerData> innerDataList)
        {
            Context context = itemView.Context;

            InnerData header = innerDataList.GetRange(0, 1)[0];

            List<InnerData> tail = innerDataList.GetRange(1, innerDataList.Count - 1);

            mRecyclerView.SetLayoutManager(new InnerLayoutManager());

            (mRecyclerView.GetAdapter() as MainInnerAdapter)?.AddData(tail);

            Glide.With(context)
                    .Load(header.AvatarUrl)
                    .Placeholder(Resource.Drawable.avatar_placeholder)
                    .Transform(new ImageCircleTransformation(context))
                    .Into(mAvatar);

            string title1 = header.Title + "?";

            SpannableString title2 = new SpannableString(header.Title + "? - " + header.Name);
            title2.SetSpan(new AbsoluteSizeSpan(mTitleSize1), 0, title1.Length, SpanTypes.InclusiveInclusive);
            title2.SetSpan(new AbsoluteSizeSpan(mTitleSize2), title1.Length, title2.Length(), SpanTypes.InclusiveInclusive);
            title2.SetSpan(new ForegroundColorSpan(Color.Argb(204, 255, 255, 255)), title1.Length, title2.Length(), SpanTypes.InclusiveInclusive);

            mHeaderCaption1.Text = title1;
            mHeaderCaption2.TextFormatted = title2;

            mName.Text = string.Format("%s %s",
                header.Name,
                context.GetString(Resource.String.asked)
            );

            mInfo.Text = string.Format("%s %s · %s",
                header.Age,
                context.GetString(Resource.String.years),
                header.Address
            );
        }

        public void ClearContent()
        {
            Glide.Clear(mAvatar);
            (mRecyclerView.GetAdapter() as MainInnerAdapter)?.ClearData();
        }

        private float ComputeRatio(RecyclerView recyclerView)
        {
            View child0 = recyclerView.GetChildAt(0);

            int pos = recyclerView.GetChildAdapterPosition(child0);
            if (pos != 0)
            {
                return 0;
            }

            int height = child0.Height;
            float y = Math.Max(0, child0.GetY());

            return y / height;
        }

        private void OnItemScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            float ratio = ComputeRatio(recyclerView);

            float footerRatio = Math.Max(0, Math.Min(FOOTER_RATIO_START, ratio) - FOOTER_RATIO_DIFF) / FOOTER_RATIO_MAX;
            float avatarRatio = Math.Max(0, Math.Min(AVATAR_RATIO_START, ratio) - AVATAR_RATIO_DIFF) / AVATAR_RATIO_MAX;
            float answerRatio = Math.Max(0, Math.Min(ANSWER_RATIO_START, ratio) - ANSWER_RATIO_DIFF) / ANSWER_RATIO_MAX;
            float middleRatio = Math.Max(0, Math.Min(MIDDLE_RATIO_START, ratio) - MIDDLE_RATIO_DIFF) / MIDDLE_RATIO_MAX;

            ViewCompat.SetPivotY(mFooter, 0);
            ViewCompat.SetScaleY(mFooter, footerRatio);
            ViewCompat.SetAlpha(mFooter, footerRatio);

            ViewCompat.SetPivotY(mMiddleAnswer, mMiddleAnswer.Height);
            ViewCompat.SetScaleY(mMiddleAnswer, 1f - answerRatio);
            ViewCompat.SetAlpha(mMiddleAnswer, 0.5f - answerRatio);

            ViewCompat.SetAlpha(mHeaderCaption1, answerRatio);
            ViewCompat.SetAlpha(mHeaderCaption2, 1f - answerRatio);

            View mc2 = mMiddleCollapsible[1];

            ViewCompat.SetPivotX(mc2, 0);
            ViewCompat.SetPivotY(mc2, mc2.Height / 2);

            foreach (View view in mMiddleCollapsible)
            {
                ViewCompat.SetScaleX(view, avatarRatio);
                ViewCompat.SetScaleY(view, avatarRatio);
                ViewCompat.SetAlpha(view, avatarRatio);
            }

            ViewGroup.LayoutParams lp = mMiddle.LayoutParameters;

            lp.Height = m120dp - (int)(m10dp * (1f - middleRatio));

            mMiddle.LayoutParameters = lp;
        }

        public class ScrollListener : RecyclerView.OnScrollListener
        {

            private MainOuterItem outerItem;

            public ScrollListener(MainOuterItem outerItem)
            {
                this.outerItem = outerItem;
            }
            public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
            {
                outerItem.mIsScrolling = newState != RecyclerView.ScrollStateIdle;
            }

            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                outerItem.OnItemScrolled(recyclerView, dx, dy);
            }

        }

        public override View Header
        {
            get
            {
                return mHeader;
            }
        }

        public override View HeaderAlphaView
        {
            get
            {
                return mHeaderAlpha;
            }
        }
    }
}