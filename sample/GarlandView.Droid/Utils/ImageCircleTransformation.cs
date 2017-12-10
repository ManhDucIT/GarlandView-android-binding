using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide.Load.Engine.Bitmap_recycle;
using Com.Bumptech.Glide.Load.Resource.Bitmap;

namespace GarlandView.Droid
{
    public class ImageCircleTransformation : BitmapTransformation
    {

        public ImageCircleTransformation(Android.Content.Context context) : base(context)
        {
        }

        public override string Id => "ImageCircleTransformation";

        protected override Bitmap Transform(IBitmapPool bitmapPool, Bitmap source, int outWidth, int outHeight)
        {
            int size = Math.Min(source.Width, source.Height);

            int width = (source.Width - size) / 2;
            int height = (source.Height - size) / 2;

            Bitmap squaredBitmap = Bitmap.CreateBitmap(source, width, height, size, size);

            if (squaredBitmap != source)
            {
                source.Recycle();
            }

            Bitmap bitmap = Bitmap.CreateBitmap(size, size, Bitmap.Config.Argb8888);

            Canvas canvas = new Canvas(bitmap);

            Paint paint = new Paint();

            BitmapShader shader = new BitmapShader(squaredBitmap, BitmapShader.TileMode.Clamp,
                BitmapShader.TileMode.Clamp);

            paint.SetShader(shader);
            paint.AntiAlias = true;

            float r = size / 2f;

            canvas.DrawCircle(r, r, r, paint);

            squaredBitmap.Recycle();

            return BitmapResource.Obtain(bitmap, bitmapPool).Get();
        }
    }

}