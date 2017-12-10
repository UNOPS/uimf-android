using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AndroidApp.Forms.Inputs
{
    using System.IO;
    using Android.Util;

    public class FileListFragment : ListFragment
    {
        public static readonly string DefaultInitialDirectory = "/";
        private FileListAdapter _adapter;
        private DirectoryInfo _directory;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this._adapter = new FileListAdapter(this.Activity, new FileSystemInfo[0]);
            this.ListAdapter = this._adapter;
        }

        public override void OnListItemClick(ListView l, View v, int position, long id)
        {
            var fileSystemInfo = this._adapter.GetItem(position);

            if (fileSystemInfo.IsFile())
            {
                // Do something with the file.  In this case we just pop some toast.
                Log.Verbose("FileListFragment", "The file {0} was clicked.", fileSystemInfo.FullName);
                Toast.MakeText(this.Activity, "You selected file " + fileSystemInfo.FullName, ToastLength.Short).Show();
            }
            else
            {
                // Dig into this directory, and display it's contents
                this.RefreshFilesList(fileSystemInfo.FullName);
            }

            base.OnListItemClick(l, v, position, id);
        }

        public override void OnResume()
        {
            base.OnResume();
            this.RefreshFilesList(DefaultInitialDirectory);
        }

        public void RefreshFilesList(string directory)
        {
            IList<FileSystemInfo> visibleThings = new List<FileSystemInfo>();
            var dir = new DirectoryInfo(directory);

            try
            {
                foreach (var item in dir.GetFileSystemInfos().Where(item => item.IsVisible()))
                {
                    visibleThings.Add(item);
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, "Problem retrieving contents of " + directory, ToastLength.Long).Show();
                return;
            }

            this._directory = dir;

            this._adapter.AddDirectoryContents(visibleThings);

            // If we don't do this, then the ListView will not update itself when then data set 
            // in the adapter changes. It will appear to the user that nothing has happened.
            this.ListView.RefreshDrawableState();
        }
    }



    public class FileListAdapter : ArrayAdapter<FileSystemInfo>
    {
        private readonly Context _context;

        public FileListAdapter(Context context, IList<FileSystemInfo> fsi)
            : base(context, Resource.Layout.file_picker_list_item, Android.Resource.Id.Text1, fsi)
        {
            this._context = context;
        }

        /// <summary>
        ///   We provide this method to get around some of the
        /// </summary>
        /// <param name="directoryContents"> </param>
        public void AddDirectoryContents(IEnumerable<FileSystemInfo> directoryContents)
        {
            this.Clear();
            if (directoryContents.Any())
            {
                // .AddAll was only introduced in API level 11 (Android 3.0). 
                // If the "Minimum Android to Target" is set to Android 3.0 or 
                // higher, then this code will be used.
                this.AddAll(directoryContents.ToArray());

                this.NotifyDataSetChanged();
            }
            else
            {
                this.NotifyDataSetInvalidated();
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var fileSystemEntry = this.GetItem(position);

            FileListRowViewHolder viewHolder;
            View row;
            if (convertView == null)
            {
                row = this._context.GetLayoutInflater().Inflate(Resource.Layout.file_picker_list_item, parent, false);
                viewHolder = new FileListRowViewHolder(row.FindViewById<TextView>(Resource.Id.file_picker_text),
                    row.FindViewById<ImageView>(Resource.Id.file_picker_image));
                row.Tag = viewHolder;
            }
            else
            {
                row = convertView;
                viewHolder = (FileListRowViewHolder)row.Tag;
            }
            viewHolder.Update(fileSystemEntry.Name, fileSystemEntry.IsDirectory() ? Resource.Drawable.folder : Resource.Drawable.file);

            return row;
        }
    }

    public class FileListRowViewHolder : Java.Lang.Object
    {
        public FileListRowViewHolder(TextView textView, ImageView imageView)
        {
            this.TextView = textView;
            this.ImageView = imageView;
        }

        public ImageView ImageView { get; private set; }
        public TextView TextView { get; private set; }

        /// <summary>
        ///   This method will update the TextView and the ImageView that are
        ///   are
        /// </summary>
        /// <param name="fileName"> </param>
        /// <param name="fileImageResourceId"> </param>
        public void Update(string fileName, int fileImageResourceId)
        {
            this.TextView.Text = fileName;
            this.ImageView.SetImageResource(fileImageResourceId);
        }
    }

    public static class Helpers
    {
        /// <summary>
        ///   Will obtain an instance of a LayoutInflater for the specified Context.
        /// </summary>
        /// <param name="context"> </param>
        /// <returns> </returns>
        public static LayoutInflater GetLayoutInflater(this Context context)
        {
            return context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
        }

        /// <summary>
        ///   This method will tell us if the given FileSystemInfo instance is a directory.
        /// </summary>
        /// <param name="fsi"> </param>
        /// <returns> </returns>
        public static bool IsDirectory(this FileSystemInfo fsi)
        {
            if (fsi == null || !fsi.Exists)
            {
                return false;
            }

            return (fsi.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
        }

        /// <summary>
        ///   This method will tell us if the the given FileSystemInfo instance is a file.
        /// </summary>
        /// <param name="fsi"> </param>
        /// <returns> </returns>
        public static bool IsFile(this FileSystemInfo fsi)
        {
            if (fsi == null || !fsi.Exists)
            {
                return false;
            }
            return !IsDirectory(fsi);
        }

        public static bool IsVisible(this FileSystemInfo fsi)
        {
            if (fsi == null || !fsi.Exists)
            {
                return false;
            }

            var isHidden = (fsi.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
            return !isHidden;
        }
    }
}