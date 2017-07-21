using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.IO;
using FluentFTP;
using System.Net;

namespace savSyncAndorid
{
    [Activity(Label = "savSyncAndorid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        List<string> itemList = new List<string>();
        List<game> gameList = new List<game>();

        struct game
        {
            public string gameName;
            public string gamePath;

        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //Get our controls
            Button addButton = FindViewById<Button>(Resource.Id.btnAdd);
            Button removeButton = FindViewById<Button>(Resource.Id.btnRemove);
            Button getButton = FindViewById<Button>(Resource.Id.btnGet);
            Button putButton = FindViewById<Button>(Resource.Id.btnPut);

            //Set listView mode
            ListView listGames = FindViewById<ListView>(Resource.Id.listGames);
            listGames.ChoiceMode = ChoiceMode.Single;

            //Add custom functions for each button.
            addButton.Click += addButtonClick;
            removeButton.Click += removeButtonClick;
            getButton.Click += getButtonClick;
            putButton.Click += putButtonClick;

            //Heres where the fun starts.
            string line;
            string listGameName;
            string listGamePath;
            string path = Android.OS.Environment.ExternalStorageDirectory + "/Download";
            string listPath = path.ToString() + "/list.txt";


            //if list file exists, then parse it. if it doesn't, create it and move on.
            if (File.Exists(listPath) == true)
            {

                //Read through file, creating a gameList
                using (StreamReader sw = new StreamReader(listPath))
                {
                    while ((line = sw.ReadLine()) != null)
                    {
                        //skip any misshappen blank line
                        if (line == "")
                        {
                            continue;
                        }

                        int indexOfComma = line.IndexOf(",");
                        int lineLength = line.Length;

                        listGameName = line.Substring(0, indexOfComma);
                        listGamePath = line.Substring(indexOfComma + 1, lineLength - indexOfComma - 1);

                        game tempGame = new game();
                        tempGame.gameName = listGameName;
                        tempGame.gamePath = listGamePath;
                        gameList.Add(tempGame);
                    }
                }

                //Refresh the listbox with the fancy new gameList
                refreshListBox();
            }
            else
            {
                File.Create(listPath);
            }

        }


        async void addButtonClick(object obj, EventArgs e)
        {

            //Sets paths
            string path = Android.OS.Environment.ExternalStorageDirectory + "/Download";
            string listPath = path.ToString() + "/list.txt";

            //Creates a filePicker
            FileData fileData = await CrossFilePicker.Current.PickFile();

            //new games object for storing name/path into the gamesList
            game newGame = new game();

            newGame.gameName = fileData.FileName;
            newGame.gamePath = fileData.FilePath;

            gameList.Add(newGame);

            //Update the listFile
            using (StreamWriter sw = File.AppendText(listPath))
            {
                sw.WriteLine(newGame.gameName + "," + newGame.gamePath);

            }

            //refresh Listbox
            refreshListBox();
        }

        void removeButtonClick(object obj, EventArgs e)
        {
            //Sets paths and grabs ListView
            string path = Android.OS.Environment.ExternalStorageDirectory + "/Download";
            string listPath = path.ToString() + "/list.txt";
            ListView listGames = FindViewById<ListView>(Resource.Id.listGames);


            //remove the selected item from the gameList, unless nothing is selected
            if (listGames.CheckedItemPosition == -1)
            {
                return;
            }
            else
            {
                gameList.RemoveAt(listGames.CheckedItemPosition);

            }

            //Deletes the old listFile
            File.Delete(listPath);

            //Write a new list file with contents of GamesList
            using (StreamWriter sw = File.AppendText(listPath))
            {
                for (int x = 0; x < gameList.Count; x++)
                {
                    sw.WriteLine(gameList[x].gameName + "," + gameList[x].gamePath);
                }

            }
            refreshListBox();

        }

        void getButtonClick(object obj, EventArgs e)
        {

            ListView listGames = FindViewById<ListView>(Resource.Id.listGames);

            //Gets index of selected Item
            int gameIndex = listGames.CheckedItemPosition;

            string downloadSavePath = gameList[gameIndex].gamePath;
            string downloadSaveName = gameList[gameIndex].gameName;

            //Makes a messagebox thing
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            AlertDialog alertDialog = builder.Create();
            alertDialog.SetTitle("Finished!");


            FtpClient client = new FtpClient("127.0.0.1");
            client.Credentials = new NetworkCredential("user", "password");

            client.Connect();

            //if the file exists on the ftp server, download it to the path recorded in the list
            if (client.FileExists("//Desktop//FTP//" + downloadSaveName))
            {
                bool success = client.DownloadFile(downloadSavePath, "//Desktop//FTP//" + downloadSaveName);

                if (success == true)
                {
                    alertDialog.SetMessage("It success!");
                }
                else
                {
                    alertDialog.SetMessage("It failure...");
                }
            }
            else
            {
                alertDialog.SetMessage("Remote file didn't exist...");
            }

            alertDialog.Show();
            client.Disconnect();

        }

        void putButtonClick(object obj, EventArgs e)
        {

            ListView listGames = FindViewById<ListView>(Resource.Id.listGames);

            //gets selected item index
            int gameIndex = listGames.CheckedItemPosition;

            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            AlertDialog alertDialog = builder.Create();
            alertDialog.SetTitle("Finished!");

            //gets the file we're gonna ftp
            string uploadSavePath = gameList[gameIndex].gamePath;
            string uploadSaveName = gameList[gameIndex].gameName;

            FtpClient client = new FtpClient("127.0.0.1");
            client.Credentials = new NetworkCredential("user", "password");

            client.Connect();

            bool success = client.UploadFile(uploadSavePath, "//Desktop//FTP//" + uploadSaveName, FtpExists.Overwrite);
            if (success == true)
            {
                alertDialog.SetMessage("It success!");
            }
            else
            {
                alertDialog.SetMessage("It failure...");
            }
            client.Disconnect();

    }

        public void refreshListBox()
        {
            ListView listGames = FindViewById<ListView>(Resource.Id.listGames);
            //Clear the listbox
            itemList.Clear();

            for (int x = 0; x < gameList.Count; x++)
            {
                itemList.Add(gameList[x].gameName);
            }
            
            //Populates listview with items in itemList
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItemSingleChoice, itemList);

            listGames.Adapter = adapter;

        }


    }
}

