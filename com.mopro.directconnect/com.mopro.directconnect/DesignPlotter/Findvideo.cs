﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Database;
using static Android.Provider.MediaStore;
using Android.Provider;
using Android.Media;
using Android.Graphics;
using static Android.Provider.MediaStore.Video;
using Android.Util;
using FFImageLoading;
using Android.Content.PM;

namespace com.mopro.directconnect
{
    [Activity(Label = "Findvideo", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Findvideo : Activity
    {
        ImageHandler ObjImageHandler;
        List<String> objImages = new List<String>();
        ExpandableHeightGridView gallery;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Viewimages);
            Window.SetSoftInputMode(SoftInput.AdjustResize | SoftInput.StateHidden);
            ObjImageHandler = new ImageHandler(this);
            // Click Back button Events Occurs below
            ImageView Backbutton = FindViewById<ImageView>(Resource.Id.Backbutton);
            Backbutton.Click += (o, e) => {
                StartActivity(new Intent(this, typeof(Videoupdates)));
                OverridePendingTransition(Resource.Drawable.slide_from_left, Resource.Drawable.slide_to_right);
            };
            ImageView Capturebutton = FindViewById<ImageView>(Resource.Id.Capturebutton);
            Capturebutton.SetImageResource(Resource.Drawable.camera);
            Capturebutton.Click += (o, e) => {
                ObjImageHandler.VideoAction();
            };
            TextView headingtext = FindViewById<TextView>(Resource.Id.headingtext);
            headingtext.SetTypeface(AppFont.GetTitle(this), TypefaceStyle.Normal);
            headingtext.Text = "VIDEOS";
            getAllShownImagesPath(this);
            gallery = FindViewById<ExpandableHeightGridView>(Resource.Id.galleryGridView);
            gallery.FastScrollEnabled = (true);
            gallery.ScrollingCacheEnabled = (false);
            Loadimages();
        }
        public async void Loadimages()
        {
            this.RunOnUiThread(() =>
            {
                gallery.Adapter = new ImageAdapter(this, objImages);
                gallery.ItemClick += Gallery_ItemClick;
            });
        }
        //This Method is used to perform Resultof actitives
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                if (resultCode != Result.Canceled)
                {
                    base.OnActivityResult(requestCode, resultCode, data);
                    if (requestCode == 1 && resultCode == Result.Ok)
                    {
                        if (data != null)
                        {
                            ObjImageHandler.picUri = data.Data;
                            Intent objIntent = new Intent(this, typeof(Videoupdates));
                            objIntent.PutExtra("video", ObjImageHandler.picUri.Path);
                            StartActivity(objIntent);
                        }
                        else
                        {
                            Intent mediascanintent = new Intent(Intent.ActionMediaScannerScanFile);
                            ObjImageHandler.picUri = Android.Net.Uri.FromFile(ObjImageHandler._file);
                            Intent objIntent = new Intent(this, typeof(Videoupdates));
                            objIntent.PutExtra("video", ObjImageHandler.picUri.Path);
                            StartActivity(objIntent);
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void Gallery_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent objIntent = new Intent(this, typeof(Videoupdates));
            objIntent.PutExtra("video", objImages[e.Position]);
            StartActivity(objIntent);
            OverridePendingTransition(Resource.Drawable.slide_from_left, Resource.Drawable.slide_to_right);
        }
        // Click Back button Events Occurs below
        public override void OnBackPressed()
        {
            base.OnBackPressed();
            StartActivity(new Intent(this, typeof(Videoupdates)));
            OverridePendingTransition(Resource.Drawable.slide_from_left, Resource.Drawable.slide_to_right);
        }
        /**
    * Getting All Images Path.
    *
    * @param activity
    *            the activity
    * @return ArrayList with images Path
    */
        private void getAllShownImagesPath(Activity activity)
        {
            objImages = new List<String>();
            Android.Net.Uri uri;
            ICursor cursor;
            int column_index_data, column_index_folder_name;
            String absolutePathOfImage = null;
            uri = Android.Provider.MediaStore.Video.Media.ExternalContentUri;
            String[] projection = { MediaColumns.Data, MediaStore.Video.Media.InterfaceConsts.BucketDisplayName };
            cursor = activity.ContentResolver.Query(uri, projection, null, null, null);
            column_index_data = cursor.GetColumnIndexOrThrow(MediaColumns.Data);
            column_index_folder_name = cursor.GetColumnIndexOrThrow(MediaStore.Video.Media.InterfaceConsts.BucketDisplayName);
            while (cursor.MoveToNext())
            {
                absolutePathOfImage = cursor.GetString(column_index_data);
                objImages.Add(absolutePathOfImage);
            }
        }
        private void SelectedImagesPath(Activity activity)
        {
            objImages = new List<String>();
            Android.Net.Uri uri;
            ICursor cursor;
            int column_index_data, column_index_folder_name;
            String absolutePathOfImage = null;
            uri = Android.Provider.MediaStore.Video.Media.ExternalContentUri;
            String[] projection = { MediaColumns.Data, MediaStore.Video.Media.InterfaceConsts.BucketDisplayName };
            cursor = activity.ContentResolver.Query(uri, projection, null, null, null);
            column_index_data = cursor.GetColumnIndexOrThrow(MediaColumns.Data);
            column_index_folder_name = cursor.GetColumnIndexOrThrow(MediaStore.Video.Media.InterfaceConsts.BucketDisplayName);
            while (cursor.MoveToNext())
            {
                absolutePathOfImage = cursor.GetString(column_index_data);
                objImages.Add(absolutePathOfImage);
            }
        }
        private class ImageAdapter : BaseAdapter<String>
        {

            /** The context. */
            private Activity context;
            private List<String> images;
            /**
             * Instantiates a new image adapter.
             *
             * @param localContext
             *            the local context
             */
            public ImageAdapter(Activity localContext, List<String> imagesv)
            {
                context = localContext;
                images = imagesv;
            }
            // Assign position for every item
            public override long GetItemId(int position)
            {
                return position;
            }
            public override String this[int position]
            {
                get { return images[position]; }
            }
            public override int Count
            {
                get { return images.Count; }
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                View view = convertView;
                if (view == null)
                    view = context.LayoutInflater.Inflate(Resource.Layout.viewimageslist, null);
                FFImageLoading.Views.ImageViewAsync objImageView = view.FindViewById<FFImageLoading.Views.ImageViewAsync>(Resource.Id.Image);
                try
                {
                    context.RunOnUiThread(() =>
                {
                    //ImageService.Instance.LoadFile(images[position])
                    //   .Retry(2, 200)
                    //   .DownSample(context.Resources.DisplayMetrics.WidthPixels / 3, context.Resources.DisplayMetrics.WidthPixels / 3)
                    //   .LoadingPlaceholder("article_placeholder", FFImageLoading.Work.ImageSource.ApplicationBundle)
                    //   .ErrorPlaceholder("article_placeholder", FFImageLoading.Work.ImageSource.ApplicationBundle)
                    //   .Into(objImageView);
                    Bitmap bmThumbnail = ThumbnailUtils.CreateVideoThumbnail(images[position], Android.Provider.ThumbnailKind.MiniKind);
                    objImageView.SetImageBitmap(bmThumbnail);
                });
                }
                catch { }
                //objImageView.SetImageBitmap(ThumbnailUtils.CreateVideoThumbnail(images[position], Android.Provider.VideoThumbnailKind));
                return view;
            }

        }
    }
    public class ExpandableHeightGridView : GridView
    {
        bool _isExpanded = false;

        public ExpandableHeightGridView(Context context) : base(context)
        {
        }

        public ExpandableHeightGridView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public ExpandableHeightGridView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }

            set { _isExpanded = value; }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            if (IsExpanded)
            {
                int expandSpec = MeasureSpec.MakeMeasureSpec(View.MeasuredSizeMask, MeasureSpecMode.AtMost);
                base.OnMeasure(widthMeasureSpec, expandSpec);

                var layoutParameters = this.LayoutParameters;
                layoutParameters.Height = this.MeasuredHeight;
            }
            else
            {
                base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            }
        }
    }
}