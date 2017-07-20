using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.IO;

namespace savSyncAndorid
{
    [Activity(Label = "savSynAndorid", MainLauncher = true, Icon = "@drawable/icon")]
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

            Button addButton = FindViewById<Button>(Resource.Id.btnAdd);
            Button removeButton = FindViewById<Button>(Resource.Id.btnRemove);
            Button getButton = FindViewById<Button>(Resource.Id.btnGet);
            Button putButton = FindViewById<Button>(Resource.Id.btnPut);

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
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string listPath = path + "/list.txt";


            //if file exists, then parse it. if it doesn't create it and move on.
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


        void addButtonClick(object obj, EventArgs e)
        {
            /*
            Intent fileintent = new Intent(Intent.ActionGetContent);
            fileintent.SetType("gagt/sdf");
            string selectedFile = StartActivityForResult(fileintent, 1);

            int indexOfSlash = selectedFile.LastIndexOf("\\");

            string newGameName = selectedFile.Substring(indexOfSlash + 1, selectedFile.Length - indexOfSlash - 1);
            string newGamePath = selectedFile;

            //Create a new gameObject to store the name/path in so we can add to list
            game newGame = new game();

            newGame.gameName = newGameName;
            newGame.gamePath = newGamePath;
            gameList.Add(newGame);

            //Update the listFile
            using (StreamWriter sw = File.AppendText(listPath))
            {
                sw.WriteLine(newGame.gameName + "," + newGame.gamePath);

            }
            */
            //refresh Listbox
            refreshListBox();
        }

        void removeButtonClick(object obj, EventArgs e)
        {

        }

        void getButtonClick(object obj, EventArgs e)
        {


        }

        void putButtonClick(object obj, EventArgs e)
        {


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
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItemSingleChoice, itemList);

            listGames.Adapter = adapter;

        }


    }
}

