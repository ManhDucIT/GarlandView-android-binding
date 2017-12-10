using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Ramotion.Garlandview;
using GarlandView.Droid.Main.Inner;
using Object = Java.Lang.Object;

namespace GarlandView.Droid.Main.Outer
{
    public class MainOuterAdapter : TailAdapter
    {

        private const int POOL_SIZE = 16;

        private readonly List<List<InnerData>> mData;
        private readonly RecyclerView.RecycledViewPool mPool;

        public MainOuterAdapter(List<List<InnerData>> data)
        {
            this.mData = data;

            mPool = new RecyclerView.RecycledViewPool();
            mPool.SetMaxRecycledViews(0, POOL_SIZE);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(viewType, parent, false);

            return new MainOuterItem(view, mPool);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            (holder as MainOuterItem)?.SetContent(mData[position]);
        }

        public override int GetItemViewType(int position)
        {
            return Resource.Layout.outer_item;
        }

        public override int ItemCount
        {
            get
            {
                return mData.Count;
            }
        }

        public override void OnViewRecycled(Object holder)
        {
            (holder as MainOuterItem)?.ClearContent();
        }

    }
}